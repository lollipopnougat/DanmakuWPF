using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DanmakuWPF.Danmaku
{
    public class DanmakuInfo
    {
        public DanmakuInfo(string text, double fontSize, string fill, string stroke, string fontFamily, string type)
        {
            Text = text;
            FontSize = fontSize;
            Fill = fill;
            Stroke = stroke;
            FontFamily = fontFamily;
            Type = type;
        }

        public string Text { get; set; }

        private double _fontSize;
        public double FontSize
        {
            get => _fontSize;
            set 
            { 
                if (value > 56)
                {
                    _fontSize = 56;
                }
                else if(value < 16)
                {
                    _fontSize = 16;
                }
                else
                {
                    _fontSize = value;
                }
            }
        }
        public string Fill { get; set; }
        public string Stroke { get; set; }
        public string FontFamily { get; set; }
        public string Type { get; set; }
    }
}
