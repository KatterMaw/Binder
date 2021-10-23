using Binder.Pages;
using Binder.UI;
using MahApps.Metro.IconPacks;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace Binder.Windows
{
    class MainWindowVM : ViewModel
    {
        #region WindowResizeAndMovingHelper

        public CornerRadius UniformCornerRadius
        {
            get
            {
                if (_parentWindow == null) return new CornerRadius(5);
                else if (_parentWindow.WindowState == WindowState.Maximized) return new CornerRadius(0);
                else if (_parentWindow.Top == 0 && _parentWindow.ActualHeight == Utils.ScreenResolutionWithoutTaskbar.Height) return new CornerRadius(0);
                else if (_parentWindow.ActualHeight == Utils.ScreenResolutionWithoutTaskbar.Height / 2 && _parentWindow.ActualWidth == Utils.ScreenResolutionWithoutTaskbar.Width / 2) return new CornerRadius(0);
                else return new CornerRadius(5);
            }
        }
        public CornerRadius TopRightCornerRadius
        {
            get
            {
                if (_parentWindow == null) return new CornerRadius(5, 0, 0, 0);
                else if (_parentWindow.WindowState == WindowState.Maximized) return new CornerRadius(0);
                else if (_parentWindow.Top == 0 && _parentWindow.ActualHeight == Utils.ScreenResolutionWithoutTaskbar.Height) return new CornerRadius(0);
                else if (_parentWindow.ActualHeight == Utils.ScreenResolutionWithoutTaskbar.Height / 2 && _parentWindow.ActualWidth == Utils.ScreenResolutionWithoutTaskbar.Width / 2) return new CornerRadius(0);
                else return new CornerRadius(5, 0, 0, 0);
            }
        }
        public CornerRadius TopCornerRadius
        {
            get
            {
                if (_parentWindow == null) return new CornerRadius(5, 5, 0, 0);
                else if (_parentWindow.WindowState == WindowState.Maximized) return new CornerRadius(0);
                else if (_parentWindow.Top == 0 && _parentWindow.ActualHeight == Utils.ScreenResolutionWithoutTaskbar.Height) return new CornerRadius(0);
                else if (_parentWindow.ActualHeight == Utils.ScreenResolutionWithoutTaskbar.Height / 2 && _parentWindow.ActualWidth == Utils.ScreenResolutionWithoutTaskbar.Width / 2) return new CornerRadius(0);
                else return new CornerRadius(5, 5, 0, 0);
            }
        }
        public CornerRadius BottomCornerRadius
        {
            get
            {
                if (_parentWindow == null) return new CornerRadius(0, 0, 5, 5);
                else if (_parentWindow.WindowState == WindowState.Maximized) return new CornerRadius(0);
                else if (_parentWindow.Top == 0 && _parentWindow.ActualHeight == Utils.ScreenResolutionWithoutTaskbar.Height) return new CornerRadius(0);
                else if (_parentWindow.ActualHeight == Utils.ScreenResolutionWithoutTaskbar.Height / 2 && _parentWindow.ActualWidth == Utils.ScreenResolutionWithoutTaskbar.Width / 2) return new CornerRadius(0);
                else return new CornerRadius(0, 0, 5, 5);
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
                    _parentWindow.WindowState = WindowState.Maximized;
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

        public UI.Menu[] Menus { get; set; } = new UI.Menu[5]
            {
                new UI.Menu(
                    "Главная",
                    PackIconMaterialKind.Home,
                    PackIconMaterialKind.HomeOutline,
                    new HomePage()),

                new UI.Menu(
                    "Бинды",
                    PackIconMaterialKind.None,
                    PackIconMaterialKind.None,
                    new BindsPage()),

                new UI.Menu(
                    "Команды",
                    PackIconMaterialKind.None,
                    PackIconMaterialKind.None,
                    new CommandsPage()),

                new UI.Menu(
                    "Оверлей",
                    PackIconMaterialKind.None,
                    PackIconMaterialKind.None,
                    new OverlayPage()),

                new UI.Menu(
                    "Настройки",
                    PackIconMaterialKind.Cog,
                    PackIconMaterialKind.CogOutline,
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

        private RelayCommand _switchSidebarCommand;
        public RelayCommand SwitchSidebarCommand
        {
            get
            {
                return _switchSidebarCommand ??
                  (_switchSidebarCommand = new RelayCommand(obj =>
                  {
                      DoubleAnimation animation = new DoubleAnimation();
                      animation.From = _parentWindow.SidebarGrid.Width;
                      if (_sidebarIsOpened) animation.To = 65;
                      else animation.To = 250;
                      _sidebarIsOpened = !_sidebarIsOpened;
                      animation.Duration = new Duration(new TimeSpan(0, 0, 0, 0, 300));
                      animation.FillBehavior = FillBehavior.HoldEnd;
                      ExponentialEase ease = new ExponentialEase();
                      ease.EasingMode = EasingMode.EaseInOut;
                      ease.Exponent = 5;
                      animation.EasingFunction = ease;
                      _parentWindow.SidebarGrid.BeginAnimation(Grid.WidthProperty, animation);
                  }));
            }
        }

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
        }

        private MainWindow _parentWindow;
        private bool _sidebarIsOpened = false;


        private void UpdateWindowBorder()
        {
            OnPropertyChanged(nameof(UniformCornerRadius));
            OnPropertyChanged(nameof(TopRightCornerRadius));
            OnPropertyChanged(nameof(TopCornerRadius));
            OnPropertyChanged(nameof(BottomCornerRadius));
            OnPropertyChanged(nameof(ShadowThickness));
        }
    }
}
