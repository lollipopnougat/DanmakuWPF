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

        private static Dictionary<string, string> ParseJson(string json)
        {
            var dict = new Dictionary<string, string>();
            int state = 0;
            var sb = new StringBuilder();
            var key = "";
            foreach (var ch in json)
            {
                switch (state)
                {
                    case 0:
                        if (ch == '{')
                        {
                            state = 1;
                        }
                        break;
                    case 1:
                        if (ch == '"')
                        {
                            state = 2;
                        }
                        break;
                    case 2:
                        if (ch == '"')
                        {
                            key = sb.ToString();
                            sb.Clear();
                            state = 3;
                        }
                        else
                        {
                            sb.Append(ch);
                        }
                        break;
                    case 3:
                        if (ch == ':')
                        {
                            state = 4;
                        }
                        break;
                    case 4:
                        if (ch == '"')
                        {
                            state = 5;
                        }
                        break;
                    case 5:
                        if (ch == '"')
                        {
                            var value = sb.ToString();
                            dict.Add(key, value);
                            sb.Clear();
                            state = 6;
                        }
                        else
                        {
                            sb.Append(ch);
                        }
                        break;
                    case 6:
                        if (ch == '}')
                        {
                            state = 7;
                        }
                        break;
                    case 7:
                        break;
                }
            }
            return dict;
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
}
