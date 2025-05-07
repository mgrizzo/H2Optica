using System;
using System.IO;

namespace H2OpticaLogic
{
    public static class Logger
    {
        public static readonly string logFilePath = "log.txt";

        public static void Log(string message)
        {
            try
            {
                File.AppendAllText(logFilePath, $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}{Environment.NewLine}");
            }
            catch { };
        }

        public static void Log(Exception exception)
        {
            Log($"[EXCEPTION] - {DateTime.Now:yyyy-MM-dd HH:mm:ss}: {exception.Message} \n{exception.StackTrace}");
        }
    }
}
