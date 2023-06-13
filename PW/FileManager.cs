using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dane
{
    public static class FileManager
    {
        //katalogi tutaj 
        public static readonly string BaseDataDirPath =
        Path.Combine(Environment.GetFolderPath(
            Environment.SpecialFolder.MyDocuments
            ),
            "kulki");

        public static void DirIsValid(bool writePath = false)
        {
            if (!Directory.Exists(BaseDataDirPath))
            {
                Directory.CreateDirectory(BaseDataDirPath);
            }
            //try - catch do uprawnien zapisu
            try
            {
                using var fs = File.Create(
                    Path.Combine(BaseDataDirPath, Path.GetRandomFileName()),
                    1,
                    FileOptions.DeleteOnClose);
            }
            catch (Exception e)
            {
                throw new Exception("Bazowy dokument zapisu logów nie posiada uprawnien do zapisu", e);
            }

            if (writePath) Console.WriteLine($"Dane sciezka = {BaseDataDirPath}");
        }

        //public static void UsunKatalog()
        //{
            //tutaj dodac usuwanie katalogu
        //}
    }
}
