using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DanmakuWPF.Utils
{
    /// <summary>
    /// 加载安装的所有字体 绑定用
    /// </summary>
    public class FontInfoData
    {
        public FontInfoData()
        {
            
            InstalledFontCollection fonts = new InstalledFontCollection();
            FontsList = new List<string>();
            foreach (FontFamily family in fonts.Families)
            {
                FontsList.Add(family.Name);
            }
            

        }
        public List<string> FontsList { get; set; }
    }
}
