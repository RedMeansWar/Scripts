﻿using System;
using System.Collections.Generic;
using System.Dynamic;
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;

namespace Red.Common.Client.Hud
{
    public class NUI : BaseScript
    {
        /// <summary>
        /// RegisterNUICallback converted from LUA to C#
        /// </summary>
        /// <param name="message"></param>
        /// <param name="callback"></param>
        public static void RegisterNUICallback(string message, Action<IDictionary<string, object>, CallbackDelegate> callback)
        {
            RegisterNuiCallbackType(message);
            RegisterNuiCallback(message, callback);
        }
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
    }
}