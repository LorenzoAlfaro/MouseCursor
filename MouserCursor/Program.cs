using System;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;

namespace MouserCursor
{
    class Program
    {
        static int x_left = 83;
        static int x_right = 157;
        static int y_up = 807;
        static int y_down = 752;
        static void Main(string[] args)
        {            
            IntPtr desktopWinHandle = Win32.GetDesktopWindow();
            int X=0;
            int Y=0;
            Win32.POINT p = new Win32.POINT(155, 755);
            int[] array = new int[4];            
            // Main loop
            while (true)
            {
                blockArmySelectAll(array);
                // System.Threading.Thread.Sleep(10);
                // rapidUnqueue(desktopWinHandle);                
            }
        }

        static void rapidUnqueue(IntPtr desktopWinHandle)
        {
            for (int KEY = 8; KEY <= 190; KEY++)
            {
                if (Win32.GetAsyncKeyState(KEY) == -32767)
                {
                    if (KEY != 0x10)
                    {
                        if (KEY == 66) // b
                        {
                            Unselect(desktopWinHandle);
                            // RightClick();                                                                
                        }
                        if (KEY == 78) // n
                        {
                            Unselect(desktopWinHandle);
                            RightClick();
                            // A_Press();                                
                            // LeftClick();                                                           
                        }
                    }
                }
            }
        }

        static void blockArmySelectAll(int[] array)
        {
            // Check for mouse position
            Point pos = Win32.GetCursorPosition();
            if (inside(pos.X, pos.Y, x_left, x_right, y_up, y_down))
            {
                array[3] = Math.Abs(pos.X - x_right);
                array[2] = Math.Abs(pos.X - x_left);
                array[1] = Math.Abs(pos.Y - y_up);
                array[0] = Math.Abs(pos.Y - y_down);

                var index = array.Select((v, i) => new { Index = i, Val = v }).FirstOrDefault(v => v.Val == array.Min()).Index;
                switch (index)
                {
                    case 3:
                        Win32.SetCursorPos(x_right, pos.Y);
                        break;
                    case 2:
                        Win32.SetCursorPos(x_left, pos.Y);
                        break;
                    case 1:
                        Win32.SetCursorPos(pos.X, y_up);
                        break;
                    case 0:
                        Win32.SetCursorPos(pos.X, y_down);
                        break;
                }
            }
        }

        static bool inside( int x, int y, int x_left, int x_right, int y_up, int y_down)
        {
            return x > x_left & x < x_right & y < y_up & y > y_down;
        }

        static int closestX(int x, int x_left, int x_right) {

            int delta1 = Math.Abs(x - x_left);
            int delta2 = Math.Abs(x - x_right);

            if( delta2 < delta1)
            {
                return x_right;
            }

            return x_left;
        }

        static int closestY(int y, int y_up, int y_down)
        {

            int delta1 = Math.Abs(y - y_up);
            int delta2 = Math.Abs(y - y_down);

            if (delta2 < delta1)
            {
                return y_down;
            }

            return y_up;
        }

        static void Unselect(IntPtr desktopWinHandle)
        {            
            Point pos = Win32.GetCursorPosition();
            //Console.WriteLine(pos.X.ToString() + " " + pos.Y.ToString());
            //Win32.POINT p = new Win32.POINT(689, 918); // SC2
            Win32.POINT p = new Win32.POINT(669, 918);
            
            Win32.ClientToScreen(desktopWinHandle, ref p);
            Win32.SetCursorPos(p.x, p.y);            
            // Key up
            //https://docs.microsoft.com/en-us/windows/win32/inputdev/virtual-key-codes
            Win32.keybd_event(0xA1, 0x36, Win32.KEYEVENTF_EXTENDEDKEY | 0, 0); // SHIFT DOWN use the right scan code!
            LeftClick();            
            Win32.keybd_event(0xA1, 0x36, Win32.KEYEVENTF_KEYUP | 0, 0); // SHIFT UP
            //System.Threading.Thread.Sleep(6); // SLEEP 10 FOR bW
            Win32.SetCursorPos(pos.X, pos.Y);  // Return Cursor to original position
                                   
            //System.Threading.Thread.Sleep(10);
            //Console.WriteLine(pos.X.ToString() + " " + pos.Y.ToString());
            //Point pos = Win32.GetCursorPosition();            
        }

        static void RightClick()
        {
            Win32.MouseEvent(Win32.MouseEventFlags.RightDown); // LEFT CLICK DOWN
            Win32.MouseEvent(Win32.MouseEventFlags.RightUp); // LEFT CLICK UP
        }

        static void LeftClick()
        {
            Win32.MouseEvent(Win32.MouseEventFlags.LeftDown); // LEFT CLICK DOWN

            Win32.MouseEvent(Win32.MouseEventFlags.LeftUp); // LEFT CLICK UP
        }

        static void A_Press()
        {
            Win32.keybd_event(0x41, 0x36, Win32.KEYEVENTF_EXTENDEDKEY | 0, 0); // A DOWN use the right scan code!
            Win32.keybd_event(0x41, 0x36, Win32.KEYEVENTF_KEYUP | 0, 0); // A UP
        }
    }

    public class Win32
    {        
        [DllImport("User32.dll")]
        public static extern short GetAsyncKeyState(int vKey);

        [DllImport("user32.dll")]
        public static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, uint dwExtraInfo);
        public const uint KEYEVENTF_KEYUP = 0x0002;
        public const uint KEYEVENTF_EXTENDEDKEY = 0x0001;
        
        [Flags]
        public enum MouseEventFlags
        {
            LeftDown = 0x00000002,
            LeftUp = 0x00000004,
            MiddleDown = 0x00000020,
            MiddleUp = 0x00000040,
            Move = 0x00000001,
            Absolute = 0x00008000,
            RightDown = 0x00000008,
            RightUp = 0x00000010
        }

        [DllImport("User32.Dll")]
        public static extern long SetCursorPos(int x, int y);

       
        [DllImport("user32.dll", SetLastError = false)] public static extern IntPtr GetDesktopWindow();

        [DllImport("user32.dll")]
        private static extern void mouse_event(int dwFlags, int dx, int dy, int dwData, int dwExtraInfo);

        public static void MouseEvent(MouseEventFlags value)
        {
            Point position = GetCursorPosition();

            mouse_event
                ((int)value,
                 position.X,
                 position.Y,
                 0,
                 0)
                ;
        }

        [DllImport("User32.Dll")]
        public static extern bool ClientToScreen(IntPtr hWnd, ref POINT point);

        //[StructLayout(LayoutKind.Sequential)]
        //public struct POINT
        //{
        //    public int x;
        //    public int y;

        //    public POINT(int X, int Y)
        //    {
        //        x = X;
        //        y = Y;
        //    }
        //}

        /// <summary>
        /// Struct representing a point.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int x;
            public int y;

            public POINT(int X, int Y)
            {
                x = X;
                y = Y;
            }

            public static implicit operator Point(POINT point)
            {
                return new Point(point.x, point.y);
            }
        }

        /// <summary>
        /// Retrieves the cursor's position, in screen coordinates.
        /// </summary>
        /// <see>See MSDN documentation for further information.</see>
        [DllImport("user32.dll")]
        public static extern bool GetCursorPos(out POINT lpPoint);

        public static Point GetCursorPosition()
        {
            POINT lpPoint;
            GetCursorPos(out lpPoint);
            // NOTE: If you need error handling
            // bool success = GetCursorPos(out lpPoint);
            // if (!success)

            return lpPoint;
        }
    }
}
