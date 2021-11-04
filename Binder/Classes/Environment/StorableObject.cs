using Binder.UI;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Timers;
using System.Windows;

namespace Binder.Environment
{
    abstract class StorableObject : ViewModel, IDisposable //BUG утечка памяти
    {
        private const uint AUTOSAVE_INTERVAL = 60000;

        public delegate void StorableObjectHandler(Type affectedType);
        public static event StorableObjectHandler SomethingChanged;

        #region cache

        private static Dictionary<Guid, WeakReference> _storableObjectsCache { get; } = new Dictionary<Guid, WeakReference>();

        private static T GetObjectOfTypeFromCacheById<T>(Guid id)
        {
            WeakReference result = new WeakReference(new SomeClass());
            if (_storableObjectsCache.TryGetValue(id, out result) && result.IsAlive) return (T)result.Target;
            else throw new IOException("Object of type " + typeof(T).FullName + " not found by id " + id);
        }
        private static bool TryGetObjectOfTypeFromCacheById<T>(Guid id, out T result)
        {
            try
            {
                result = GetObjectOfTypeById<T>(id);
                return true;
            }
            catch
            {
                result = default(T);
                return false;
            }
        }

        #endregion
        public static T[] GetAllObjectsOfType<T>()
        {
            string[] files = GetFilesByType<T>();
            T[] objects = new T[files.Length];
            for (int i = 0; i < files.Length; i++)
            {
                Guid id = Guid.Parse(Path.GetFileNameWithoutExtension(files[i]));
                if (!TryGetObjectOfTypeFromCacheById<T>(id, out objects[i]))
                {
                    objects[i] = JsonConvert.DeserializeObject<T>(File.ReadAllText(files[i]));
                    _storableObjectsCache.Add(id, new WeakReference(objects[i]));
                }
            }

            return objects;
        }

        public static T GetObjectOfTypeById<T>(Guid id)
        {
            WeakReference result1;
            if (_storableObjectsCache.TryGetValue(id, out result1) && result1.IsAlive) return (T)result1.Target;
            else if (File.Exists("Data/" + typeof(T).Name + '/' + id + ".json"))
            {
                T result2 = JsonConvert.DeserializeObject<T>(File.ReadAllText("Data/" + typeof(T).Name + '/' + id + ".json"));
                _storableObjectsCache.Add(id, new WeakReference(result2));
                return result2;
            }
            else throw new IOException("Object of type " + typeof(T).FullName + "not found by id " + id);
        }

        public static void DeleteById<T>(Guid id)
        {
            if (!File.Exists("Data/" + typeof(T).Name + id.ToString() + ".json")) throw new IOException("File \"" + "Data/" + typeof(T).Name + id.ToString() + ".json" + "\" does not exists");
            File.Delete("Data/" + typeof(T).Name + id.ToString() + ".json");
            if (_storableObjectsCache.ContainsKey(id)) _storableObjectsCache.Remove(id);

            SomethingChanged.Invoke(typeof(T));
        }

        private static string[] GetFilesByType<T>()
        {
            string directoryPath = "Data/" + typeof(T).Name;
            if (!Directory.Exists(directoryPath)) throw new IOException("Directory \"" + directoryPath + "\" not found");
            string[] files = Directory.GetFiles(directoryPath);
            return files;
        }


        public Guid Id { get; private init; }


        [JsonConstructor]
        public StorableObject(Guid id, bool usingStorage = true)
        {
            Id = id;
            if (usingStorage)
            {
                InitializeTimer();
                App.Current.Dispatcher.Invoke(() =>
                {
                    WeakEventManager<Application, ExitEventArgs>.AddHandler(App.Current, "Exit", OnExitApp);
                });
            }
            
        }
        public StorableObject() : this(Guid.NewGuid())
        {
            Save(false);
            SomethingChanged.Invoke(GetType());
        }

        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        public void Delete()
        {
            File.Delete(_filePath);
            Dispose();
        }

        private void OnExitApp(object sender, ExitEventArgs e)
        {
            Save(false);
        }

        


        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing)
            {
                
            }
            

            _storableObjectsCache.Remove(Id);
            WeakEventManager<Timer, ElapsedEventArgs>.RemoveHandler(_autoSaveTimer, "Elapsed", _autoSaveTimer_Elapsed);
            WeakEventManager<Application, ExitEventArgs>.RemoveHandler(App.Current, "Exit", OnExitApp);
            _autoSaveTimer.Dispose();


            
            _disposed = true;
        }


        private void Save(bool checkLenght)
        {
            string serialized = _serialized;
            if (!checkLenght || _lastLenght != serialized.Length)
            {
                if (!Directory.Exists("Data/" + GetType().Name)) Directory.CreateDirectory("Data/" + GetType().Name);
                using (StreamWriter stream = new StreamWriter(_filePath, false))
                {
                    stream.Write(serialized);
                }
                _lastLenght = serialized.Length;
            }
        }

        private void InitializeTimer()
        {
            _autoSaveTimer = new Timer(AUTOSAVE_INTERVAL);
            WeakEventManager<Timer, ElapsedEventArgs>.AddHandler(_autoSaveTimer, "Elapsed", _autoSaveTimer_Elapsed);
            _autoSaveTimer.AutoReset = true;
            _autoSaveTimer.Start();
        }

        private void _autoSaveTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Save(true);
        }

        ~StorableObject()
        {
            Save(false);
            Dispose(false);
        }

        private string _filePath => "Data/" + GetType().Name + '/' + Id.ToString() + ".json";
        private string _serialized => JsonConvert.SerializeObject(this);
        private int _lastLenght = 0;
        private Timer _autoSaveTimer;
        protected bool _disposed = false;
    }

    class SomeClass : StorableObject
    {

        [JsonConstructor]
        public SomeClass(Guid id) : base(id)
        {

        }
        public SomeClass() : base()
        {

        }
    }
}
