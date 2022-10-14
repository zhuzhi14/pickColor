using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace WpfApplication18
{
    public class MouseHook
    {
        public delegate void MouseHookCallback(int nCode, IntPtr wParam, IntPtr lParam);

        // public delegate void KeyHookCallback( int nCode, IntPtr wParam, IntPtr lParam);

        //wh_mouse_ll 鼠标钩子 
        public const int WhMouseLl = 14;
        public const int WmLbuttondown = 0x0201;
        public const int WmRbuttondown = 0x0204;
        public const int WmMbuttondown = 0x0207;
        public const int WM_MOUSEMOVE = 0x0200;
        public const int WM_MOUSEWHEEL = 0x020A;
        public const int WM_LBUTTONUP = 0x0202;
        public const int WM_RBUTTONUP = 0x0205;
        public const int WM_MBUTTONUP = 0x0208;
        public const int WM_LBUTTONDBLCLK = 0x0203;
        public const int WM_RBUTTONDBLCLK = 0x0206;
        public const int WM_MBUTTONDBLCLK = 0x0209;
        public const int WM_MOUSEHWHEEL = 0x020E;
        public const int WM_MOUSELEAVE = 0x02A3;
        public const int WM_NCMOUSEMOVE = 0x00A0;
        public const int WM_NCLBUTTONDOWN = 0x00A1;
        public const int WM_NCLBUTTONUP = 0x00A2;
        public const int WM_NCLBUTTONDBLCLK = 0x00A3;
        public const int WM_NCRBUTTONDOWN = 0x00A4;
        public const int WM_NCRBUTTONUP = 0x00A5;
        public const int WM_NCRBUTTONDBLCLK = 0x00A6;
        public const int WM_NCMBUTTONDOWN = 0x00A7;
        public const int WM_NCMBUTTONUP = 0x00A8;      
        public const int WM_NCMBUTTONDBLCLK = 0x00A9;
        public const int WM_NCXBUTTONDOWN = 0x00AB;
        public const int WM_NCXBUTTONUP = 0x00AC;
        public const int WM_NCXBUTTONDBLCLK = 0x00AD;
        public const int WM_NCMOUSEHOVER = 0x02A0;
        public const int WM_NCMOUSELEAVE = 0x02A2;
        public const int WM_MOUSEHOVER = 0x02A;
        //键盘钩子
        public const int WH_KEYBOARD_LL = 13;
        public const int WM_KEYDOWN = 0x0100;
        public const int WM_KEYUP = 0x0101;
        public const int WM_SYSKEYDOWN = 0x0104;
        public const int WM_SYSKEYUP = 0x0105;
        
        
        ~MouseHook()
        {
            Stop();
        }
        
        
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        //设置windows 钩子
        public static extern int SetWindowsHookEx(int idHook,MouseHookCallback lpfn, IntPtr hInstance, int threadId);
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        //卸载windows 钩子
        public static extern bool UnhookWindowsHookEx(int idHook);
        
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern int CallNextHookEx(int idHook, int nCode, IntPtr wParam, IntPtr lParam);
        //获取当前线程ID
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr GetModuleHandle(string lpModuleName);
        
        //GetDC函数为指定窗口的客户区或整个屏幕检索设备上下文 (DC) 的句柄。
        [DllImport("user32.dll")]
        static extern IntPtr GetDC(IntPtr hWnd);
        [DllImport("user32.dll")]
        //释放设备上下文 (DC)。
        static extern int ReleaseDC(IntPtr hWnd, IntPtr hDc);
        [DllImport("gdi32.dll")]
        static extern int GetPixel(IntPtr hDc, int x, int y);
        
        
        
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern bool GetCursorPos(out Point pt);
        
        public static int HMouseHook = 0;
        public static MouseHookCallback MouseHookProcedure;
        public static  MouseHookCallback KeyHookProcedure2;
        
        //祖册事件
        public static event MouseHookCallback OnMouseActivity;
        
        public static event MouseHookCallback OnKeyActivity;
        
        public static void Start()
        {
            var processModule = Process.GetCurrentProcess().MainModule;
            if (HMouseHook != 0)
            {
                return;
            }
            MouseHookProcedure = MouseHookProc;
            KeyHookProcedure2 = KeyHookProc;
            
                
            if (processModule != null)
            {
                HMouseHook = SetWindowsHookEx(WhMouseLl, MouseHookProcedure, GetModuleHandle(processModule.ModuleName), 0);
                HKeyhook = SetWindowsHookEx(WH_KEYBOARD_LL, KeyHookProcedure2, GetModuleHandle(processModule.ModuleName), 0);
                
            }
        }

        private static void KeyHookProc(int ncode, IntPtr wparam, IntPtr lparam)
        {
            
        
            if (ncode >= 0)
            {
                if (OnKeyActivity != null&&wparam==(IntPtr)WM_KEYDOWN)
                {
                    OnKeyActivity(ncode, wparam, lparam);
                }
               
            }
            CallNextHookEx(HMouseHook, ncode, wparam, lparam);
        }

     

        public static int HKeyhook { get; set; }

        public static void Stop()
        {
            bool retMouse = true;
            if (HMouseHook != 0)
            {
                retMouse = UnhookWindowsHookEx(HMouseHook);
                UnhookWindowsHookEx(HKeyhook);
                HMouseHook = 0;
            }
            if (!(retMouse)) throw new Exception("UnhookWindowsHookEx failed.");
        }

        private static void MouseHookProc(int ncode, IntPtr wparam, IntPtr lparam)
        {
            if (ncode >= 0)
            {
                if (OnMouseActivity != null)
                {
                  
                    OnMouseActivity(ncode, wparam, lparam);
                }
            }
            CallNextHookEx(HMouseHook, ncode, wparam, lparam);
        }
        
        public static (int r, int g, int b, long a) GetPointAndColor()
        {
            Point pt;
            GetCursorPos(out pt);
            var dc = GetDC(IntPtr.Zero);
            var color = GetPixel(dc, pt.X, pt.Y);
            ReleaseDC(IntPtr.Zero, dc);
            var r = (color & 0x000000FF);
            var g = (color & 0x0000FF00) >> 8;
            var b = (color & 0x00FF0000) >> 16;
            var a = (color & 0xFF000000) >> 24;
           
            return (r,g,b,a);
        }




    }
}