using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnviPath.Editor
{
    class Logger : IDisposable
    {
        private readonly FileStream _fs;

        private readonly StreamWriter _sw;

        public Logger()
        {
            this._fs = File.Open("./editor.log", FileMode.Append, FileAccess.Write);
            this._sw = new StreamWriter(this._fs);
        }

        public void WriteLine(EnvironmentVariableTarget target, string path)
        {
            this._sw.WriteLine($"[{target}]: Current=> {path}");
        }

        public void Dispose()
        {
            this._sw.WriteLine();
            this._sw.Close();
            this._fs.Close();
        }
    }
}
