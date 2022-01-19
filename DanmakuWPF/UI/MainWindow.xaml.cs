using System;
using HandyControl;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using WebSocketClient = DanmakuWPF.Utils.WebSocketClient;
using ColorConvertor = DanmakuWPF.Utils.ColorConvertor;
using WSStates = System.Net.WebSockets.WebSocketState;
using DanmakuInfoHelper = DanmakuWPF.Danmaku.DanmakuInfoHelper;
using DanmakuInfo = DanmakuWPF.Danmaku.DanmakuInfo;
using FontInfoData = DanmakuWPF.Utils.FontInfoData;
using Logger = DanmakuWPF.Utils.Logger;
using System.Windows.Interop;
using WinService = DanmakuWPF.Utils.WindowsServices;
using ColorPicker = DanmakuWPF.Utils.DesktopColorPicker;

namespace DanmakuWPF.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : HandyControl.Controls.GlowWindow
    {
        private DanmakuCurtain dmkCurt;
        private WebSocketClient wsclient;
        private SolidColorBrush redBrush = new SolidColorBrush(ColorConvertor.FromHtml("#E36464"));
        private SolidColorBrush greenBrush = new SolidColorBrush(ColorConvertor.FromHtml("#0EC665"));
        private SolidColorBrush blueBrush = new SolidColorBrush(ColorConvertor.FromHtml("#2196F3"));
        private SolidColorBrush yellowBrush = new SolidColorBrush(ColorConvertor.FromHtml("#F8CB6B"));
        private SolidColorBrush selectedBrush = Brushes.White;
        private SolidColorBrush oriBrush;
        private FontInfoData infoData = new FontInfoData();
        private ColorPicker picker;
        private Logger logger = Logger.GetLogger();
        private bool isReply;
        private const int HOTKEY_PickColor = 102;
        //private const uint KEY_O = 79;
        private IntPtr handle;
        private object loglock = new();

        public MainWindow()
        {
            InitializeComponent();
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, () =>
            {
                dmkCurt = new DanmakuCurtain(chkShadow.IsChecked.Value);
                dmkCurt.Show();
                wsclient = new WebSocketClient(serverUrl.Text);
                wsclient.OnOpen += websocket_OnOpen;
                wsclient.OnClose += websocket_OnClose;
                wsclient.OnMessage += websocket_OnMessage;
                wsclient.OnError += websocket_OnError;
            });
            picker = new ColorPicker(ChangeColor);
            fontBox.ItemsSource = infoData.FontsList;
            fontBox.SelectedIndex = 0;


        }
        private void Window_Closed(object sender, EventArgs e)
        {
            dmkCurt.Close();
            logger.AddLog("shutdown DanmakuWPF.");
            logger.SaveToFiles("danmakuwpf.log");
            picker.ReleaseScreen();
            WinService.UnregisterHotKey(handle, HOTKEY_PickColor);
            Application.Current.Shutdown();
        }

        private void DispInvoke(Action act)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Normal, act);
        }

        #region 窗体消息预处理

        /// <summary>
        /// 资源初始化事件处理 获取句柄以及设置消息处理方法
        /// </summary>
        /// <param name="e">资源初始化事件</param>
        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            handle = new WindowInteropHelper(this).Handle;
            HwndSource hWndSource = HwndSource.FromHwnd(handle);
            if (hWndSource != null)
            {
                hWndSource.AddHook(WndProc);
            }
            bool res = WinService.RegisterHotKey(handle, HOTKEY_PickColor, WinService.KeyModifiers.Alt, WinService.Keys.O);

            if (res)
            {
                logger.AddLog("hotkey regist successful.");
            }
            else
            {
                logger.AddLog("hotkey regist faild.");
            }
        }

        /// <summary>
        /// 消息处理方法
        /// </summary>
        /// <param name="hwnd">窗体句柄</param>
        /// <param name="msg">消息编号</param>
        /// <param name="wParam">参数1</param>
        /// <param name="lParam">参数2</param>
        /// <param name="handled">是否以及处理</param>
        /// <returns></returns>
        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == WinService.WM_HOTKEY)
            {
                switch (wParam.ToInt32())
                {
                    case HOTKEY_PickColor:
                        TogglePickerColor();
                        logger.AddLog("Hit key ALT+O");
                        break;
                }

            }

            return IntPtr.Zero;
        }
        #endregion

        #region 取色器相关

        // 取色状态转换方法
        public void TogglePickerColor()
        {
            if (picker.IsStart)
            {
                picker.StopGetColor();
                //selectColorBtn.IsEnabled = true;
                selectColorBtn.Content = "选取颜色";
                selectColorBtn.Background = Brushes.White;
                selectColorBtn.Foreground = oriBrush;
                selectedBrush = colorPicker.SelectedBrush;
                logger.AddLog($"stop picker color.");
            }
            else
            {
                picker.StartGetColor();
                //selectColorBtn.IsEnabled = false;
                selectColorBtn.Content = "正在取色";
                selectColorBtn.Background = yellowBrush;
                oriBrush = (SolidColorBrush)selectColorBtn.Foreground;
                selectColorBtn.Foreground = Brushes.White;
                logger.AddLog($"start picker color.");
            }
        }

        /// <summary>
        /// 修改颜色（委托）
        /// </summary>
        /// <param name="r">红</param>
        /// <param name="g">绿</param>
        /// <param name="b">蓝</param>
        public void ChangeColor(byte r, byte g, byte b)
        {
            Dispatcher.Invoke(() =>
            {
                colorPicker.SelectedBrush = new SolidColorBrush(Color.FromRgb(r, g, b));
            });
            //MessageBox.Show($"{r},{g},{b}", "");
        }
        #endregion

        #region 按钮点击事件处理方法
        // 提交按钮
        private void submitDanmaku_Click(object sender, RoutedEventArgs e)
        {
            if (chkServer.IsChecked ?? false)
            {
                if (wsclient.State == WSStates.Open)
                {
                    Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, () =>
                    {
                        var font = (string)fontBox.SelectedItem;
                        var type = chkShadow.IsChecked.Value ? "shadow" : "outline";
                        var colorStr = ColorConvertor.ToHtml(selectedBrush);
                        wsclient.Send($"/send {{\"text\":\"{danmakuTextBox.Text}\",\"fontSize\":\"{(int)fontSizeSlider.Value}\",\"fill\":\"{colorStr}\",\"stroke\":\"#000000\",\"fontFamily\":\"{font}\",\"type\":\"{type}\"}}");
                    });
                    logger.AddLog($"send server '{danmakuTextBox.Text}'.");
                }
                else
                {
                    HandyControl.Controls.MessageBox.Show($"error: 啊这...服务器还没连接上呢!", "出错了！", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                logger.AddLog($"Shoot offline Danmaku'{danmakuTextBox.Text}'.");
                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                {
                    dmkCurt.ShadowEffect = chkShadow.IsChecked.Value;
                    var font = (string)fontBox.SelectedItem;
                    var type = chkShadow.IsChecked.Value ? "shadow" : "outline";
                    var colorStr = ColorConvertor.ToHtml(selectedBrush);
                    var info = new DanmakuInfo(danmakuTextBox.Text, fontSizeSlider.Value, colorStr, "#000000", font, type);
                    dmkCurt.Shoot(info);
                }));
            }

        }

        // 连接按钮
        private void conBtn_Click(object sender, RoutedEventArgs e)
        {
            if (wsclient.State == WSStates.None || wsclient.State == WSStates.Closed)
            {
                logger.AddLog($"Start Connecting Server.");
                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, () =>
                {
                    wsclient.Open();
                });
            }
            else if (wsclient.State == WSStates.Open)
            {
                DispInvoke(() => { logger.AddLog($"Stopping Connection."); });
                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, () =>
                {
                    wsclient.Close();
                });
            }
            else if (wsclient.State == WSStates.Connecting)
            {
                HandyControl.Controls.MessageBox.Show("休息一下吧，已经在连接了", "啊这...");
            }


        }

        // 设置服务器按钮
        private void serverBtn_Click(object sender, RoutedEventArgs e)
        {
            wsclient.SetUri(serverUrl.Text);
            logger.AddLog($"Set Server to '{serverUrl.Text}'.");
        }

        // 取色确定按钮
        private void colorPicker_Confirmed(object sender, HandyControl.Data.FunctionEventArgs<Color> e)
        {

            colorPicker.Visibility = Visibility.Hidden;
            selectedBrush = colorPicker.SelectedBrush;
            logger.AddLog($"Select Color {ColorConvertor.ToHtml(selectedBrush)}.");

        }

        // 取消取色按钮
        private void colorPicker_Canceled(object sender, EventArgs e)
        {
            colorPicker.Visibility = Visibility.Hidden;
            colorPicker.SelectedBrush = selectedBrush;
        }

        // 选择颜色按钮
        private void selectColorBtn_Click(object sender, RoutedEventArgs e)
        {
            if (picker.IsStart)
            {
                TogglePickerColor();
            }
            else
            {
                colorPicker.Visibility = Visibility.Visible;
            }
        }

        // 日志按钮
        private void logBtn_Click(object sender, RoutedEventArgs e)
        {
            DrawerRight.IsOpen = true;
            logBox.ItemsSource = logger.Lines;
        }

        // 置顶按钮
        private void top_Click(object sender, RoutedEventArgs e)
        {
            if (dmkCurt.Topmost)
            {
                dmkCurt.Topmost = false;
                topDanmakuBtn.Content = "保持弹幕顶层";

            }
            else
            {
                dmkCurt.Topmost = true;
                topDanmakuBtn.Content = "取消弹幕顶层保持";
            }

        }
        #endregion

        #region WS事件处理方法
        private void websocket_OnOpen(object sender, EventArgs e)
        {
            //statusPoint.Fill = greenBrush;
            logger.AddLog($"Connect to Server successful.");
            statusPoint.Dispatcher.Invoke(() => { statusPoint.Fill = greenBrush; });
            conBtn.Dispatcher.Invoke(() => { conBtn.Background = redBrush; });
            statusBlk.Dispatcher.Invoke(() => { statusBlk.Text = "已连接"; });
            conBtn.Dispatcher.Invoke(() => { conBtn.Content = "断开连接"; });
            logger.AddLog($"Send Server '/danmaku'.");
            wsclient.Send("/danmaku");
            isReply = false;
        }

        private void websocket_OnError(object sender, Exception ex)
        {
            logger.AddLog($"Error occurred! {ex.Message}");
            HandyControl.Controls.MessageBox.Show($"error: {ex.Message}", "出错了！", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void websocket_OnMessage(object sender, string data)
        {
            logger.AddLog($"Receive {data}");
            if (data[0] == '{')
            {
                var info = DanmakuInfoHelper.FromJson(data);
                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, () =>
                {
                    dmkCurt.Shoot(info);
                });
                logger.AddLog($"Shoot '{info.Text}'.");

            }
            else if (!isReply && data == "/ok")
            {
                logger.AddLog($"Server has recognised this client.");
            }
            else
            {
                logger.AddLog($"unrecognised request, just pass.");
                //dmkCurt.Shoot(data);
            }
        }

        private void websocket_OnClose(object sender, EventArgs e)
        {
            logger.AddLog($"Server Disconnected.");
            statusPoint.Dispatcher.Invoke(() => { statusPoint.Fill = redBrush; });
            conBtn.Dispatcher.Invoke(() => { conBtn.Background = blueBrush; });
            statusBlk.Dispatcher.Invoke(() => { statusBlk.Text = "未连接"; });
            conBtn.Dispatcher.Invoke(() => { conBtn.Content = "连接"; });

        }
        #endregion

    }
}
