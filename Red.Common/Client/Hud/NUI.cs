using System;
using System.Collections.Generic;
using System.Dynamic;
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
        /// Plays a sound to client assuming the user has PlayCustomSounds made by LondonStudios
        /// </summary>
        /// <param name="soundFile"></param>
        /// <param name="soundVolume"></param>
        public static void SoundToClient(string soundFile, float soundVolume) => TriggerEvent("Client:SoundToRadius", soundFile, soundVolume);
        /// <summary>
        /// Plays a sound to all clients assuming the user has PlayCustomSounds made by LondonStudios
        /// </summary>
        /// <param name="soundFile"></param>
        /// <param name="soundVolume"></param>
        public static void SoundToAll(string soundFile, float soundVolume) => TriggerEvent("Client:SoundToAll", soundFile, soundVolume);
        /// <summary>
        /// Plays a sound in a radius assuming the user has PlayCustomSounds made by LondonStudios
        /// </summary>
        /// <param name="networkId"></param>
        /// <param name="radius"></param>
        /// <param name="soundFile"></param>
        /// <param name="soundVolume"></param>
        public static void SoundToRadius(int networkId, float radius, string soundFile, float soundVolume) => TriggerEvent("Client:SoundToRadius", networkId, radius, soundFile, soundVolume);
        /// <summary>
        /// Plays a sound at specific coords assuming the user has PlayCustomSounds made by LondonStudios
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="radius"></param>
        /// <param name="soundFile"></param>
        /// <param name="soundVolume"></param>
        public static void SoundToCoords(float x, float y, float z, float radius, string soundFile, float soundVolume) => TriggerEvent("Client:SoundToCoords", x, y, z, radius, soundFile, soundVolume);
        /// <summary>
        /// Triggers an event to perform a sound to radius on the server side.
        /// </summary>
        /// <param name="networkId"></param>
        /// <param name="soundFile"></param>
        /// <param name="soundVolume"></param>
        public static void ServerSoundToClient(int networkId, string soundFile, float soundVolume) => TriggerServerEvent("Server:SoundToClient", networkId, soundFile, soundVolume);
        /// <summary>
        /// Triggers an event to perform a sound to all on the server side.
        /// </summary>
        /// <param name="soundFile"></param>
        /// <param name="soundVolume"></param>
        public static void ServerSoundToAll(string soundFile, float soundVolume) => TriggerServerEvent("Server:SoundToAll", soundFile, soundVolume);
        /// <summary>
        /// Triggers an event to perform a sound to radius on the server side.
        /// </summary>
        /// <param name="networkId"></param>
        /// <param name="radius"></param>
        /// <param name="soundFile"></param>
        /// <param name="soundVolume"></param>
        public static void ServerSoundToRadius(int networkId, float radius, string soundFile, float soundVolume) => TriggerServerEvent("Server:SoundToRadius", networkId, radius, soundFile, soundVolume);
        /// <summary>
        /// Triggers an event to perform a sound to coords on the server side.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="radius"></param>
        /// <param name="soundFile"></param>
        /// <param name="soundVolume"></param>
        public static void ServerSoundToCoords(float x, float y, float z, float radius, string soundFile, float soundVolume) => TriggerServerEvent("Server:SoundToCoords", x, y, z, radius, soundFile, soundVolume);
    }
}
