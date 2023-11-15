using System;
using System.Threading.Tasks;
using Red.Common.Client.Misc;
using CitizenFX.Core;
using CitizenFX.Core.UI;
using static CitizenFX.Core.Native.API;
using static CitizenFX.Core.UI.Screen;
using System.Reflection.Metadata;
using System.Drawing;
using Mono.CSharp;

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
        public static string SuccessNotification(string message, bool blink = true)
        {
            ShowNotification($"~g~~h~Success~h~~s~: {message}", blink);
            return message;
        }

        public static string ErrorNotification(string message, bool blink = true)
        {
            ShowNotification($"~r~~h~Error~h~~s~: {message}", blink);
            return message; 
        }

        public static string AlertNotification(string message, bool blink = true)
        {
            ShowNotification($"~y~~h~Alert~h~~s~: {message}", blink);
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
        public static void DrawRectangle(float x, float y, float width, float height, int r, int g, int b, int a = 255)
        {
            Minimap anchor = GetMinimapAnchor();
            DrawRect(anchor.LeftX + x + (width / 2), anchor.BottomY - y + (height / 2), width, height, r, g, b, a);
        }

        public static void DrawRectangle(float x, float y, float width, float height) => DrawRectangle(x, y, width, height, 255, 255, 255, 255);
        public static void DrawRectangle(float x, float y, float width, float height, int r = 255, int g = 255, int b = 255) => DrawRectangle(x, y, width, height, r, g, b);
        public static void DrawRectangle(float x, float y, float width, float height, int a = 255) => DrawRectangle(x, y, width, height, 255, 255, 255, a);
        #endregion

        #region Minimap
        // Minimap Anchor made by https://github.com/glitchdetector/fivem-minimap-anchor/blob/master/MINIANCHOR.lua
        // 16:9 Modifications give to by traditionalism https://github.com/traditionalism
        private static Minimap GetMinimapAnchor()
        {
            // 0.05 * ((safezone - 0.9) * 10)
            float safeZoneSize = GetSafeZoneSize();
            float aspectRatio = GetAspectRatio(false);

            float factor1 = 0.05f;
            float factor2 = 0.05f;

            int resX = 1920;
            int resY = 1080;

            if ((double)aspectRatio > 2.0)
            {
                aspectRatio = 1.77777779f; // aspect ratio of 16:9
            }

            GetActiveScreenResolution(ref resX, ref resY);

            float unitX = 1f / resX; 
            float unitY = 1f / resY;
            
            Minimap minimap = new Minimap()
            {
                Width = unitX * (resX / (4f * aspectRatio)),
                Height = unitY * (resY / 5.674f),
                LeftX = unitX * (resX * (factor1 * (Math.Abs(safeZoneSize - 1f) * 10f)))
            };
            if ((double)aspectRatio > 2.0)
            {
                minimap.LeftX += minimap.Width * 0.845f;
                minimap.Width *= 0.76f;
            }
            else if ((double)aspectRatio > 1.7999999523162842)
            {
                minimap.LeftX += minimap.Width * 0.2225f;
                minimap.Width *= 0.995f;
            }

            minimap.BottomY = (float)(1.0f - (double)unitX * (resY * ((double)factor2 * ((double)Math.Abs(safeZoneSize - 1f) * 10.0))));
            minimap.RightX = minimap.LeftX + minimap.Width;
            minimap.TopY = minimap.BottomY - minimap.Height;
            minimap.X = minimap.LeftX;
            minimap.Y = minimap.TopY;
            minimap.XUnit = unitX;
            minimap.YUnit = unitY;

            return minimap;
        }
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

        public static void DrawText2d(float x, float y, float size, string text, int r = 255, int g = 255, int b = 255, int a = 255, Alignment alignment = Alignment.Left) 
        {
            Minimap anchor = GetMinimapAnchor();
            x = anchor.X + anchor.Width * x;
            y = anchor.Y - y;

            SetTextFont(4);
            SetTextScale(size, size);
            SetTextColour(r, g, b, a);
            SetTextDropShadow();
            SetTextOutline();

            if (alignment == Alignment.Right)
            {
                SetTextWrap(0, x);
                SetTextJustification(2);
            }
            else
            {
                SetTextJustification(alignment == Alignment.Center ? 0 : 1);
            }

            SetTextEntry("STRING");
            AddTextComponentString(text);
            DrawText(x, y);
        }

        public static void DrawText2d(float x, float y, float size, string text) => DrawText2d(x, y, size, text);
        public static void DrawText2d(float x, float y, float size, string text, Alignment alignment) => DrawText2d(x, y, size, text, 255, 255, 255, 255, alignment);
        public static void DrawText2d(float x, float y, float size, string text, int a) => DrawText2d(x, y, size, text, 255, 255, 255, a);

        public static void DisplayHelpText(string text) => DisplayHelpTextThisFrame(text);
        
        public static void Draw2dLine(float pos1X, float pos1Y, float pos2X, float pos2Y, float width, int r, int g, int b, int a = 255) => DrawLine_2d(pos1X, pos1Y, pos2X, pos2Y, width, r, g, b, a);
        public static void Draw2dLine(Vector2 pos1, Vector2 pos2, float width, int r, int g, int b, int a = 255) => DrawLine_2d(pos1.X, pos1.Y, pos2.X, pos2.Y, width, r, g, b, a);
        #endregion

        #region User Input
        public static async Task<string> GetUserInput(string windowTitle, string defaultText, int maxInputLength)
        {
            var spacer = "\t";

            AddTextEntry($"{GetCurrentResourceName().ToUpper()}_WINDOW_TITLE", $"{windowTitle ?? "Enter"}:{spacer}(MAX {maxInputLength} Characters)");
            DisplayOnscreenKeyboard(1, $"{GetCurrentResourceName().ToUpper()}_WINDOW_TITLE", "", defaultText ?? "", "", "", "", maxInputLength);

            await Delay(0);

            while (true)
            {
                var keyboardStatus = UpdateOnscreenKeyboard();

                switch (keyboardStatus)
                {
                    case 1: // finished editing
                        return GetOnscreenKeyboardResult();
                    case 2: // cancelled
                    case 3: // not displaying input
                        return null;
                    default:
                        await Delay(0);
                        break;
                }
            }
        }

        public static async Task GetUserInput() => await GetUserInput(null, null, 30);
        public static async Task GetUserInput(int maxInputLength) => await GetUserInput(null, null, maxInputLength);
        public static async Task GetUserInput(string windowTitle) => await GetUserInput(windowTitle, null, 30);
        public static async Task GetUserInput(string windowTitle, int maxInputLength) => await GetUserInput(windowTitle, null, maxInputLength);
        public static async Task GetUserInput(string windowTitle, string defaultText) => await GetUserInput(windowTitle, defaultText, 30);
        #endregion

        #region Texture Dictionary
        public static async void RequestTextureDictionary(string textureDict)
        {
            RequestStreamedTextureDict(textureDict, true);
            while (!HasStreamedTextureDictLoaded(textureDict))
            {
                await Delay(0);
            }
        }

        public static async void RequestTextureDict(string textureDict) => RequestTextureDictionary(textureDict);
        #endregion
    }
}
