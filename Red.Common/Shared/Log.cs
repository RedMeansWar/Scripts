using System;

namespace Red.Common
{
    public class Log
    {
        public static void Info(object message) => CitizenFX.Core.Debug.WriteLine($"[RED]: {message ?? "null"}");
        public static void Debug(object message) => Info($"[RED DEBUG]: {message ?? "null"}");
        public static void Error(object message) => Info($"[RED Error]: {message ?? "null"}");
        public static void Error(Exception ex) => Info($"[RED Exception Error]: {ex.Message}\n{ex}");

        public static void FrameworkInfo(object message) => CitizenFX.Core.Debug.WriteLine($"[FRAMEWORK]: {message ?? "null"}");
        public static void FrameworkDebug(object message) => Info($"[FRAMEWORK DEBUG]: {message ?? "null"}");
        public static void FrameworkError(object message) => Info($"[FRAMEWORK Error]: {message ?? "null"}");
        public static void FrameworkError(Exception ex) => Info($"[FRAMEWORK Exception Error]: {ex.Message}\n{ex}");
    }
}