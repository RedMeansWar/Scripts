using System;
using System.Collections.Generic;
using System.Text;

namespace Red.Framework.Server.Misc
{
    public class FrameworkLog
    {
        public static void Info(object message) => CitizenFX.Core.Debug.WriteLine($"[FRAMEWORK - SERVER]: {DateTime.Now:yyyy/MM/dd HH:mm:ss}] {message ?? "null"}");
        public static void Debug(object message) => Info($"[FRAMEWORK - SERVER DEBUG]: {message ?? "null"}");
        public static void Error(object message) => CitizenFX.Core.Debug.WriteLine($"[FRAMEWORK - SERVER ERROR]: {message}\n{message}");
        public static void Error(Exception ex) => CitizenFX.Core.Debug.WriteLine($"[FRAMEWORK - SERVER ERROR]: {ex.Message}\n{ex}");
    }
}
