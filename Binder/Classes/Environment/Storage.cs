﻿using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using Binder;
using Binder.UI;
using Binder.Classes.Data;

namespace Binder.Environment
{
    internal enum ImageImportConverting
    {
        DontConvert,
        ConvertToPng,
        ConvertToJpg
    }

    /// <summary>
    /// Contains data between sessions
    /// </summary>
    class Storage : ViewModel
    {
        #region Instance
        [JsonIgnore]
        public static Storage Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = StorageFromFileOrDefault;
                    _instance.AddCollectionChangedHandlers();
                }
                return _instance;
            }
        }
        #endregion


        #region Storage fields

        private ObservableCollection<Profile> _profiles = new ObservableCollection<Profile> { };
        internal ObservableCollection<Profile> Profiles
        {
            get => _profiles;
            set
            {
                _profiles = value;
                OnPropertyChanged();
                RaiseSave();
            }
        }

        private ImageImportConverting _imageImportConverting = ImageImportConverting.DontConvert;
        internal ImageImportConverting ImageImportConverting
        {
            get => _imageImportConverting;
            set
            {
                _imageImportConverting = value;
                OnPropertyChanged();
                RaiseSave();
            }
        }

        private bool _convertAlreadyImportedImagesIfFound = false;
        public bool ConvertAlreadyImportedImagesIfFound
        {
            get => _convertAlreadyImportedImagesIfFound;
            set
            {
                _convertAlreadyImportedImagesIfFound = value;
                OnPropertyChanged();
                RaiseSave();
            }
        }

        private bool _resizeImagesWhenImporting = true;
        public bool ResizeImagesWhenImporting
        {
            get => _resizeImagesWhenImporting;
            set
            {
                _resizeImagesWhenImporting = value;
                OnPropertyChanged();
                RaiseSave();
            }
        }

        private void AddCollectionChangedHandlers()
        {
            Profiles.CollectionChanged += (o, e) =>
            {
                OnPropertyChanged(nameof(Profiles));
                RaiseSave();
            };
        }
        #endregion


        #region internal logic


        public static void RaiseSave()
        {
            _anyFieldWasChanged = true;
        }


        [JsonConstructor]
        private Storage()
        {

        }
        static Storage()
        {
            StartThread();
        }

        private static Storage _instance;
        private static readonly FileInfo _dataFile = new FileInfo("Data/data.json");
        private static Storage _default => new Storage();
        private static Storage StorageFromFileOrDefault
        {
            get
            {
                Storage resultStorage;
                if (TryGetStorageFromFile(out resultStorage)) return resultStorage;
                else return _default;
            }
        }

        private static bool _anyFieldWasChanged = false;
        private static string _serializedInstance => JsonConvert.SerializeObject(Instance);
        private static bool _keepWork;
        private static Thread _saveThread;

        
        private static void StartThread()
        {
            _keepWork = true;
            _saveThread = new Thread(SaveThread);
            Task.Run(() => // Чтобы не было конфликтов дескрипторов
            {
                Thread.Sleep(3000);
                _saveThread.Start();
            });
            App.Current.Dispatcher.BeginInvoke(() =>
            {
                App.Current.Exit += (o, e) =>
                {
                    _keepWork = false;
                    if (_anyFieldWasChanged) Save();
                };
            });
        }
        private static void SaveThread()
        {
            while (_keepWork)
            {
                if (_anyFieldWasChanged) Save();
                Thread.Sleep(1000);
            }
        }
        private static void Save()
        {
            using (StreamWriter stream = new StreamWriter(_dataFile.FullName, false))
            {
                stream.Write(_serializedInstance);
            }
            _anyFieldWasChanged = false;
        }
        private static bool TryGetStorageFromFile(out Storage resultStorage)
        {
            if (_dataFile.Exists)
            {
                resultStorage = JsonConvert.DeserializeObject<Storage>(File.ReadAllText(_dataFile.FullName));
                return resultStorage != null;
            }
            else
            {
                resultStorage = null;
                return false;
            }
        }
        
        private static void RecreateInstance()
        {
            _instance = _default;
        }
        #endregion
    }
}
