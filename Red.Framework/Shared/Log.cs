using System;

namespace Red.Framework
{
    public class Log
    {
        public static void Info(object message) => CitizenFX.Core.Debug.WriteLine($"[FRAMEWORK]: {DateTime.Now:yyyy/MM/dd HH:mm:ss} - {message ?? "null"}");

        public static void Debug(object message) => Info($"[FRAMEWORK]: {DateTime.Now:yyyy/MM/dd HH:mm:ss} - {message ?? "null"}");

        public static void Error(object message) => Info($"[FRAMEWORK ERROR]: {message ?? "null"}");

        public static void Error(Exception exception) => Info($"[FRAMEWORK EXCEPTION ERROR]: {exception.Message}\n{exception}");
    }
}