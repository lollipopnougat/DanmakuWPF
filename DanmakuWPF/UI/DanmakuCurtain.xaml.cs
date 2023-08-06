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
using System.Windows.Shapes;
using System.Windows.Interop;
using DanmakuManager = DanmakuWPF.Danmaku.DanmakuManager;
using DanmakuInfo = DanmakuWPF.Danmaku.DanmakuInfo;

namespace DanmakuWPF.UI
{
    /// <summary>
    /// DanmakuCurtain.xaml 的交互逻辑
    /// </summary>
    public partial class DanmakuCurtain : Window
    {
        //public DanmakuCurtain()
        //{
        //    InitializeComponent();
        //}

        #region 准备阶段
        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            var hwnd = new WindowInteropHelper(this).Handle;
            Utils.WindowsServices.SetWindowExTransparent(hwnd);
        }
        #endregion


        public double screenHeight = SystemParameters.PrimaryScreenHeight;
        public double screenWidth = SystemParameters.PrimaryScreenWidth;

        public bool ShadowEffect { get; set; }

        private DanmakuManager dm = null;


        public DanmakuCurtain(bool enableShadow = false)
        {
            InitializeComponent();

            Top = 0;
            Left = 0;
            Width = screenWidth;
            Height = screenHeight;

            ShadowEffect = enableShadow;
        }

        public void Shoot(string text)
        {
            if (dm == null)
            {
                dm = new DanmakuManager(curtain, ShadowEffect);
            }
            dm = dm ?? new DanmakuManager(curtain, ShadowEffect);

            dm.Shoot(text);
        }

        public void Shoot(DanmakuInfo info)
        {
            if (dm == null)
            {
                dm = new DanmakuManager(curtain, ShadowEffect);
            }
            dm = dm ?? new DanmakuManager(curtain, ShadowEffect);
            dm.Shoot(info);
        }

    }
}
