using Binder.Classes.Data;
using Binder.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Binder.UserControls
{
    class ProfileImageVM : ViewModel
    {
        internal ProfileImageVM() { }

        internal void Initialize(ProfileImage parentControl)
        {
            _parentControl = parentControl;
        }
        internal void SetProfile(Profile profile)
        {
            _profile = profile;
        }

        private UserControl _parentControl;
        private Profile _profile;
    }
}
