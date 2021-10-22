using Binder.Stuff;
using System;
using System.Collections.Generic;
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

        public void Initalize(MainWindow parentWindow)
        {
            _parentWindow = parentWindow;
            _windowResizer = new WindowResizer(parentWindow);

            parentWindow.StateChanged += (o, e) => UpdateWindowBorder();
            parentWindow.SizeChanged += (o, e) => UpdateWindowBorder();
            parentWindow.LocationChanged += (o, e) => UpdateWindowBorder();

            parentWindow.StateChanged += (o, e) =>
            {
                OnPropertyChanged(nameof(ResizeBorderThickness));
            };
        }

        private MainWindow _parentWindow;


        private void UpdateWindowBorder()
        {
            OnPropertyChanged(nameof(UniformCornerRadius));
            OnPropertyChanged(nameof(TopRightCornerRadius));
            OnPropertyChanged(nameof(TopCornerRadius));
            OnPropertyChanged(nameof(ShadowThickness));
        }
    }
}
