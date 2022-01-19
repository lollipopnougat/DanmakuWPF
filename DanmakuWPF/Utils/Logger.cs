using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace DanmakuWPF.Utils
{
    class Logger
    {
        
        private ObservableCollection<string> lines;
        public ObservableCollection<string> Lines
        {
            get
            {
                return lines;//.ToList();
            }
        }

        private static object _syncLock = new object();

        private Logger()
        {
            lines = new ObservableCollection<string>();
            BindingOperations.EnableCollectionSynchronization(lines, _syncLock);
            AddLog("init Logger.");
        }

        public static Logger logger;

        public static Logger GetLogger()
        {
            logger = logger ?? new Logger();
            return logger;
        }

        public void AddLog(string text)
        {
            var now = DateTime.Now;
            var item = $"[{now.ToString("G")}] {text}";
            lines.Add(item);
        }

        public string GetAllLines()
        {
            return string.Join("\n", lines);
        }

        public void SaveToFiles(string path)
        {
            File.WriteAllLines(path, lines, Encoding.UTF8);
        }

        public void Clear()
        {
            lines.Clear();
        }

    }
}
