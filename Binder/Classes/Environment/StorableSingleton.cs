using Binder.UI;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;

namespace Binder.Environment
{
    abstract class StorableSingleton : ViewModel, IDisposable
    {
        private const uint AUTOSAVE_INTERVAL = 60000;

        #region cache
        private static Dictionary<Type, WeakReference> _singletonStorableObjectsCache { get; } = new Dictionary<Type, WeakReference>();
        #endregion

        public static T GetSingleton<T>()
        {
            if (_singletonStorableObjectsCache.ContainsKey(typeof(T)))
            {
                if (_singletonStorableObjectsCache[typeof(T)].IsAlive) return (T)_singletonStorableObjectsCache[typeof(T)].Target;
                else _singletonStorableObjectsCache.Remove(typeof(T));
            }

            if (File.Exists("Data/" + typeof(T).Name + ".json"))
            {
                T result = JsonConvert.DeserializeObject<T>(File.ReadAllText("Data/" + typeof(T).Name + ".json"));
                _singletonStorableObjectsCache.Add(typeof(T), new WeakReference(result));
                return result;
            }
            else throw new IOException("Singleton of type " + typeof(T).FullName + " not found");
        }


        public static void DeleteSingleton<T>()
        {
            if (!File.Exists("Data/" + typeof(T).Name + ".json")) throw new IOException("Singleton object of type " + typeof(T).FullName + " not found");

            File.Delete("Data/" + typeof(T).Name + ".json");
            if (_singletonStorableObjectsCache.ContainsKey(typeof(T))) _singletonStorableObjectsCache.Remove(typeof(T));
        }


        [JsonConstructor]
        public StorableSingleton()
        {
            InitializeTimer();
            App.Current.Dispatcher.Invoke(() =>
            {
                WeakEventManager<Application, ExitEventArgs>.AddHandler(App.Current, "Exit", OnExitApp);
            });
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

            _singletonStorableObjectsCache.Remove(GetType());
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

        ~StorableSingleton()
        {
            Save(false);
            Dispose(false);
        }

        private string _filePath => "Data/" + GetType().Name + ".json";
        private string _serialized => JsonConvert.SerializeObject(this);
        private int _lastLenght = 0;
        private Timer _autoSaveTimer;
        protected bool _disposed = false;


    }
}
