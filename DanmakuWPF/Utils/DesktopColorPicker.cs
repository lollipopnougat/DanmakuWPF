using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Timers;
using System.Windows.Media;

namespace DanmakuWPF.Utils
{
    public delegate void DelegateChangeMyColor(byte r, byte g, byte b); //用于在timer中修改UI的委托
    public delegate void DelegateUIControl();
    class DesktopColorPicker
    {

        private Timer tim;
        private bool isRunningGetColor;
        private IntPtr hdc;
        public bool IsStart => isRunningGetColor;

        private DelegateChangeMyColor cmc;
        //private DelegateUIControl duc;

        public DesktopColorPicker(DelegateChangeMyColor cm)
        {
            cmc = cm;
            //duc = du;
        }

        #region Win32API

        [DllImport("gdi32")]
        private static extern int DeleteObject(IntPtr o); // 删除内核对象

        [DllImport("user32")]
        public static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC); //释放场景句柄

        [DllImport("user32.dll")]
        private static extern IntPtr GetDC(IntPtr hwnd); //获取设备场景句柄

        [DllImport("gdi32.dll")]
        private static extern int GetPixel(IntPtr hdc, int x, int y); //取指定点颜色


        [DllImport("user32.dll")]
        public static extern bool GetCursorPos(out POINT lpPoint); //获取光标所在的坐标

        #endregion
        public void StartGetColor()
        {
            if (tim == null)
            {
                hdc = GetDC(IntPtr.Zero);//取到设备场景(0就是全屏的设备场景)
                tim = new Timer();
                tim.Interval = 20;
                tim.Elapsed += new ElapsedEventHandler(GetColor); // Timer定时结束执行的方法
            }
            if (!isRunningGetColor)
            {
                isRunningGetColor = true;
                //duc(); //让UI不可操作(委托)
                //label.Content = "当前：屏幕取色模式";
                tim.Start();
            }

        }

        public void StopGetColor()
        {
            if (isRunningGetColor)
            {
                tim.Stop();
                //label.Content = "当前：软件选色模式";
                //ReleaseDC(IntPtr.Zero, hdc);
                //DeleteObject(hdc);
                isRunningGetColor = false;
                //duc(); //让UI可操作
            }

        }

        public void ReleaseScreen()
        {
            if (hdc != IntPtr.Zero)
            {
                _ = ReleaseDC(IntPtr.Zero, hdc);
            }
        }

        private void GetColor(object source, ElapsedEventArgs e)
        {
            tim.Stop();
            POINT po;
            _ = GetCursorPos(out po);
            //POINT p = new POINT(po.X, po.Y);

            int c = GetPixel(hdc, po.X, po.Y);//取指定点颜色
            var r = (byte)(c & 0xFF); //转换R
            var g = (byte)((c & 0xFF00) / 256);
            var b = (byte)((c & 0xFF0000) / 65536);

            //byte r = (byte)(c & 0xFF);
            //byte g = (byte)((c & 0xFF00) / 256);//转换G
            //byte b = (byte)((c & 0xFF0000) / 65536);//转换B

            cmc(r, g, b); //UI同步
            if (isRunningGetColor)
            {
                tim.Start();
            }

        }

        /// <summary>
        /// 根据给出的RGB数值字符串返回构造的Color对象
        /// </summary>
        /// <param name="reds">R</param>
        /// <param name="greens">G</param>
        /// <param name="blues">B</param>
        /// <returns>Color对象</returns>
        //public static Color getColorByRGBString(string reds, string greens, string blues)
        //{
        //    if (!(int.TryParse(reds, out int red) && int.TryParse(greens, out int green) && int.TryParse(blues, out int blue))) throw new ArgumentException("RGB格式不正确");
        //    //red &= 0xFF;
        //    //green &= 0xFF;
        //    //blue &= 0xFF;
        //    return Color.FromRgb((byte)red, (byte)green, (byte)blue);
        //}



    }
}
