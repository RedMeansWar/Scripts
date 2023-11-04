using CitizenFX.Core;
using CitizenFX.Core.UI;
using Red.Common.Client.Misc;
using System;
using static CitizenFX.Core.Native.API;
using static CitizenFX.Core.UI.Screen;

namespace Red.Common.Client.Hud
{
    public class HUD : ClientScript
    {
        #region Extensions
        public static void DisplayHUD(bool display = true) => DisplayHud(display);
        public static void SetMaxArmorDisplay(int maxValue) => SetMaxArmourHudDisplay(maxValue);
        public static void SetMaxHealthDisplay(int maxValue) => SetMaxHealthHudDisplay(maxValue); 
        public static void ChangeFakeCash(int cash, int bank) => ChangeFakeMpCash(cash, bank);
        public static void HideHUDComponent(HudComponent component) => HideHudComponentThisFrame((int)component);
        public static void HideHUDComponent(int component) => HideHudComponentThisFrame(component);
        public static void HideHUDComponent(HUDIdentifier component) => HideHudComponentThisFrame((int)component);
        public static void ForceWeaponWheel(bool show = true) => HudForceWeaponWheel(show);
        public static void SetBlipOpacitity(Blip blip, int opacitity) => SetBlipAlpha(blip.Handle, opacitity);
        public static void SetComponentPosition(int componentId, float x, float y) => SetHudComponentPosition(componentId, x, y);
        public static void SetComponentPosition(HUDIdentifier componentId, float x, float y) => SetHudComponentPosition((int)componentId, x, y);
        public static void SetComponentPosition(int componentId, Vector2 position) => SetHudComponentPosition(componentId, position.X, position.Y);
        public static void SetComponentPosition(HUDIdentifier componentId, Vector2 position) => SetHudComponentPosition((int)componentId, position.X, position.Y);
        public static void SetHUDAnimationStopLevel(Player player, bool toggle) => N_0xde45d1a1ef45ee61(player.Handle, toggle);
        public static void SetHUDAnimationStopLevel(int player, bool toggle) => N_0xde45d1a1ef45ee61(player, toggle);
        public static void DisplayHUDWhenPaused() => DisplayHudWhenPausedThisFrame();
        public static void DisplayHUDWhenDead() => DisplayHudWhenDeadThisFrame();
        public static void SetBlipName(Blip blip) => EndTextCommandSetBlipName(blip.Handle);

        public static bool IsScriptedComponentActive(int componentId) => IsScriptedHudComponentActive(componentId);
        public static bool IsScriptedComponentActive(HUDIdentifier componentId) => IsScriptedHudComponentActive((int)componentId);
        public static bool IsScriptedComponentHiddenThisFrame(int componentId) => IsScriptedHudComponentHiddenThisFrame(componentId);
        public static bool IsScriptedComponentHiddenThisFrame(HUDIdentifier componentId) => IsScriptedHudComponentHiddenThisFrame((int)componentId);
        public static bool IsHUDHidden() => IsHudHidden();
        public static bool IsThisHelpMessageBeingDisplayed(int index) => EndTextCommandIsThisHelpMessageBeingDisplayed(index);
        public static bool IsThisHelpMessageBeingDisplayed(HUDIndex index) => EndTextCommandIsThisHelpMessageBeingDisplayed((int)index);
        public static bool HUDIsVisable => Screen.Hud.IsVisible;

        public static int DisplayFeedPostAward(string textureDict, string textureName, int rpBonus, int colorOverlay, string title) => EndTextCommandThefeedPostAward(textureDict, textureName, rpBonus, colorOverlay, title);
        public static int DisplayFeedPostCrewRankup(string charTitle, string clanTextureDict, string clanTextureName, bool isImportant, bool showSubtitle) => EndTextCommandThefeedPostCrewRankup(charTitle, clanTextureDict, clanTextureName, isImportant, showSubtitle);
        public static int DisplayFeedPostCrewtag(bool crewIsPrivate, bool containsRockstar, int crewTag, int rank, bool hasFounder, bool isImportant, int clanHandle, int r, int g, int b) => EndTextCommandThefeedPostCrewtag(crewIsPrivate, containsRockstar, ref crewTag, rank, hasFounder, isImportant, clanHandle, r, g, b);
        public static int DisplayFeedPostCrewtagWithGameName(bool crewIsPrivate, bool containsRockstar, int crewTag, int rank, bool isLeader, bool isImportant, int clanHandle, string gamerStr, int r, int g, int b) => EndTextCommandThefeedPostCrewtagWithGameName(crewIsPrivate, containsRockstar, ref crewTag, rank, isLeader, isImportant, clanHandle, gamerStr, r, g, b);
        public static int DisplayMessageText(string textureDict, string textureName, bool flash, int iconType, string sender, string subject) => EndTextCommandThefeedPostMessagetext(textureDict, textureName, flash, iconType, sender, subject);
        public static int DisplayMessageText(string textureDict, string textureName, bool flash, IconType iconType, string sender, string subject) => EndTextCommandThefeedPostMessagetext(textureDict, textureName, flash, (int)iconType, sender, subject);
        public static int DisplayFeedPostStats(string statTitle, int iconEnum, bool stepVal, int barValue, bool isImportant, string picTextureDict, string picTextureName) => EndTextCommandThefeedPostStats(statTitle, iconEnum, stepVal, barValue, isImportant, picTextureDict, picTextureName);
        public static int DisplayFeedPostStats(string statTitle, IconType iconEnum, bool stepVal, int barValue, bool isImportant, string picTextureDict, string picTextureName) => EndTextCommandThefeedPostStats(statTitle, (int)iconEnum, stepVal, barValue, isImportant, picTextureDict, picTextureName);
        public static int DisplayFeedPostMessageTextUpdate(string textureDict, string textureName, bool flash, int iconType, string nameText, string subtitleText, float duration) => EndTextCommandThefeedPostMessagetextTu(textureDict, textureName, flash, iconType, nameText, subtitleText, duration);
        public static int DisplayFeedPostMessageTextUpdate(string textureDict, string textureName, bool flash, IconType iconType, string nameText, string subtitleText, float duration) => EndTextCommandThefeedPostMessagetextTu(textureDict, textureName, flash, (int)iconType, nameText, subtitleText, duration);
        #endregion

        #region Notifications
        public static string SuccessNotification(string message)
        {
            ShowNotification($"~g~~h~Success~h~~s~: {message}", true);
            return message;
        }

        public static string ErrorNotification(string message)
        {
            ShowNotification($"~r~~h~Error~h~~s~: {message}", true);
            return message; 
        }

        public static string AlertNotification(string message)
        {
            ShowNotification($"~y~~h~Alert~h~~s~: {message}", true);
            return message;
        }

        public static void DisplayNotification(string message)
        {
            SetNotificationTextEntry("STRING");
            AddTextComponentString(message);
            EndTextCommandThefeedPostTicker(false, false);
        }
        #endregion

        #region Rectangles
        public static void DrawRectangle(float x, float y, float width, float height) => DrawRectangle(x, y, width, height, 3, 188, 255);
        public static void DrawRectangle(float x, float y, float width, float height, int r, int g, int b, int a = 255) => DrawRect(x, y, width, height, r, g, b, a);
        #endregion

        #region Drawing Text
        // forked from https://forum.cfx.re/t/draw-3d-text-as-marker/2643565 by JoeyTheDev (Converted to C#) 
        public static void DrawText3d(float x, float y, float z, string text, float size, int r, int g, int b, int a = 255)
        {
            Vector3 PlayerPos = Game.PlayerPed.Position;

            float screenXPos = 0.0f;
            float screenYPos = 0.0f;

            if (GetDistanceBetweenCoords(x, y, z, PlayerPos.X, PlayerPos.Y, PlayerPos.Z, true) < 5.0f)
            {
                World3dToScreen2d(x, y, z, ref screenXPos, ref screenYPos);
                SetTextScale(0.0f, size);
                SetTextFont(0);
                SetTextColour(r, g, b, a);
                SetTextDropshadow(0, 0, 0, 0, 255);
                SetTextDropShadow();
                SetTextOutline();
                SetTextEntry("STRING");
                SetTextCentre(true);
                AddTextComponentString(text);
                DrawText(screenXPos, screenYPos);
                DrawRect(screenXPos, screenYPos + 0.125f, (float)text.Length / 300, 0.03f, 23, 23, 23, 70);
            }
        }

        public static void DrawText2d(float x, float y, float size, string text, int r, int g, int b, int a = 255)
        {
            float referenceScreenWidth = 1920f;
            float referenceScreenHeight = 1080f;

            float screenWidth = Resolution.Width;
            float screenHeight = Resolution.Height;

            float safezoneOffsetX = (referenceScreenWidth - screenWidth) / 2.0f;
            float safezoneOffsetY = (referenceScreenHeight - screenHeight) / 2.0f;

            float scaleX = screenWidth / referenceScreenWidth;
            float scaleY = screenHeight / referenceScreenHeight;

            x += safezoneOffsetX;
            y += safezoneOffsetY;

            size *= Math.Min(scaleX, scaleY);

            SetTextScale(1.0f, size);
            SetTextColour(r, g, b, a);
            SetTextDropShadow();
            SetTextOutline();
            SetTextEntry("STRING");
            DrawText(x, y);
        }

        public static void DrawText2d(float x, float y, float size, string text) => DrawText2d(x, y, size, text, 255, 255, 255, 255);

        public static void DisplayHelpText(string text) => DisplayHelpTextThisFrame(text);
        
        public static void Draw2dLine(float pos1X, float pos1Y, float pos2X, float pos2Y, float width, int r, int g, int b, int a = 255) => DrawLine_2d(pos1X, pos1Y, pos2X, pos2Y, width, r, g, b, a);
        public static void Draw2dLine(Vector2 pos1, Vector2 pos2, float width, int r, int g, int b, int a = 255) => DrawLine_2d(pos1.X, pos1.Y, pos2.X, pos2.Y, width, r, g, b, a);
        #endregion

        #region User Input

        #endregion

        public static async void RequestTextureDictionary(string textureDict)
        {
            RequestStreamedTextureDict(textureDict, true);
            while (!HasStreamedTextureDictLoaded(textureDict))
            {
                await Delay(0);
            }
        }

        public static async void RequestTextureDict(string textureDict) => RequestTextureDictionary(textureDict);
    }
}
