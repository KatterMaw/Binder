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
    internal partial class ProfileImage : UserControl
    {
        internal static readonly DependencyProperty ProfileProperty;

        static ProfileImage()
        {
            ProfileProperty = DependencyProperty.Register("Profile", typeof(Profile), typeof(ProfileImage), new FrameworkPropertyMetadata(OnProfileChanged));
        }


        internal Profile Profile
        {
            get => (Profile)GetValue(ProfileProperty);
            set => SetValue(ProfileProperty, value);
        }

        internal ProfileImage()
        {
            InitializeComponent();

            ((ProfileImageVM)DataContext).Initialize(this);
        }

        private static void OnProfileChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            
        }
    }
}
