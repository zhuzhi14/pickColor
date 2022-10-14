using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;
using Application = System.Windows.Application;
using Color = System.Windows.Media.Color;

namespace WpfApplication18
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
     
        public MainWindow()
        {
            InitializeComponent();
           // GetPointAndColor();
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            MouseHook.Stop();
            MouseHook.OnMouseActivity -= MouseHook_OnMouseActivity;
            Application.Current.Shutdown();
            
        }

        //
       

        private void Button_OnClick(object sender, RoutedEventArgs e)
        {
            MouseHook.Start();
            MouseHook.OnMouseActivity += MouseHook_OnMouseActivity;
            MouseHook.OnKeyActivity += MouseHook_OnKeyActivity;
           
            // if (_isClicked == false)
            // {
            //  
            // }
            // else
            // {
            //     MouseHook.Stop();
            //     MouseHook.OnMouseActivity -= MouseHook_OnMouseActivity;
            //     MouseHook.OnKeyActivity -= MouseHook_OnKeyActivity;
            // }

        }
        //键盘事件
        private void MouseHook_OnKeyActivity(int ncode, IntPtr wparam, IntPtr lparam)
        {
           if(ncode>=0)
           {
               var key = (Keys)Marshal.ReadInt32(lparam);
               if (key == Keys.Space)
              
               {
                
                   TextBox4.Text += "空格";
                   

               }
                if (key == Keys.Enter)
                {
                     TextBox4.Text += "回车";
                }
                if (key == Keys.Escape)
                {
                    TextBox4.Text += "esc";
                }
                if (key == Keys.Tab)
                {
                    TextBox4.Text += "tab";
                }
              
           } 
            
            
        }
        

       

        private void MouseHook_OnMouseActivity(int ncode, IntPtr wparam, IntPtr lparam)
        {
           
            if (ncode >= 0)
            {
                MouseHook.GetCursorPos(out var point);
              
                TextBox1.Text ="位置:"+point.X + " " + point.Y;
                var (r,g,b,a)=MouseHook.GetPointAndColor();
                TextBox2.Text = "rgb:" + r + " " + g + " " + b + " " + a;
                TextBox3.Text = "颜色:"+ColorTranslator.ToHtml(System.Drawing.Color.FromArgb(r, g, b));

                var colorR=Convert.ToByte(r);
                var colorG=Convert.ToByte(g);
                var colorB=Convert.ToByte(b);
                
                Button1.Background = new SolidColorBrush(Color.FromArgb(255,colorR,colorG,colorB));
            }
            
        }
        

        
        
    }
    public struct Point
    {
        public int X;
        public int Y;
        public Point(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }
    }


    public class ColorStruct
    {
        public int R { get; set; }
        public int G { get; set; }
        public int B { get; set; }
        public long A { get; set; }
    }

}