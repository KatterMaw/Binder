using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Binder.Windows
{
    class MainWindowVM : ViewModel
    {
        public void Initalize(MainWindow parentWindow)
        {
            _parentWindow = parentWindow;
        }

        private MainWindow _parentWindow;
    }
}
