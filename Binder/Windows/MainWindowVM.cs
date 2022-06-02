using Binder.Classes.Data;
using Binder.Environment;
using Binder.Pages;
using Binder.UI;
using MahApps.Metro.IconPacks;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;

namespace Binder.Windows
{
    class MainWindowVM : ViewModel
    {
        #region WindowResizeAndMovingHelper

        private const int CornerRadius = 10;

        public CornerRadius UniformCornerRadius
        {
            get
            {
                if (_parentWindow == null) return new CornerRadius(CornerRadius);
                else if (_parentWindow.WindowState == WindowState.Maximized) return new CornerRadius(0);
                else if (_parentWindow.Top == 0 && _parentWindow.ActualHeight == Utils.ScreenResolutionWithoutTaskbar.Height) return new CornerRadius(0);
                else if (_parentWindow.ActualHeight == Utils.ScreenResolutionWithoutTaskbar.Height / 2 && _parentWindow.ActualWidth == Utils.ScreenResolutionWithoutTaskbar.Width / 2) return new CornerRadius(0);
                else return new CornerRadius(CornerRadius);
            }
        }
        public CornerRadius TopRightCornerRadius
        {
            get
            {
                if (_parentWindow == null) return new CornerRadius(CornerRadius, 0, 0, 0);
                else if (_parentWindow.WindowState == WindowState.Maximized) return new CornerRadius(0);
                else if (_parentWindow.Top == 0 && _parentWindow.ActualHeight == Utils.ScreenResolutionWithoutTaskbar.Height) return new CornerRadius(0);
                else if (_parentWindow.ActualHeight == Utils.ScreenResolutionWithoutTaskbar.Height / 2 && _parentWindow.ActualWidth == Utils.ScreenResolutionWithoutTaskbar.Width / 2) return new CornerRadius(0);
                else return new CornerRadius(CornerRadius, 0, 0, 0);
            }
        }
        public CornerRadius TopCornerRadius
        {
            get
            {
                if (_parentWindow == null) return new CornerRadius(CornerRadius, CornerRadius, 0, 0);
                else if (_parentWindow.WindowState == WindowState.Maximized) return new CornerRadius(0);
                else if (_parentWindow.Top == 0 && _parentWindow.ActualHeight == Utils.ScreenResolutionWithoutTaskbar.Height) return new CornerRadius(0);
                else if (_parentWindow.ActualHeight == Utils.ScreenResolutionWithoutTaskbar.Height / 2 && _parentWindow.ActualWidth == Utils.ScreenResolutionWithoutTaskbar.Width / 2) return new CornerRadius(0);
                else return new CornerRadius(CornerRadius, CornerRadius, 0, 0);
            }
        }
        public CornerRadius BottomCornerRadius
        {
            get
            {
                if (_parentWindow == null) return new CornerRadius(0, 0, CornerRadius, CornerRadius);
                else if (_parentWindow.WindowState == WindowState.Maximized) return new CornerRadius(0);
                else if (_parentWindow.Top == 0 && _parentWindow.ActualHeight == Utils.ScreenResolutionWithoutTaskbar.Height) return new CornerRadius(0);
                else if (_parentWindow.ActualHeight == Utils.ScreenResolutionWithoutTaskbar.Height / 2 && _parentWindow.ActualWidth == Utils.ScreenResolutionWithoutTaskbar.Width / 2) return new CornerRadius(0);
                else return new CornerRadius(0, 0, CornerRadius, CornerRadius);
            }
        }
        public CornerRadius BottomLeftCornerRadius
        {
            get
            {
                if (_parentWindow == null) return new CornerRadius(0, 0, 0, CornerRadius);
                else if (_parentWindow.WindowState == WindowState.Maximized) return new CornerRadius(0);
                else if (_parentWindow.Top == 0 && _parentWindow.ActualHeight == Utils.ScreenResolutionWithoutTaskbar.Height) return new CornerRadius(0);
                else if (_parentWindow.ActualHeight == Utils.ScreenResolutionWithoutTaskbar.Height / 2 && _parentWindow.ActualWidth == Utils.ScreenResolutionWithoutTaskbar.Width / 2) return new CornerRadius(0);
                else return new CornerRadius(0, 0, 0, CornerRadius);
            }
        }
        public Thickness ShadowThickness
        {
            get
            {
                if (_parentWindow == null) return new Thickness(5);
                else if (_parentWindow.WindowState == WindowState.Maximized) return new Thickness(0);
                else if (_parentWindow.Top == 0 && _parentWindow.ActualHeight == Utils.ScreenResolutionWithoutTaskbar.Height)
                {
                    if (_parentWindow.Left == 0 && _parentWindow.ActualWidth == Utils.ScreenResolutionWithoutTaskbar.Width / 2) return new Thickness(0, 0, 5, 0);
                    else if (_parentWindow.Left == Utils.ScreenResolutionWithoutTaskbar.Width / 2 && _parentWindow.ActualWidth == Utils.ScreenResolutionWithoutTaskbar.Width / 2) return new Thickness(5, 0, 0, 0);
                    else return new Thickness(5, 0, 5, 0);
                }
                else if (_parentWindow.ActualHeight == Utils.ScreenResolutionWithoutTaskbar.Height / 2 && _parentWindow.ActualWidth == Utils.ScreenResolutionWithoutTaskbar.Width / 2) return new Thickness(0);
                else return new Thickness(5);
            }
        }
        public Thickness ResizeBorderThickness // Баг фикс. KatterMaw
        {
            get
            {
                if (_parentWindow == null) return NormalResizeBorderThickness;
                else if (_parentWindow.WindowState == WindowState.Maximized) return NoResizeBorderThickness;
                else return NormalResizeBorderThickness;
            }
        }

        private WindowResizer _windowResizer;

        private Thickness NormalResizeBorderThickness = new Thickness(10);
        private Thickness NoResizeBorderThickness = new Thickness(0);

        #endregion

        #region NonClientReplacer

        private RelayCommand _closeWindowCommand;
        public RelayCommand CloseWindowCommand
        {
            get
            {
                return _closeWindowCommand ?? (_closeWindowCommand = new RelayCommand(obj =>
                {
                    App.Current.Shutdown();
                }));
            }
        }
        private RelayCommand _maximizeWindowCommand;
        public RelayCommand MaximizeWindowCommand
        {
            get
            {
                return _maximizeWindowCommand ?? (_maximizeWindowCommand = new RelayCommand(obj =>
                {
                    if (_parentWindow.WindowState == WindowState.Maximized) _parentWindow.WindowState = WindowState.Normal;
                    else _parentWindow.WindowState = WindowState.Maximized;
                }));
            }
        }
        private RelayCommand _minimizeWindowCommand;
        public RelayCommand MinimizeWindowCommand
        {
            get
            {
                return _minimizeWindowCommand ?? (_minimizeWindowCommand = new RelayCommand(obj =>
                {
                    _parentWindow.WindowState = WindowState.Minimized;
                }));
            }
        }

        #endregion

        #region sideBarMenu

        public UI.Menu[] Menus { get; set; } = new UI.Menu[5]
            {
                new UI.Menu(
                    "Главная",
                    PackIconMaterialKind.Home,
                    PackIconMaterialKind.HomeOutline,
                    PackIconBootstrapIconsKind.HouseFill,
                    PackIconBootstrapIconsKind.House,
                    new HomePage()),

                new UI.Menu(
                    "Бинды",
                    PackIconMaterialKind.None,
                    PackIconMaterialKind.None,
                    PackIconBootstrapIconsKind.BookmarksFill,
                    PackIconBootstrapIconsKind.Bookmarks,
                    new BindsPage()),

                new UI.Menu(
                    "Команды",
                    PackIconMaterialKind.None,
                    PackIconMaterialKind.None,
                    PackIconBootstrapIconsKind.TerminalFill,
                    PackIconBootstrapIconsKind.Terminal,
                    new CommandsPage()),

                new UI.Menu(
                    "Оверлей",
                    PackIconMaterialKind.None,
                    PackIconMaterialKind.None,
                    PackIconBootstrapIconsKind.CollectionFill,
                    PackIconBootstrapIconsKind.Collection,
                    new OverlayPage()),

                new UI.Menu(
                    "Настройки",
                    PackIconMaterialKind.Cog,
                    PackIconMaterialKind.CogOutline,
                    PackIconBootstrapIconsKind.GearFill,
                    PackIconBootstrapIconsKind.Gear,
                    new SettingsPage())
            };
        public UI.Menu SelectedMenu
        {
            get => _selectedMenu;
            set
            {
                _selectedMenu = value;
                foreach (UI.Menu menu in Menus)
                {
                    menu.IsSelected = menu == value;
                }
                OnPropertyChanged();
                OnPropertyChanged(nameof(Menus));
            }
        }
        private UI.Menu _selectedMenu;

        public Visibility ProfileListControlsVisibility => _sidebarIsOpened ? Visibility.Visible : Visibility.Collapsed;

        private RelayCommand _switchSidebarCommand;
        public RelayCommand SwitchSidebarCommand
        {
            get
            {
                return _switchSidebarCommand ??
                  (_switchSidebarCommand = new RelayCommand(obj =>
                  {
                      _sidebarIsOpened = !_sidebarIsOpened;

                      ExponentialEase ease = new ExponentialEase();
                      ease.EasingMode = EasingMode.EaseInOut;
                      ease.Exponent = 5;

                      #region Sidebar
                      DoubleAnimation sideBarSlidingAnimation = new DoubleAnimation();
                      sideBarSlidingAnimation.From = _parentWindow.SidebarGrid.Width;
                      sideBarSlidingAnimation.To = _sidebarIsOpened ? 250 : 65;
                      sideBarSlidingAnimation.Duration = new Duration(new TimeSpan(0, 0, 0, 0, 300));
                      sideBarSlidingAnimation.FillBehavior = FillBehavior.HoldEnd;
                      sideBarSlidingAnimation.EasingFunction = ease;
                      _parentWindow.SidebarGrid.BeginAnimation(Grid.WidthProperty, sideBarSlidingAnimation);
                      #endregion

                      #region ProfileListControlsGrid
                      if (_sidebarIsOpened)
                          Task.Run(() =>
                          {
                              App.Current.Dispatcher.Invoke(() => _parentWindow.ProfileListControlGrid.Visibility = _sidebarIsOpened ? Visibility.Visible : Visibility.Hidden);
                          });
                      Task.Run(() =>
                          {
                              Thread.Sleep(50);
                              App.Current.Dispatcher.Invoke(() =>
                              {
                                  DoubleAnimation profilesControlOpacityAnimation = new DoubleAnimation();
                                  profilesControlOpacityAnimation.From = _parentWindow.ProfileListControlGrid.Opacity;
                                  profilesControlOpacityAnimation.To = _sidebarIsOpened ? 1 : 0;
                                  profilesControlOpacityAnimation.Duration = new Duration(new TimeSpan(0, 0, 0, 0, 200));
                                  profilesControlOpacityAnimation.FillBehavior = FillBehavior.HoldEnd;
                                  profilesControlOpacityAnimation.EasingFunction = ease;
                                  _parentWindow.ProfileListControlGrid.BeginAnimation(Grid.OpacityProperty, profilesControlOpacityAnimation);
                              });
                          });
                      if (!_sidebarIsOpened)
                      Task.Run(() =>
                      {
                          Thread.Sleep(200);
                          App.Current.Dispatcher.Invoke(() => _parentWindow.ProfileListControlGrid.Visibility = _sidebarIsOpened ? Visibility.Visible : Visibility.Hidden);
                      });
                      #endregion

                  }));
            }
        }

        #endregion

        #region Profiles

        public bool ReplaceComboBoxWithAList => _parentWindow.ActualHeight > 690;
        public Visibility ComboBoxVisibility => ReplaceComboBoxWithAList ? Visibility.Collapsed : Visibility.Visible;
        public Visibility ListVisibility => ReplaceComboBoxWithAList ? Visibility.Visible : Visibility.Collapsed;

        #endregion



        #region Add/EditProfilePopup

        private string _addOrEditProfilePopup_Operation;
        internal string AddOrEditProfilePopup_Operation
        {
            get => _addOrEditProfilePopup_Operation;
            set
            {
                _addOrEditProfilePopup_Operation = value;
                OnPropertyChanged();
            }
        }

        private string _addOrEditProfilePopup_NewName;
        public string AddOrEditProfilePopup_NewName
        {
            get => _addOrEditProfilePopup_NewName;
            set
            {
                _addOrEditProfilePopup_NewName = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(AddOrEditProfilePopup_NewNameFirstChar));
                if (string.IsNullOrEmpty(value))
                {
                    AddOrEditProfilePopup_NewNameValidationPassed = false;
                    throw new Exception("Это поле обязательно!");
                }
                else if (string.IsNullOrWhiteSpace(value))
                {
                    AddOrEditProfilePopup_NewNameValidationPassed = false;
                    throw new Exception("Название не может быть пробелом!");
                }
                else AddOrEditProfilePopup_NewNameValidationPassed = true;
            }
        }

        private bool _addOrEditProfilePopup_NewNameValidationPassed = false;
        private bool AddOrEditProfilePopup_NewNameValidationPassed
        {
            get => _addOrEditProfilePopup_NewNameValidationPassed;
            set
            {
                _addOrEditProfilePopup_NewNameValidationPassed = value;
                OnPropertyChanged(nameof(AddOrEditProfilePopup_CanApply));
            }
        }

        public char AddOrEditProfilePopup_NewNameFirstChar
        {
            get
            {
                if (string.IsNullOrWhiteSpace(AddOrEditProfilePopup_NewName)) return ' ';
                return AddOrEditProfilePopup_NewName.ToCharArray()[0];
            }
        }
        private string _addOrEditProfilePopup_OldName;
        public string AddOrEditProfilePopup_OldName
        {
            get => _addOrEditProfilePopup_OldName;
            set
            {
                _addOrEditProfilePopup_OldName = value;
                OnPropertyChanged();
            }
        }

        private string _addOrEditProfilePopup_NewImagePath;
        public string AddOrEditProfilePopup_NewImagePath
        {
            get => _addOrEditProfilePopup_NewImagePath;
            set
            {
                _addOrEditProfilePopup_NewImagePath = value;
                OnPropertyChanged();
            }
        }

        public BitmapImage AddOrEditProfilePopup_NewImagePreview
        {
            get
            {
                if (string.IsNullOrWhiteSpace(AddOrEditProfilePopup_NewImagePath)) return null;
                return Utils.GetBitmapFromFile(AddOrEditProfilePopup_NewImagePath).ToBitmapImage();
            }
        }
        public bool AddOrEditProfilePopup_CanApply => _addOrEditProfilePopup_NewNameValidationPassed;


        private RelayCommand _addOrEditProfilePopup_ShowToEditCommand;
        public RelayCommand AddOrEditProfilePopup_ShowToEditCommand
        {
            get
            {
                return _addOrEditProfilePopup_ShowToEditCommand ??
                  (_addOrEditProfilePopup_ShowToEditCommand = new RelayCommand(obj =>
                  {
                      AddOrEditProfilePopup_Operation = "Изменение профиля \"" + AddOrEditProfilePopup_OldName + "\"";

                      _parentWindow.MainContentView.Effect = _pupupBackgroundBlur;
                      _parentWindow.EditProfilePopup.Visibility = Visibility.Visible;

                      SubscribeImageOnEvents();
                  }));
            }
        }

        private RelayCommand _addOrEditProfilePopup_ShowToAddCommand;
        public RelayCommand AddOrEditProfilePopup_ShowToAddCommand
        {
            get
            {
                return _addOrEditProfilePopup_ShowToAddCommand ??
                  (_addOrEditProfilePopup_ShowToAddCommand = new RelayCommand(obj =>
                  {
                      AddOrEditProfilePopup_Operation = "Добавление нового профиля";

                      _parentWindow.MainContentView.Effect = _pupupBackgroundBlur;
                      _parentWindow.EditProfilePopup.Visibility = Visibility.Visible;

                      SubscribeImageOnEvents();
                  }));
            }
        }

        private RelayCommand _addOrEditProfilePopup_ApplyCommand;
        public RelayCommand AddOrEditProfilePopup_ApplyCommand
        {
            get
            {
                return _addOrEditProfilePopup_ApplyCommand ??
                  (_addOrEditProfilePopup_ApplyCommand = new RelayCommand(obj =>
                  {
                      _parentWindow.MainContentView.Effect = null;
                      _parentWindow.EditProfilePopup.Visibility = Visibility.Collapsed;
                      AddOrEditProfilePopup_Clear();

                      UnsubscribeImageOnEvents();
                  }));
            }
        }

        private RelayCommand _addOrEditProfilePopup_CancelCommand;
        public RelayCommand AddOrEditProfilePopup_CancelCommand
        {
            get
            {
                return _addOrEditProfilePopup_CancelCommand ??
                  (_addOrEditProfilePopup_CancelCommand = new RelayCommand(obj =>
                  {
                      _parentWindow.MainContentView.Effect = null;
                      _parentWindow.EditProfilePopup.Visibility = Visibility.Collapsed;
                      AddOrEditProfilePopup_Clear();

                      UnsubscribeImageOnEvents();
                  }));
            }
        }

        private RelayCommand _addOrEditProfilePopup_ShowFileDialogForImage;
        public RelayCommand AddOrEditProfilePopup_ShowFileDialogForImage
        {
            get
            {
                return _addOrEditProfilePopup_ShowFileDialogForImage ??
                  (_addOrEditProfilePopup_ShowFileDialogForImage = new RelayCommand(obj =>
                  {
                      using (OpenFileDialog openFileDialog = new OpenFileDialog())
                      {
                          openFileDialog.Filter = "Изображения(*.png;*.jpg)|*.png;*.jpg|Все файлы (*.*)|*.*";
                          openFileDialog.Multiselect = false;
                          if (openFileDialog.ShowDialog() == DialogResult.OK)
                          {
                              if (File.Exists(openFileDialog.FileName)) AddOrEditProfilePopup_NewImagePath = openFileDialog.FileName;
                          }
                      }
                  }));
            }
        }


        private void AddOrEditProfilePopup_Clear()
        {
            AddOrEditProfilePopup_NewName = null;
            AddOrEditProfilePopup_NewImagePath = null;
        }
        private void SubscribeImageOnEvents()
        {
            _parentWindow.AddOrEditProfilePopup_ImagePresenter.MouseEnter += AddOrEditProfilePopup_ImagePresenter_MouseEnter;
            _parentWindow.AddOrEditProfilePopup_ImagePresenter.MouseLeave += AddOrEditProfilePopup_ImagePresenter_MouseLeave;
        }

        private void UnsubscribeImageOnEvents()
        {
            _parentWindow.AddOrEditProfilePopup_ImagePresenter.MouseEnter -= AddOrEditProfilePopup_ImagePresenter_MouseEnter;
            _parentWindow.AddOrEditProfilePopup_ImagePresenter.MouseLeave -= AddOrEditProfilePopup_ImagePresenter_MouseLeave;
        }

        private void AddOrEditProfilePopup_ImagePresenter_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            throw new NotImplementedException();
        }
        private void AddOrEditProfilePopup_ImagePresenter_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            throw new NotImplementedException();
        }

        #endregion

        public void Initalize(MainWindow parentWindow)
        {
            _parentWindow = parentWindow;
            _windowResizer = new WindowResizer(parentWindow);

            SelectedMenu = Menus[0];

            parentWindow.StateChanged += (o, e) => UpdateWindowBorder();
            parentWindow.SizeChanged += (o, e) => UpdateWindowBorder();
            parentWindow.LocationChanged += (o, e) => UpdateWindowBorder();

            parentWindow.StateChanged += (o, e) =>
            {
                OnPropertyChanged(nameof(ResizeBorderThickness));
            };

            parentWindow.SizeChanged += (o, e) =>
            {
                OnPropertyChanged(nameof(ComboBoxVisibility));
                OnPropertyChanged(nameof(ListVisibility));
            };
            parentWindow.PreviewKeyDown += (o, e) =>
            {
                if (e.Key == System.Windows.Input.Key.Tab || e.Key == System.Windows.Input.Key.System)
                {
                    e.Handled = true;
                }
            };
        }

        private MainWindow _parentWindow;
        private bool _sidebarIsOpened = false;
        private BlurEffect _pupupBackgroundBlur = new BlurEffect() { Radius = 5 };


        private void UpdateWindowBorder()
        {
            OnPropertyChanged(nameof(UniformCornerRadius));
            OnPropertyChanged(nameof(TopRightCornerRadius));
            OnPropertyChanged(nameof(TopCornerRadius));
            OnPropertyChanged(nameof(BottomCornerRadius));
            OnPropertyChanged(nameof(BottomLeftCornerRadius));
            OnPropertyChanged(nameof(ShadowThickness));
        }
    }
}
