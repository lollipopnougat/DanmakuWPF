using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Effects;
using ColorConvertor = DanmakuWPF.Utils.ColorConvertor;


namespace DanmakuWPF.Danmaku
{
    public class OutlineDanmaku : Utils.OutlineTextBlock
    {
        public OutlineDanmaku(string text, double fontSize = 36, string fill = "#ffffff", string stroke = "#000000", string fontFamily = "微软雅黑")
        {
            FontSize = fontSize;
            Text = text;
            Fill = new SolidColorBrush(ColorConvertor.FromHtml(fill));
            Stroke = new SolidColorBrush(ColorConvertor.FromHtml(stroke));
            FontFamily = new FontFamily(fontFamily);
            
            Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            Arrange(new Rect(0, 0, DesiredSize.Width, DesiredSize.Height));
        }

    }

    public class ShadowDanmaku : TextBlock
    {
        //TODO: Optimizing the performance of danmaku animation with shadow.
        public ShadowDanmaku(string text, double fontSize = 36, string fill = "#ffffff", string fontFamily = "微软雅黑")
        {
            FontSize = fontSize;
            Text = text;
            Foreground = new SolidColorBrush(ColorConvertor.FromHtml(fill));
            FontFamily = new FontFamily(fontFamily);

            Effect = new DropShadowEffect
            {
                Color = Colors.Black,
                BlurRadius = 2,
                ShadowDepth = 1,
                Opacity = 1,
                RenderingBias = RenderingBias.Performance
            };

            Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            Arrange(new Rect(0, 0, DesiredSize.Width, DesiredSize.Height));
        }
    }
}
