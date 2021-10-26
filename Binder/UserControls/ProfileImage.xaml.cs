using Binder.Classes.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Binder.UserControls
{
    /// <summary>
    /// Представляет картинку в круглой форме, или, если картинки нет, цветной задник с первой буквой названия профиля
    /// </summary>
    public partial class ProfileImage : UserControl
    {
        internal static readonly DependencyProperty ImageProperty;

        static ProfileImage()
        {
            ImageProperty = DependencyProperty.Register("Profile", typeof(BitmapImage), typeof(ProfileImage), new FrameworkPropertyMetadata(OnImageChanged));
        }


        internal BitmapImage Image
        {
            get => (BitmapImage)GetValue(ImageProperty);
            set
            {
                SetValue(ImageProperty, value);
                //Img.Source = value;
            }
        }

        internal ProfileImage()
        {
            InitializeComponent();
        }

        private static void OnImageChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            
        }
    }
}
