using Binder.Environment;
using Binder.UI;
using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows.Media.Imaging;

namespace Binder.Classes.Data
{
    internal class Profile : ViewModel, IDisposable
    {
        private const int ImageSize = 30;

        internal Profile New
        {
            get
            {
                return new Profile("", Color.FromArgb(Utils.Random.Next(50, 150), Utils.Random.Next(50, 150), Utils.Random.Next(50, 150)), Guid.NewGuid(), new ObservableCollection<Bind> { }, new ObservableCollection<Abbreviation> { });
            }
        }

        private string _name;
        internal string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged();
                Storage.RaiseSave();

                if (value.Length == 0) FirstCharInName = ' ';
                else FirstCharInName = value.ElementAt(0);
            }
        }

        [JsonIgnore]
        private char _firstCharInName;
        internal char FirstCharInName
        {
            get => _firstCharInName;
            private set
            {
                _firstCharInName = value;
                OnPropertyChanged();
            }
        }

        internal Color BackgroundColor { get; init; }

        internal Guid Id { get; init; }

        internal ObservableCollection<Bind> Binds { get; init; }

        internal ObservableCollection<Abbreviation> Abbreviations { get; init; }

        [JsonIgnore]
        internal string ImagePath
        {
            get
            {
                string expectedPath = $"Data/Resources/profile-{Id}.";
                if (File.Exists(expectedPath + ".png")) return expectedPath + ".png";
                else if (File.Exists(expectedPath + "jpg")) return expectedPath + "jpg";
                else return null;
            }
        }

        private BitmapImage _image;
        [JsonIgnore]
        internal BitmapImage Image
        {
            get => _image;
            private set
            {
                _image = value;
                OnPropertyChanged();
            }
        }

        

        public void SetImage(string filePath)
        {
            if (!File.Exists(filePath)) throw new IOException($"File {filePath} don't exists");

            if (!Storage.Instance.ResizeImagesWhenImporting && Storage.Instance.ImageImportConverting == ImageImportConverting.DontConvert)
            {
                File.Copy(filePath, $"Data/Resources/profile-{Id}.{Path.GetExtension(filePath)}", true);
                return;
            }

            Bitmap bitmap = Utils.GetBitmapFromFile(filePath);

            if (Storage.Instance.ResizeImagesWhenImporting)
            {
                bitmap = Utils.ResizeBitmap(bitmap, new Size(ImageSize, ImageSize));
            }

            Image = bitmap.ToBitmapImage();

            string destPath = string.Empty;

            switch (Storage.Instance.ImageImportConverting)
            {
                case ImageImportConverting.DontConvert:
                    destPath = $"Data/Resources/profile-{Id}.{Path.GetExtension(filePath)}";
                    if (File.Exists(destPath)) File.Delete(destPath);
                    bitmap.Save(destPath);
                    break;
                case ImageImportConverting.ConvertToPng:
                    destPath = $"Data/Resources/profile-{Id}.png";
                    if (File.Exists(destPath)) File.Delete(destPath);
                    bitmap.Save(destPath, ImageFormat.Png);
                    break;
                case ImageImportConverting.ConvertToJpg:
                    destPath = $"Data/Resources/profile-{Id}.jpg";
                    if (File.Exists(destPath)) File.Delete(destPath);
                    bitmap.Save(destPath, ImageFormat.Jpeg);
                    break;
            }

           
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }


        private bool _disposed = false;


        [JsonConstructor]
        private Profile(string name, Color backgroundColor, Guid id, ObservableCollection<Bind> binds, ObservableCollection<Abbreviation> abbreviations)
        {
            Name = name;
            BackgroundColor = backgroundColor;
            Id = id;
            Binds = binds;
            Abbreviations = abbreviations;

            Initialize();
            LoadImageFromFile();

            
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    Binds.CollectionChanged -= Binds_CollectionChanged;
                    Abbreviations.CollectionChanged -= Abbreviations_CollectionChanged;
                }
                // освобождаем неуправляемые объекты
                _disposed = true;
            }
        }

        private void LoadImageFromFile()
        {
            if (ImagePath == null) Image = null;
            else Image = Utils.GetBitmapFromFile(ImagePath).ToBitmapImage();
        }

        private void Initialize()
        {
            Binds.CollectionChanged += Binds_CollectionChanged;
            Abbreviations.CollectionChanged += Abbreviations_CollectionChanged;
        }

        private void Binds_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(nameof(Binds));
            Storage.RaiseSave();
        }

        private void Abbreviations_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(nameof(Abbreviations));
            Storage.RaiseSave();
        }

        ~Profile()
        {
            Dispose(false);
        }



    }
}
