using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;

namespace Binder.IO
{
    static class MouseAndKeyboardInput
    {
        public static bool IsKeyPressed(Keys key)
        {
            return GetAsyncKeyState(key) != 0;
        }

        public static bool IsKeyToggled(Keys key)
        {
            return GetAsyncKeyState(key) == -32767;
        }

        

        [DllImport("User32.dll")]
        private static extern short GetAsyncKeyState(Keys vKey);

        [DllImport("user32.dll")]
        static extern void mouse_event(int dwFlags, int dx, int dy, int dwData, int dwExtraInfo);
        public static void MouseMove(int xDelta, int yDelta)
        {
            mouse_event(0x0001, xDelta, yDelta, 0, 0);
        }
    }
}
