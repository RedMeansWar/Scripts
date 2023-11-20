using System;

namespace Red.Common.Client.Diagnostics
{
    public class Log
    {
        public static void Info(object message) => CitizenFX.Core.Debug.WriteLine($"[CLIENT]: {DateTime.Now:yyyy/MM/dd HH:mm:ss} - {message ?? "null"}");
        public static void Debug(object message) => Info($"[CLIENT DEBUG]: {message ?? "null"}");
        public static void Error(object message) => CitizenFX.Core.Debug.WriteLine($"[CLIENT ERROR]: {message}\n{message}");
        public static void Error(Exception ex) => CitizenFX.Core.Debug.WriteLine($"[CLIENT ERROR]: {ex.Message}\n{ex}");
    }
}
