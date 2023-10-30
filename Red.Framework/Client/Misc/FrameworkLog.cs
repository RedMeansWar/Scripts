using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Red.Framework.Client.Misc
{
    public class FrameworkLog
    {
        public static void Info(object message) => CitizenFX.Core.Debug.WriteLine($"[FRAMEWORK - CLIENT]: {DateTime.Now:yyyy/MM/dd HH:mm:ss} - {message ?? "null"}");
        public static void Debug(object message) => Info($"[FRAMEWORK - CLIENT DEBUG]: {message ?? "null"}");
        public static void Error(object message) => CitizenFX.Core.Debug.WriteLine($"[FRAMEWORK - CLIENT ERROR]: {message}\n{message}");
        public static void Error(Exception ex) => CitizenFX.Core.Debug.WriteLine($"[FRAMEWORK - CLIENT ERROR]: {ex.Message}\n{ex}");
    }
}
