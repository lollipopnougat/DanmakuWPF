using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace DanmakuWPF.Danmaku
{
    public class DanmakuManager
    {
        private Grid container;

        private int lineHeight = 56;
        private int paddingTop = 8;

        private bool[] isOccupy;
        private bool enableShadowEffect;
        private int lines;

        public DanmakuManager(Grid grid, bool enableShadow)
        {
            container = grid;

            lines = (int)(container.RenderSize.Height / lineHeight) - 1;
            isOccupy = new bool[lines];

            enableShadowEffect = enableShadow;
        }

        public int usableLine()
        {
            for (int line = 0; line < lines; line += 1)
            {
                if (!isOccupy[line])
                {
                    isOccupy[line] = true;
                    return line;
                }
            }
            return -1;
        }

        public void clearLine()
        {
            for (int line = 0; line < lines; line += 1)
            {
                isOccupy[line] = false;
            }
        }

        public int lineLocationY(int line)
        {
            return (line * lineHeight) + paddingTop;
        }


        public void Shoot(string text)
        {
            var info = new DanmakuInfo(text, 36, "#ffffff", "#000000", "微软雅黑", "shadow");
            //var line = usableLine();

            //if (line == -1)
            //{
            //    clearLine();
            //    line = usableLine();
            //}

            //FrameworkElement danmaku; // = new OutlineDanmaku(text);
            // Danmaku initilization and display
            if (enableShadowEffect)
            {
                //danmaku = new ShadowDanmaku(text);
                info.Type = "shadow";
            }
            else
            {
                //danmaku = new OutlineDanmaku(text);
                info.Type = "outline";
            }

            // 描述矩形边框的粗细
            //danmaku.Margin = new Thickness(0, lineLocationY(line), 0, 0);
            //container.Children.Add(danmaku);

            // Initilizing animation
            //var anim = new DoubleAnimation();
            //anim.From = this.container.RenderSize.Width;
            //anim.To = -danmaku.DesiredSize.Width - 1600;
            //anim.SpeedRatio = danmaku.DesiredSize.Width > 80 ?
            //    (.05 * (danmaku.DesiredSize.Width / 1500 + 1)) :
            //    (.1 * ((100 - danmaku.DesiredSize.Width) / 100 + 1));
            //TranslateTransform trans = new TranslateTransform();
            //danmaku.RenderTransform = trans;

            // Handling the end of danmaku
            //anim.Completed += new EventHandler(delegate (object o, EventArgs a) {
            //    container.Children.Remove(danmaku);
            //});

            // Managing the danmaku lines
            //var timer = new DispatcherTimer();
            //timer.Interval = TimeSpan.FromMilliseconds(300);
            //timer.Tick += new EventHandler(delegate (object o, EventArgs a) {
            //    Point relativePoint = danmaku.TransformToAncestor(container)
            //              .Transform(new Point(0, 0));
            //    if (relativePoint.X < container.ActualWidth - danmaku.DesiredSize.Width - 50)
            //    {
            //        timer.Stop();
            //        isOccupy[line] = false;
            //    }
            //});
            //timer.Start();

            // Play animation
            //trans.BeginAnimation(TranslateTransform.XProperty, anim);
            Shoot(info);
        }

        public void Shoot(DanmakuInfo info)
        {
            var line = usableLine();

            if (line == -1)
            {
                clearLine();
                line = usableLine();
            }

            FrameworkElement danmaku; // = new OutlineDanmaku(text);
            // Danmaku initilization and display
            if (info.Type == "shadow")
            {
                danmaku = new ShadowDanmaku(info.Text, info.FontSize, info.Fill, info.FontFamily);
            }
            else
            {
                danmaku = new OutlineDanmaku(info.Text, info.FontSize, info.Fill, info.Stroke, info.FontFamily);
            }

            // 描述矩形边框的粗细
            danmaku.Margin = new Thickness(0, lineLocationY(line), 0, 0);
            container.Children.Add(danmaku);

            // Initilizing animation
            var anim = new DoubleAnimation();
            anim.From = this.container.RenderSize.Width;
            anim.To = -danmaku.DesiredSize.Width - 1600;
            anim.SpeedRatio = danmaku.DesiredSize.Width > 80 ?
                (.05 * (danmaku.DesiredSize.Width / 1500 + 1)) :
                (.1 * ((100 - danmaku.DesiredSize.Width) / 100 + 1));
            TranslateTransform trans = new TranslateTransform();
            danmaku.RenderTransform = trans;

            // Handling the end of danmaku
            anim.Completed += new EventHandler(delegate (object o, EventArgs a) {
                container.Children.Remove(danmaku);
            });

            // Managing the danmaku lines
            var timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(300);
            timer.Tick += new EventHandler(delegate (object o, EventArgs a) {
                Point relativePoint = danmaku.TransformToAncestor(container)
                          .Transform(new Point(0, 0));
                if (relativePoint.X < container.ActualWidth - danmaku.DesiredSize.Width - 50)
                {
                    timer.Stop();
                    isOccupy[line] = false;
                }
            });
            timer.Start();

            // Play animation
            trans.BeginAnimation(TranslateTransform.XProperty, anim);
        }

    }
}
