using System;
using System.Collections.Generic;
using System.Text;

namespace Red.Common.Server.Diagnostics
{
    public class Log
    {
        public static void Info(object message) => CitizenFX.Core.Debug.WriteLine($"[SERVER]: {DateTime.Now:yyyy/MM/dd HH:mm:ss} - {message ?? "null"}");
        public static void Debug(object message) => Info($"[SERVER DEBUG]: {message ?? "null"}");
        public static void Error(object message) => CitizenFX.Core.Debug.WriteLine($"[SERVER ERROR]: {message}\n{message}");
        public static void Error(Exception ex) => CitizenFX.Core.Debug.WriteLine($"[SERVER ERROR]: {ex.Message}\n{ex}");

    }
}
