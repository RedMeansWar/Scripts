using CitizenFX.Core;
using CitizenFX.Core.UI;
using System;
using System.Threading.Tasks;
using static CitizenFX.Core.Native.API;

namespace Red.Common.Client.Hud
{
    public class HUD : ClientScript
    {
        /// <summary>
        /// Toggles the clients HUD
        /// </summary>
        /// <param name="display"></param>
        public static void DisplayHUD(bool display = true) => DisplayHud(display);
        /// <summary>
        /// Shortened version of Screen.Hud.IsVisable
        /// </summary>
        public static bool HUDIsVisible => Screen.Hud.IsVisible;
        /// <summary>
        /// Determines if the HUD is hidden another version of IsHudHidden
        /// </summary>
        /// <returns></returns>
        public static bool IsHUDHidden() => IsHudHidden();
        #region Notifications
        /// <summary>
        /// Gives a success notification using bold, green, and white text
        /// </summary>
        /// <param name="message"></param>
        /// <param name="blink"></param>
        /// <returns></returns>
        public static string SuccessNotification(string message, bool blink = true)
        {
            Screen.ShowNotification($"~g~~h~Success~h~~s~: {message}", blink);
            return message;
        }
        /// <summary>
        /// Gives a error notification using bold, red, and white text
        /// </summary>
        /// <param name="message"></param>
        /// <param name="blink"></param>
        /// <returns></returns>
        public static string ErrorNotification(string message, bool blink = true)
        {
            Screen.ShowNotification($"~r~~h~Error~h~~s~: {message}", blink);
            return message;
        }
        /// <summary>
        /// Gives a alert notification using bold, yello, and white text
        /// </summary>
        /// <param name="message"></param>
        /// <param name="blink"></param>
        /// <returns></returns>
        public static string AlertNotification(string message, bool blink = true)
        {
            Screen.ShowNotification($"~y~~h~Alert~h~~s~: {message}", blink);
            return message;
        }
        /// <summary>
        /// Draws a notification, shorten down version of Screen.ShowNotification();
        /// </summary>
        /// <param name="message"></param>
        /// <param name="blink"></param>
        /// <returns></returns>
        public static string DisplayNotification(string message, bool blink = false)
        {
            Screen.ShowNotification(message, blink);
            return message;
        }
        /// <summary>
        /// Draws an image notification
        /// </summary>
        /// <param name="picture"></param>
        /// <param name="icon"></param>
        /// <param name="title"></param>
        /// <param name="subtitle"></param>
        /// <param name="message"></param>
        public static void DrawImageNotification(string picture, int icon, string title, string subtitle, string message)
        {
            SetNotificationTextEntry("STRING");
            AddTextComponentString(message);
            SetNotificationMessage(picture, picture, true, icon, title, subtitle);
        }
        #endregion

        #region Rectangles
        /// <summary>
        /// Draws a rectangle on the screen that keeps its position when adjusting the safezone size or aspect ratio
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="r"></param>
        /// <param name="g"></param>
        /// <param name="b"></param>
        /// <param name="a"></param>
        public static void DrawRectangle(float x, float y, float width, float height, int r = 255, int g = 255, int b = 255, int a = 255)
        {
            // Fetch anchor for relative rectangle positioning
            Minimap anchor = Minimap.GetMinimapAnchor();
            
            // Draw rectangle with position based off player's anchor.
            DrawRect(anchor.LeftX + x + (width / 2), anchor.BottomY - y + (height / 2), width, height, r, g, b, a);
        }
        /// <summary>
        /// Shortened version of DrawRectangle but only color value in the opacity (a)
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="a"></param>
        public static void DrawRectangle(float x, float y, float width, float height, int a) => DrawRectangle(x, y, width, height, 255, 255, 255, a);
        #endregion

        #region Draw Text
        /// <summary>
        /// Draws text on screen on the 2nd dimension that moves with aspect ratio / safezone.
        /// https://forum.cfx.re/t/solved-text-at-the-top-left-of-the-screen/13080 by Nihonium (converted to C#)
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="size"></param>
        /// <param name="text"></param>
        /// <param name="r"></param>
        /// <param name="g"></param>
        /// <param name="b"></param>
        /// <param name="a"></param>
        /// <param name="alignment"></param>
        public static void DrawText2d(float x, float y, float size, string text, int r = 255, int g = 255, int b = 255, int a = 255, Alignment alignment = Alignment.Left)
        {
            // Fetch anchor for relative text positioning
            Minimap anchor = Minimap.GetMinimapAnchor();

            // Calculate screen coordinates based on anchor, width, and height
            x = anchor.X + anchor.Width * x;
            y = anchor.Y - y; //Invert y-axis for screen drawing

            // Set text fonts, 4 is default. What do the other one looks like?
            SetTextFont(4); // Font ID: 4 (adjustable?)
            SetTextScale(size, size); // Uniform scaling for both horizontal and vertical dimensions

            // Set text color and visual effects
            SetTextColour(r, g, b, a);
            SetTextDropShadow(); // Add a drop shadow for readability
            SetTextOutline(); // Outline the text for better visibility

            // Handle text alignment
            if (alignment == Alignment.Right)
            {
                // Right-Align text wrapping at specific HUD coords
                SetTextWrap(0, x);
                SetTextJustification(2);
            }
            else
            {
                // Center or left align based on value
                SetTextJustification(alignment == Alignment.Center ? 0 : 1);
            }

            // Prepare text for rendering on screen.
            SetTextEntry("STRING");
            AddTextComponentString(text);

            // Draw the text at calculated anchor coords.
            DrawText(x, y);
        }

        public static void DrawText2d(float x, float y, float size, string text) => DrawText2d(x, y, size, text, 255, 255, 255, 255, Alignment.Left);
        public static void DrawText2d(float x, float y, float size, string text, int a) => DrawText2d(x, y, size, text, 255, 255, 255, a);
        public static void DrawText2d(float x, float y, float size, string text, Alignment alignment) => DrawText2d(x, y, size, text, 255, 255, 255, 255, alignment);
        /// <summary>
        /// Creates text on the 3rd dimension that shows up on all client side
        /// forked from https://forum.cfx.re/t/draw-3d-text-as-marker/2643565 by JoeyTheDev (Converted to C#) 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="text"></param>
        /// <param name="size"></param>
        /// <param name="r"></param>
        /// <param name="g"></param>
        /// <param name="b"></param>
        /// <param name="a"></param>
        public static void DrawText3d(float x, float y, float z, string text, float size, float radius, int r, int g, int b, int a = 255)
        {
            // Grab player position for a distance check.
            Vector3 PlayerPos = Game.PlayerPed.Position;

            float screenXPos = 0.0f; // default screen position
            float screenYPos = 0.0f; // default screen position

            // Calculate distance between player position and in-game position.
            if (GetDistanceBetweenCoords(x, y, z, PlayerPos.X, PlayerPos.Y, PlayerPos.Z, true) < radius)
            {
                // Project 3D world coords to a 2D space
                World3dToScreen2d(x, y, z, ref screenXPos, ref screenYPos);

                // Text rendering properties
                SetTextScale(0.0f, size); // custom size scaling (adjustable)
                SetTextFont(0); // Font ID: 0 (adjustable if needed what do other fonts look like?)
                SetTextColour(r, g, b, a);
                SetTextDropshadow(0, 0, 0, 0, 255); // Black drop shadow (hard to tell when viewing it in-game)
                SetTextDropShadow();
                SetTextOutline();

                // Prepare text for rendering
                SetTextEntry("STRING");
                SetTextCentre(true);
                AddTextComponentString(text);

                // Draw text on screen and add a black rectangle around it.
                DrawText(screenXPos, screenYPos);
                DrawRect(screenXPos, screenYPos + 0.125f, (float)text.Length / 300, 0.03f, 23, 23, 23, 70);
            }
        }
        /// <summary>
        /// Shortened version of DisplayHelpTextThisFrame
        /// </summary>
        /// <param name="text"></param>
        public static void DisplayHelpText(string text) => Screen.DisplayHelpTextThisFrame(text);
        /// <summary>
        /// Shortened version of DisplayHelpTextThisFrame
        /// </summary>
        /// <param name="text"></param>
        public static void DrawHelpText(string text) => Screen.DisplayHelpTextThisFrame(text);
        #endregion
        /// <summary>
        /// Draws a texture on the screen.
        /// </summary>
        /// <param name="textureDict"></param>
        public static async void RequestTextureDictionary(string textureDict)
        {
            RequestStreamedTextureDict(textureDict, true);
            while (!HasStreamedTextureDictLoaded(textureDict))
            {
                await Delay(0);
            }
        }
        /// <summary>
        /// Draws a texture on the screen.
        /// </summary>
        /// <param name="textureDict"></param>
        public static async void RequestTextureDict(string textureDict) => RequestTextureDictionary(textureDict);
        /// <summary>
        /// Get a user input text string.
        /// </summary>
        /// <returns></returns>
        public static async Task<string> GetUserInput() => await GetUserInput(null, null, 30);
        /// <summary>
        /// Get a user input text string.
        /// </summary>
        /// <param name="maxInputLength"></param>
        /// <returns></returns>
        public static async Task<string> GetUserInput(int maxInputLength) => await GetUserInput(null, null, maxInputLength);
        /// <summary>
        /// Get a user input text string.
        /// </summary>
        /// <param name="windowTitle"></param>
        /// <returns></returns>
        public static async Task<string> GetUserInput(string windowTitle) => await GetUserInput(windowTitle, null, 30);
        /// <summary>
        /// Get a user input text string.
        /// </summary>
        /// <param name="windowTitle"></param>
        /// <param name="maxInputLength"></param>
        /// <returns></returns>
        public static async Task<string> GetUserInput(string windowTitle, int maxInputLength) => await GetUserInput(windowTitle, null, maxInputLength);
        /// <summary>
        /// Get a user input text string.
        /// </summary>
        /// <param name="windowTitle"></param>
        /// <param name="defaultText"></param>
        /// <returns></returns>
        public static async Task<string> GetUserInput(string windowTitle, string defaultText) => await GetUserInput(windowTitle, defaultText, 30);
        /// <summary>
        /// Get a user input text string.
        /// </summary>
        /// <param name="windowTitle"></param>
        /// <param name="defaultText"></param>
        /// <param name="maxInputLength"></param>
        /// <returns></returns>
        public static async Task<string> GetUserInput(string windowTitle, string defaultText, int maxInputLength)
        {
            // Create the window title with clear formatting
            var spacer = "\t"; // Tab for visual space
            AddTextEntry($"{GetCurrentResourceName().ToUpper()}_WINDOW_TITLE", $"{windowTitle ?? "Enter"}:{spacer}(MAX {maxInputLength} Characters)");

            // Display onscreen keyboard with input constraints based off of what the user wants.
            DisplayOnscreenKeyboard(1, $"{GetCurrentResourceName().ToUpper()}_WINDOW_TITLE", "", defaultText ?? "", "", "", "", maxInputLength);
            await Delay(0); // slightly delay next task. Does this make a difference?

            // Monitor keyboard status until the enter key is pressed
            while (true)
            {
                var keyboardStatus = UpdateOnscreenKeyboard();

                switch (keyboardStatus)
                {
                    case 3: // Input field no longer displayed (unknown cause)
                    case 2: // User cancelled input
                        return null;
                    case 1: // User finished editing
                        return GetOnscreenKeyboardResult(); // Retrieve text
                    default:
                        await Delay(0); // Delay briefly for responsiveness without busy-waiting
                        break;
                }
            }
        }
        /// <summary>
        /// Gets the safezone of the player
        /// </summary>
        /// <returns>The safezone of the player assuming the resolution is 1920x1080 and the aspect ratio is 16:9</returns>
        public static dynamic GetSafeZone()
        {
            float size = 10 - ((float)Math.Round(GetSafeZone(), 2) * 100) - 90;

            dynamic safeZone = new
            {
                X = (int)Math.Round(size * (GetAspectRatio(false) * 5.4)),
                Y = (int)Math.Round(size * 5.4)
            };

            int screenWidth = 0, screenHeight = 0;
            GetScreenResolution(ref screenWidth, ref screenHeight);

            if (screenWidth > 1920)
            {
                safeZone.X += (screenWidth - 1920) / 2;
            }

            return safeZone;
        }
        /// <summary>
        /// Gets the screen resolution
        /// </summary>
        /// <returns>The players Resolution assuming that it's 1920x1080</returns>
        public static dynamic GetResolution()
        {
            dynamic resolution = new
            {
                width = 1080.0 * GetAspectRatio(false),
                height = 1080.0
            };

            return resolution;
        }
    }
}
