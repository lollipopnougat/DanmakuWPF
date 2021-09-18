using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MColor = System.Windows.Media.Color;
using SolidColorBrush = System.Windows.Media.SolidColorBrush;

namespace DanmakuWPF.Utils
{
    static class ColorConvertor
    {
        public static MColor FromHtml(string colorStr)
        {
            Color color = ColorTranslator.FromHtml(colorStr);
            return MColor.FromArgb(color.A, color.R, color.G, color.B);
        }

        public static MColor FromDColor(Color color)
        {
            return MColor.FromArgb(color.A, color.R, color.G, color.B);
        }

        public static Color FromMColor(MColor color)
        {
            return Color.FromArgb(color.A, color.R, color.G, color.B);
        }



        public static string ToHtml(SolidColorBrush brush)
        {
            
            return ColorTranslator.ToHtml(FromMColor(brush.Color));
        }

        
    }
}
