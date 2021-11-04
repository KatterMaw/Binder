using Binder.Environment;
using Binder.UI;
using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Binder.Classes.Data
{
    internal class Profile : StorableObject
    {
        private const int ImageSize = 30;

        public static Profile New
        {
            get
            {
                return new Profile("", new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, (byte)Utils.Random.Next(50, 150), (byte)Utils.Random.Next(50, 150), (byte)Utils.Random.Next(50, 150))), Guid.NewGuid(), new ObservableCollection<Bind> { }, new ObservableCollection<Abbreviation> { });
            }
        }
        public static Profile NewTemp
        {
            get
            {
                return new Profile("", new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, (byte)Utils.Random.Next(50, 150), (byte)Utils.Random.Next(50, 150), (byte)Utils.Random.Next(50, 150))), Guid.NewGuid(), new ObservableCollection<Bind> { }, new ObservableCollection<Abbreviation> { }, false);
            }
        }
        private string _name;
        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged();

                if (value == null || value.Length == 0) FirstCharInName = ' ';
                else FirstCharInName = value.ElementAt(0);
            }
        }

        [JsonIgnore]
        private char _firstCharInName;
        public char FirstCharInName
        {
            get => _firstCharInName;
            private set
            {
                _firstCharInName = value;
                OnPropertyChanged();
            }
        }

        public SolidColorBrush BackgroundBrush { get; init; }

        public ObservableCollection<Bind> Binds { get; init; }

        public ObservableCollection<Abbreviation> Abbreviations { get; init; }

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

        internal bool ImageFileIsExist => File.Exists(ImagePath);
        internal Visibility CustomImageVisibility => ImageFileIsExist ? Visibility.Visible : Visibility.Collapsed;
        internal Visibility DefaultImageVisibility => ImageFileIsExist ? Visibility.Collapsed : Visibility.Visible;


        internal void SetImage(string filePath)
        {
            if (!File.Exists(filePath)) throw new IOException($"File {filePath} don't exists");

            Bitmap bitmap = Utils.GetBitmapFromFile(filePath);

            bitmap = Utils.ResizeBitmap(bitmap, new System.Drawing.Size(ImageSize, ImageSize));

            Image = bitmap.ToBitmapImage();

            string destPath = string.Empty;

            destPath = $"Data/Resources/profile-{Id}.png";
            if (File.Exists(destPath)) File.Delete(destPath);
            bitmap.Save(destPath, ImageFormat.Png);

            RefreshImage();
        }

        internal void DeleteImage()
        {
            if (ImagePath != null) File.Delete(ImagePath);
            RefreshImage();
        }


        [JsonConstructor]
        private Profile(string name, SolidColorBrush backgroundBrush, Guid id, ObservableCollection<Bind> binds, ObservableCollection<Abbreviation> abbreviations, bool usingStorage = true) : base(id, usingStorage)
        {
            Name = name;
            BackgroundBrush = backgroundBrush;
            Binds = binds;
            Abbreviations = abbreviations;

            Initialize();
            LoadImageFromFile();
        }

        protected override void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    Binds.CollectionChanged -= Binds_CollectionChanged;
                    Abbreviations.CollectionChanged -= Abbreviations_CollectionChanged;
                }
                // освобождаем неуправляемые объекты

                base.Dispose(disposing);

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
        }

        private void Abbreviations_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(nameof(Abbreviations));
        }

        private void RefreshImage()
        {
            OnPropertyChanged(nameof(ImageFileIsExist));
            OnPropertyChanged(nameof(CustomImageVisibility));
            OnPropertyChanged(nameof(DefaultImageVisibility));
        }

        ~Profile()
        {
            Dispose(false);
        }



    }
}
