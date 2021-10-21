using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Binder
{
    /// <summary>
    /// A hotkey. Allows you to easily track the status of the keys on the keyboard or mouse.
    /// </summary>
    class HotKey
    {
        private const int SleepTime = 1;

        public bool IsPressed { get; private set; }
        public Action ActionOnPress { get; set; }

        public HotKey(Keys key, Action actionOnPress = null)
        {
            _key = key;
            ActionOnPress = actionOnPress;
            _allCreatedHotKeys.Add(this);
        }

        static HotKey()
        {
            /*App.Current.Dispatcher.Invoke(() => 
            {
                App.Current.Exit += (o, e) => _keepWork = false;
            });*/
            Task.Factory.StartNew(() =>
            {
                App.Current.Dispatcher.Invoke(() => App.Current.Exit += (o, e) => _keepWork = false);
            });
            _mainThread = new Thread(new ThreadStart(MainThread));
            _mainThread.Start();
        }

        public void Deactivate()
        {
            _allCreatedHotKeys.Remove(this);
        }

        private static void MainThread()
        {
            Thread.Sleep(20);
            while (_keepWork)
            {
                try
                {
                    foreach (HotKey hotKey in _allCreatedHotKeys)
                    {
                        bool isPressed = MouseAndKeyboardInput.IsKeyPressed(hotKey._key);
                        if (hotKey.ActionOnPress != null && !hotKey.IsPressed && isPressed) Task.Factory.StartNew(() => hotKey.ActionOnPress.Invoke());
                        hotKey.IsPressed = isPressed;
                    }
                }
                catch
                {
                    continue;
                }
                Thread.Sleep(1);
            }
        }

        private Keys _key;

        private static bool _keepWork = true;
        private static Thread _mainThread;
        private static List<HotKey> _allCreatedHotKeys = new List<HotKey> { };
    }
}
