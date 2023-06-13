using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;

namespace Dane
{
    public interface ILogWriter : IDisposable
    {
        void Write(IEnumerable<LogAccess> logAccesses);
    }
    public class LogWriter : ILogWriter
    {
        private string _logPlikSciezka;

        public void Dispose()
        {
            //FileManager.UsunKatalog();
        }

        public LogWriter(string nazwaPliku = "")
        {
            FileManager.DirIsValid();

            if (String.IsNullOrWhiteSpace(nazwaPliku)) nazwaPliku = $"Pozycje({DateTime.Now:yyyy-MM-dd' 'HH-mm-ss}).log";
            //$"Kolizje({DateTime.Now:'D'yyyy-MM-dd'T'HH-mm-ss}).log"
            _logPlikSciezka = Path.Combine(FileManager.BaseDataDirPath, nazwaPliku);
            File.Create(_logPlikSciezka);
        }

        public void Write(IEnumerable<LogAccess> logAccesses)
        {
            var lg = new StringBuilder();


            foreach (var logAccess in logAccesses)
            {
                lg.Append("| ").Append(logAccess.TimeStamp)
                    .Append(" | ").Append(logAccess.Setting)
                    .Append(" # ").Append(logAccess.LineNumber)
                    .Append("\t").Append(logAccess.Msg).AppendLine();
            }

            try
            {
                using (StreamWriter writer = File.AppendText(_logPlikSciezka))
                {
                    writer.WriteLine(lg.ToString());
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
                throw;
            }

        }

    }
}
