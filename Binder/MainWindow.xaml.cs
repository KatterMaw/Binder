using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WindowsInput;

namespace Binder
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            InputSimulator inputSimulator = new InputSimulator();
            WinHotKey winHotKey = new WinHotKey(Key.NumPad1, KeyModifier.None, (h) =>
            {
                inputSimulator.Keyboard.KeyPress((WindowsInput.Native.VirtualKeyCode)Keys.T);
                inputSimulator.Keyboard.TextEntry("Привет!");
                inputSimulator.Keyboard.KeyPress((WindowsInput.Native.VirtualKeyCode)Keys.Enter);
            }, true);
        }
    }
}