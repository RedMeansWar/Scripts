using System;

namespace Red.Common.Client.Diagnostics
{
    public static class Log
    {
        public static void Info(object message) => CitizenFX.Core.Debug.WriteLine($"[INFO]: {DateTime.Now:yyyy/MM/dd HH:mm:ss} -  {message ?? "null"}");
        public static void Debug(object message) => Info($"[DEBUG]: {message ?? "null"}");
        public static void Error(object message) => CitizenFX.Core.Debug.WriteLine($"[ERROR]: {message}\n{message}");
        public static void Error(Exception ex) => CitizenFX.Core.Debug.WriteLine($"[ERROR]: {ex.Message}\n{ex}");
    }
}
