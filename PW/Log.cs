using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Reflection.Metadata;
using System.Threading;
using System.IO;

namespace Dane
{
    public enum LogType { INFO, WARNING, ERROR, FATAL, DEBUG }
    public static class BallLogger
    {
        private static uint currLogFileNum = 0;
        private static uint maxLogFilesNum = 10;
        private static uint maxLogFileSizeKB = 256; // in KB

        private static readonly Queue<string> messagesToSave = new();

        private static Timer saveTaskTimer = null;
        private static bool saving = false;

        private static async void AddToBuffer(string message)
        {
            await Task.Run(() =>
            {
                lock (messagesToSave)
                {
                    messagesToSave.Enqueue(message);
                }
            });
        }

        private static string GetLogsDirPath()
        {
            return new StringBuilder("./.logs").ToString();
        }

        private static string GetLogFilePath()
        {
            return new StringBuilder(GetLogsDirPath()).Append("/LOG").Append(currLogFileNum).Append(".log").ToString();
            //return new StringBuilder(GetLogsDirPath()).Append("/LOG_").Append(DateTime.Now.ToString("yyyyMMddHHmmssff")).Append(".log").ToString();
        }

        private static void SaveTask()
        {
            if (!saving)
            {
                saving = true;
                lock (messagesToSave)
                {
                    int num = messagesToSave.Count;
                    while (num > 0)
                    {
                        FileInfo logFileInfo = new(GetLogFilePath());
                        if (logFileInfo.Length >= maxLogFileSizeKB * 1024)
                        {
                            currLogFileNum = (currLogFileNum + 1) % maxLogFilesNum;
                            logFileInfo = new(GetLogFilePath());
                            logFileInfo.Create().Close();
                        }
                        using (StreamWriter writer = logFileInfo.AppendText())
                        {
                            writer.WriteLine(messagesToSave.Dequeue());
                        }
                        num--;
                    }
                }
                saving = false;
            }

            /*if (!saving)
            {
                saving = true;
                lock (messagesToSave)
                {
                    if (messagesToSave.Count > 0)
                    {
                        FileInfo logFileInfo = new(GetLogFilePath());
                        using (StreamWriter writer = new(logFileInfo.Create()))
                        {
                            while (messagesToSave.Count > 0)
                            {
                                writer.WriteLine(messagesToSave.Dequeue());
                            }
                        }
                    }
                }
                saving = false;
            }*/
        }

        public static void StartLogging()
        {
            DirectoryInfo directoryInfo = new(GetLogsDirPath());
            if (!directoryInfo.Exists)
            {
                directoryInfo.Create();
            }
            // Dla drugiej wersji zakomentuj 2 linijki
            FileInfo logFileInfo = new(GetLogFilePath());
            logFileInfo.Create().Close();
            saveTaskTimer = new(new TimerCallback((obj) => { SaveTask(); }), null, 0, 1000);
        }

        public static void StopLogging()
        {
            saveTaskTimer.Change(Timeout.Infinite, Timeout.Infinite);
        }

        public static void Log(string message, LogType type)
        {
            DateTime logTime = DateTime.Now;
            StringBuilder sb = new StringBuilder(logTime.ToString("F")).Append(logTime.ToString(":ff")).Append(" [").Append(type.ToString()).Append("] ").Append(message);
            AddToBuffer(sb.ToString());
        }

        public static void SetMaxLogFilesNum(uint value)
        {
            maxLogFilesNum = value;
        }

        public static void SetMaxLogFileSizeKB(uint value)
        {
            maxLogFileSizeKB = value;
        }
    }
}
