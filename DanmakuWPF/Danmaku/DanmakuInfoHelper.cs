using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DanmakuWPF.Danmaku
{
    public static class DanmakuInfoHelper
    {
        public static DanmakuInfo FromJson(string json)
        {
            var text = GetJsonValue(json, "text");
            var dou = GetJsonValue(json, "fontSize");
            var fontSize = double.Parse(dou, System.Globalization.NumberStyles.Float);
            var fill = GetJsonValue(json, "fill");
            var stroke = GetJsonValue(json, "stroke");
            var fontFamily = GetJsonValue(json, "fontFamily");
            var type = GetJsonValue(json, "type");

            var info = new DanmakuInfo(text, fontSize, fill, stroke, fontFamily, type);
            return info;

        }

        private static string GetJsonValue(string json, string key)
        {
            var pattern = new Regex($@"[""|']{key}[""|']\s?:\s?[""|'](.+?)[""|']");
            var matches = pattern.Match(json);
            if (matches.Groups.Count != 0)
            {
                return matches.Groups[1].Value;
            }
            else
            {
                return null;
            }
        }
    }

    /*
     * {
     *   "text": "aaabbb",
     *   "fontSize": "16",
     *   "fill": "#ffffff", 
     *   "stroke": "#ffffff",
     *   "fontFamily": "xxxx",
     *   "type": "outline/shadow"
     * }
     */
}
