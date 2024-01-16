using System;
using System.Collections.Generic;
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;

namespace Red.Common.Client.Hud
{
    public class NUI : ClientScript
    {
        /// <summary>
        /// RegisterNUICallback converted from LUA to C#
        /// </summary>
        /// <param name="message"></param>
        /// <param name="callback"></param>
        public static void RegisterNUICallback(string message, Action<IDictionary<string, object>, CallbackDelegate> callback) => RegisterNuiCallback(message, callback);
        /// <summary>
        /// SendNUIMessage converted from LUA to C#
        /// </summary>
        /// <param name="message"></param>
        public static void SendNUIMessage(string message) => SendNuiMessage(message);
        /// <summary>
        /// SetNUIFocus converted from LUA to C#
        /// </summary>
        /// <param name="hasFocus"></param>
        /// <param name="hasCursor"></param>
        public static void SetNUIFocus(bool hasFocus, bool hasCursor) => SetNuiFocus(hasFocus, hasCursor);
        /// <summary>
        /// Advanced Version of SetNUIFocus with a keep input function.
        /// </summary>
        /// <param name="hasFocus"></param>
        /// <param name="hasCursor"></param>
        /// <param name="keepInput"></param>
        public static void SetNUIFocus(bool hasFocus, bool hasCursor, bool keepInput = false)
        {
            SetNUIFocus(hasFocus, hasCursor);
            SetNUIFocusKeepInput(keepInput);
        }
        /// <summary>
        /// SetNUIFocusKeepInput converted from LUA to C#
        /// </summary>
        /// <param name="keepInput"></param>
        public static void SetNUIFocusKeepInput(bool keepInput) => SetNuiFocusKeepInput(keepInput);
    }
}
