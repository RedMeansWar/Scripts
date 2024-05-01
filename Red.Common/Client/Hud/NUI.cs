using System;
using System.Collections.Generic;
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;

namespace Red.Common.Client.Hud
{
    public class NUI : BaseScript
    {
        /// <summary>
        /// Registers a callback for a specific NUI message, providing a convenient way to handle NUI interactions.
        /// </summary>
        /// <param name="message">The NUI message to register the callback for.</param>
        /// <param name="callback">The callback function to invoke when the NUI message is received.</param>
        public static void RegisterNUICallback(string message, Action<IDictionary<string, object>, CallbackDelegate> callback)
        {
            // Utilize the underlying RegisterNuiCallback function for registration.
            RegisterNuiCallback(message, callback);
        }

        /// <summary>
        /// Sends a message to the NUI (New User Interface), providing a bridge for communication between C# and the JavaScript NUI file.
        /// </summary>
        /// <param name="message">The message to send to the NUI.</param>
        public static void SendNUIMessage(string message)
        {
            // Utilize the underlying SendNuiMessage function to transmit the message to the NUI.
            SendNuiMessage(message);
        }

        /// <summary>
        /// Sets the focus and cursor visibility for the NUI (New User Interface), controlling user interaction with NUI elements.
        /// </summary>
        /// <param name="hasFocus">Whether the NUI should have input focus.</param>
        /// <param name="hasCursor">Whether the mouse cursor should be visible over the NUI.</param>
        public static void SetNUIFocus(bool hasFocus, bool hasCursor)
        {
            // Utilize the underlying SetNuiFocus function to adjust NUI focus and cursor visibility.
            SetNuiFocus(hasFocus, hasCursor);
        }

        /// <summary>
        /// Sets the focus and cursor visibility for the NUI (New User Interface), optionally preserving input even when losing focus.
        /// </summary>
        /// <param name="hasFocus">Whether the NUI should have input focus.</param>
        /// <param name="hasCursor">Whether the mouse cursor should be visible over the NUI.</param>
        /// <param name="keepInput">Whether to keep input handling even when losing focus (default: false).</param>
        public static void SetNUIFocusWithInput(bool hasFocus, bool hasCursor, bool keepInput = false)
        {
            // Set the NUI focus and cursor visibility.
            SetNUIFocus(hasFocus, hasCursor);

            // Optionally enable input handling persistence.
            SetNuiFocusKeepInput(keepInput);
        }

        /// <summary>
        /// Controls whether the NUI (New User Interface) retains input handling even when losing focus.
        /// </summary>
        /// <param name="keepInput">Whether to keep input handling when the NUI loses focus.</param>
        public static void SetNUIFocusKeepInput(bool keepInput)
        {
            // Utilize the underlying SetNuiFocusKeepInput function to set input persistence behavior.
            SetNuiFocusKeepInput(keepInput);
        }

        /// <summary>
        /// Checks if the NUI (New User Interface) has focus.
        /// </summary>
        /// <returns>True if the NUI has focus, false otherwise.</returns>
        public static bool IsNUIFocused()
        {
            return IsNuiFocused();
        }

        /// <summary>
        /// Checks if the NUI has focus while allowing input to pass through to the game.
        /// </summary>
        /// <returns>True if the NUI has focus while keeping input open, false otherwise.</returns>
        public static bool IsNUIFocusedKeepingInput()
        {
            return IsNuiFocusKeepingInput();
        }
    }
}
