using CitizenFX.Core;
using CitizenFX.Core.Native;
using static CitizenFX.Core.Native.API;

namespace Red.Common.Client
{
    public class Controls
    {
        public static int UpdateKeyboard() => UpdateOnscreenKeyboard();
        public static int TimeSinceLastInput(int inputGroup = 0) => GetTimeSinceLastInput(inputGroup);
        public static int TimeSinceLastInput(ControlType inputGroup) => TimeSinceLastInput((int)inputGroup);

        public static void EnableKeyAction(int key, int inputGroup = 0, bool enableKey = true) => EnableControlAction(key, inputGroup, enableKey);
        public static void EnableKeyAction(Control key, int inputGroup, bool enableKey) => EnableKeyAction((int)key, inputGroup, enableKey);
        public static void EnableKeyAction(KeyboardKeys key, int inputGroup, bool enableKey) => EnableKeyAction((Control)key, inputGroup, enableKey);
        public static void EnableKeyAction(int key, ControlType inputGroup = ControlType.Player, bool enableKey = true) => EnableKeyAction(key, (int)inputGroup, enableKey);
        public static void EnableKeyAction(Control key, ControlType inputGroup = ControlType.Player, bool enableKey = true) => EnableKeyAction(key, (int)inputGroup, enableKey);
        public static void EnableKeyAction(KeyboardKeys key, ControlType inputGroup = ControlType.Player, bool enableKey = false) => EnableKeyAction(key, (int)inputGroup, enableKey);

        public static void DisableKeyAction(int key, int inputGroup = 0, bool disableKey = true) => DisableControlAction(inputGroup, key, disableKey);
        public static void DisableKeyAction(Control key, int inputGroup, bool disableKey) => DisableKeyAction((int)key, inputGroup, disableKey);
        public static void DisableKeyAction(KeyboardKeys key, int inputGroup, bool disableKey = true) => DisableKeyAction((Control)key, inputGroup, disableKey);
        public static void DisableKeyAction(int key, ControlType inputGroup = ControlType.Player, bool disableKey = true) => DisableKeyAction(key, (int)inputGroup, disableKey);
        public static void DisableKeyAction(Control key, ControlType inputGroup = ControlType.Player, bool disableKey = true) => DisableKeyAction(key, (int)inputGroup, disableKey);
        public static void DisableKeyAction(KeyboardKeys key, ControlType inputGroup = ControlType.Player, bool disableKey = true) => DisableKeyAction(key, (int)inputGroup, disableKey);

        public static void DisableAllKeyActions(int inputGroup) => DisableAllControlActions(inputGroup);
        public static void DisableAllKeyActions(ControlType inputGroup) => DisableAllKeyActions((int)inputGroup);

        public static bool SetKeyNormal(int key, float amount, int inputGroup = 0) => SetControlNormal(inputGroup, key, amount);
        public static bool SetKeyNormal(Control key, float amount, int inputGroup = 0) => SetKeyNormal((int)key, amount, inputGroup);
        public static bool SetKeyNormal(Control key, float amount, ControlType inputGroup = 0) => SetKeyNormal((int)key, amount, (int)inputGroup);

        public static bool IsKeyEnabled(int key, int inputGroup = 0) => Game.IsControlEnabled(inputGroup, (Control)key) && Game.CurrentInputMode == InputType.Keyboard && UpdateKeyboard() != (int)OnScreenStatus.Editing;
        public static void IsKeyEnabled(Control key, int inputGroup = 0) => IsKeyEnabled((int)key, inputGroup);
        public static void IsKeyEnabled(KeyboardKeys key, int inputGroup = 0) => IsKeyEnabled((Control)key, inputGroup);
        public static void IsKeyEnabled(int key, ControlType inputGroup = ControlType.Player) => IsKeyEnabled(key, (int)inputGroup);
        public static void IsKeyEnabled(Control key, ControlType inputGroup = ControlType.Player) => IsKeyEnabled(key, (int)inputGroup);
        public static void IsKeyEnabled(KeyboardKeys key, ControlType inputGroup = ControlType.Player) => IsKeyEnabled(key, (int)inputGroup);

        public static bool IsKeyPressed(int key, int inputGroup = 0) => Game.IsControlPressed(inputGroup, (Control)key) && Game.CurrentInputMode == InputType.Keyboard;
        public static bool IsKeyPressed(Control key) => IsKeyPressed(key, ControlType.Player);
        public static bool IsKeyPressed(Control key, int inputGroup = 0) => IsKeyPressed((int)key, inputGroup);
        public static bool IsKeyPressed(KeyboardKeys key, int keyGroup = 0) => IsKeyPressed((Control)key, keyGroup);
        public static bool IsKeyPressed(int key, ControlType inputGroup = ControlType.Player) => IsKeyPressed(key, (int)inputGroup);
        public static bool IsKeyPressed(Control key, ControlType inputGroup = ControlType.Player) => IsKeyPressed(key, (int)inputGroup);
        public static bool IsKeyPressed(KeyboardKeys key, ControlType inputGroup = ControlType.Player) => IsKeyPressed(key, (int)inputGroup);

        public static bool IsDisableKeyJustPressed(int key, int inputGroup = 0) => Game.IsDisabledControlJustPressed(inputGroup, (Control)key) && Game.CurrentInputMode == InputType.Keyboard;
        public static bool IsDisableKeyJustPressed(Control key) => IsDisableKeyJustPressed(key, ControlType.Player);
        public static bool IsDisableKeyJustPressed(Control key, int inputGroup = 0) => IsDisableKeyJustPressed((int)key, inputGroup);
        public static bool IsDisableKeyJustPressed(KeyboardKeys key, int inputGroup = 0) => IsDisableKeyJustPressed((Control)key, inputGroup);
        public static bool IsDisableKeyJustPressed(int key, ControlType inputGroup = ControlType.Player) => IsDisableKeyJustPressed(key, (int)inputGroup);
        public static bool IsDisableKeyJustPressed(Control key, ControlType inputGroup = ControlType.Player) => IsDisableKeyJustPressed(key, (int)inputGroup);
        public static bool IsDisableKeyJustPressed(KeyboardKeys key, ControlType inputGroup = ControlType.Player) => IsDisableKeyJustPressed(key, (int)inputGroup);

        public static bool IsDisabledKeyJustReleased(int key, int inputGroup = 0) => Game.IsDisabledControlJustReleased(inputGroup, (Control)key) && Game.CurrentInputMode == InputType.Keyboard && UpdateKeyboard() != (int)OnScreenStatus.Editing;
        public static bool IsDisabledKeyJustReleased(Control key) => IsDisabledKeyJustReleased(key, ControlType.Player);
        public static bool IsDisabledKeyJustReleased(Control key, int inputGroup = 0) => IsDisabledKeyJustReleased((int)key, inputGroup);
        public static bool IsDisabledKeyJustReleased(KeyboardKeys key, int inputGroup = 0) => IsDisabledKeyJustReleased((Control)key, inputGroup);
        public static bool IsDisabledKeyJustReleased(int key, ControlType inputGroup = ControlType.Player) => IsDisabledKeyJustReleased(key, (int)inputGroup);
        public static bool IsDisabledKeyJustReleased(Control key, ControlType inputGroup = ControlType.Player) => IsDisabledKeyJustReleased(key, (int)inputGroup);
        public static bool IsDisabledKeyJustReleased(KeyboardKeys key, ControlType inputGroup = ControlType.Player) => IsDisabledKeyJustReleased(key, (int)inputGroup);

        public static bool IsKeyJustPressed(int key, int inputGroup = 0) => Game.IsControlJustPressed(inputGroup, (Control)key) && Game.CurrentInputMode == InputType.Keyboard;
        public static bool IsKeyJustPressed(Control key) => IsKeyJustPressed((int)key, ControlType.Player);
        public static bool IsKeyJustPressed(Control key, int inputGroup = 0) => IsKeyJustPressed((int)key, inputGroup);
        public static bool IsKeyJustPressed(KeyboardKeys key, int inputGroup = 0) => IsKeyJustPressed((Control)key, inputGroup);
        public static bool IsKeyJustPressed(int key, ControlType inputGroup = ControlType.Player) => IsKeyJustPressed(key, (int)inputGroup);
        public static bool IsKeyJustPressed(Control key, ControlType inputGroup = ControlType.Player) => IsKeyJustPressed(key, (int)inputGroup);
        public static bool IsKeyJustPressed(KeyboardKeys key, ControlType inputGroup = ControlType.Player) => IsKeyJustPressed(key, (int)inputGroup);

        public static bool IsKeyJustPressedRegardlessOfDisabled(int key, int inputGroup = 0) => IsKeyPressed(key, inputGroup) || IsDisabledKeyJustReleased(key, inputGroup) && Game.CurrentInputMode == InputType.Keyboard && UpdateKeyboard() != (int)OnScreenStatus.Editing;
        public static bool IsKeyJustPressedRegardlessOfDisabled(Control key) => IsKeyPressed(key, ControlType.Player);
        public static bool IsKeyJustPressedRegardlessOfDisabled(Control key, int inputGroup = 0) => IsKeyJustPressedRegardlessOfDisabled((int)key, inputGroup);
        public static bool IsKeyJustPressedRegardlessOfDisabled(KeyboardKeys key, int inputGroup = 0) => IsKeyJustPressedRegardlessOfDisabled((Control)key, inputGroup);
        public static bool IsKeyJustPressedRegardlessOfDisabled(int key, ControlType inputGroup = ControlType.Player) => IsKeyJustPressedRegardlessOfDisabled(key, (int)inputGroup);
        public static bool IsKeyJustPressedRegardlessOfDisabled(Control key, ControlType inputGroup = ControlType.Player) => IsKeyJustPressedRegardlessOfDisabled(key, (int)inputGroup);
        public static bool IsKeyJustPressedRegardlessOfDisabled(KeyboardKeys key, ControlType inputGroup = ControlType.Player) => IsKeyJustPressedRegardlessOfDisabled(key, (int)inputGroup);

        public static bool IsButtonEnabled(int button, int inputGroup = 0) => Game.IsControlEnabled(inputGroup, (Control)button) && Game.CurrentInputMode == InputType.Controller;
        public static void IsButtonEnabled(Control button) => IsButtonEnabled((int)button, ControlType.Player);
        public static void IsButtonEnabled(Control button, int inputGroup) => IsButtonEnabled((int)button, inputGroup);
        public static void IsButtonEnabled(ControllerButtons button, int inputGroup) => IsButtonEnabled((Control)button, inputGroup);
        public static void IsButtonEnabled(int button, ControlType inputGroup = ControlType.Player) => IsButtonEnabled(button, (int)inputGroup);
        public static void IsButtonEnabled(Control button, ControlType inputGroup = ControlType.Player) => IsButtonEnabled(button, (int)inputGroup);
        public static void IsButtonEnabled(ControllerButtons button, ControlType inputGroup = ControlType.Player) => IsButtonEnabled(button, (int)inputGroup);

        public static bool IsButtonPressed(int button, int inputGroup = 0) => Game.IsControlPressed(inputGroup, (Control)button) && Game.CurrentInputMode == InputType.Controller;
        public static bool IsButtonPressed(Control button) => IsButtonPressed((int)button, ControlType.Player);
        public static bool IsButtonPressed(Control button, int inputGroup = 0) => IsButtonPressed((int)button, inputGroup);
        public static bool IsButtonPressed(ControllerButtons button, int keyGroup = 0) => IsButtonPressed((Control)button, keyGroup);
        public static bool IsButtonPressed(int button, ControlType inputGroup = ControlType.Player) => IsButtonPressed(button, (int)inputGroup);
        public static bool IsButtonPressed(Control button, ControlType inputGroup = ControlType.Player) => IsButtonPressed(button, (int)inputGroup);
        public static bool IsButtonPressed(ControllerButtons button, ControlType inputGroup = ControlType.Player) => IsButtonPressed(button, (int)inputGroup);

        public static bool IsDisableButtonJustPressed(int button, int inputGroup = 0) => Game.IsDisabledControlJustPressed(inputGroup, (Control)button) && Game.CurrentInputMode == InputType.Controller;
        public static bool IsDisableButtonJustPressed(Control button) => IsDisableButtonJustPressed(button, ControlType.Player);
        public static bool IsDisableButtonJustPressed(Control button, int inputGroup = 0) => IsDisableButtonJustPressed((int)button, inputGroup);
        public static bool IsDisableButtonJustPressed(ControllerButtons button, int inputGroup = 0) => IsDisableButtonJustPressed((Control)button, inputGroup);
        public static bool IsDisableButtonJustPressed(int button, ControlType inputGroup = ControlType.Player) => IsDisableButtonJustPressed(button, (int)inputGroup);
        public static bool IsDisableButtonJustPressed(Control button, ControlType inputGroup = ControlType.Player) => IsDisableButtonJustPressed(button, (int)inputGroup);
        public static bool IsDisableButtonJustPressed(ControllerButtons button, ControlType inputGroup = ControlType.Player) => IsDisableButtonJustPressed(button, (int)inputGroup);

        public static bool IsDisabledButtonJustReleased(int button, int inputGroup = 0) => Game.IsDisabledControlJustReleased(inputGroup, (Control)button) && Game.CurrentInputMode == InputType.Controller;
        public static bool IsDisabledButtonJustReleased(Control button) => IsDisabledButtonJustReleased(button, ControlType.Player);
        public static bool IsDisabledButtonJustReleased(Control button, int inputGroup = 0) => IsDisabledButtonJustReleased((int)button, inputGroup);
        public static bool IsDisabledButtonJustReleased(ControllerButtons button, int inputGroup = 0) => IsDisabledButtonJustReleased((Control)button, inputGroup);
        public static bool IsDisabledButtonJustReleased(int button, ControlType inputGroup = ControlType.Player) => IsDisabledButtonJustReleased(button, (int)inputGroup);
        public static bool IsDisabledButtonJustReleased(Control button, ControlType inputGroup = ControlType.Player) => IsDisabledButtonJustReleased(button, (int)inputGroup);
        public static bool IsDisabledButtonJustReleased(ControllerButtons button, ControlType inputGroup = ControlType.Player) => IsDisabledButtonJustReleased(button, (int)inputGroup);

        public static bool IsButtonJustPressed(int button, int inputGroup = 0) => Game.IsControlJustPressed(inputGroup, (Control)button) && Game.CurrentInputMode == InputType.Controller;
        public static bool IsButtonJustPressed(Control button) => IsButtonJustPressed(button, ControlType.Player);
        public static bool IsButtonJustPressed(Control button, int inputGroup = 0) => IsButtonJustPressed((int)button, inputGroup);
        public static bool IsButtonJustPressed(ControllerButtons button, int inputGroup = 0) => IsButtonJustPressed((Control)button, inputGroup);
        public static bool IsButtonJustPressed(int button, ControlType inputGroup = ControlType.Player) => IsButtonJustPressed(button, (int)inputGroup);
        public static bool IsButtonJustPressed(Control button, ControlType inputGroup = ControlType.Player) => IsButtonJustPressed(button, (int)inputGroup);
        public static bool IsButtonJustPressed(ControllerButtons button, ControlType inputGroup = ControlType.Player) => IsButtonJustPressed(button, (int)inputGroup);

        public static bool IsButtonJustPressedRegardlessOfDisabled(int button, int inputGroup = 0) => IsKeyPressed(button, inputGroup) || IsDisabledKeyJustReleased(button, inputGroup) && Game.CurrentInputMode == InputType.Controller;
        public static bool IsButtonJustPressedRegardlessOfDisabled(Control button) => IsButtonJustPressedRegardlessOfDisabled(button, ControlType.Player);
        public static bool IsButtonJustPressedRegardlessOfDisabled(Control button, int inputGroup = 0) => IsButtonJustPressedRegardlessOfDisabled((int)button, inputGroup);
        public static bool IsButtonJustPressedRegardlessOfDisabled(ControllerButtons button, int inputGroup = 0) => IsButtonJustPressedRegardlessOfDisabled((Control)button, inputGroup);
        public static bool IsButtonJustPressedRegardlessOfDisabled(int button, ControlType inputGroup = ControlType.Player) => IsButtonJustPressedRegardlessOfDisabled(button, (int)inputGroup);
        public static bool IsButtonJustPressedRegardlessOfDisabled(Control button, ControlType inputGroup = ControlType.Player) => IsButtonJustPressedRegardlessOfDisabled(button, (int)inputGroup);
        public static bool IsButtonJustPressedRegardlessOfDisabled(ControllerButtons button, ControlType inputGroup = ControlType.Player) => IsButtonJustPressedRegardlessOfDisabled(button, (int)inputGroup);

        public static void EnableMouseAction(MouseInput key) => EnableControlAction((int)ControlType.Player, (int)key, true);
        public static void DisableMouseAction(MouseInput key) => DisableControlAction((int)ControlType.Player, (int)key, true);
        public static void DisableAllMouseActions() => DisableAllControlActions((int)ControlType.Player);

        public static bool SetMouseButtonNormal(MouseInput key, float amount) => SetControlNormal((int)ControlType.Player, (int)key, amount) && Game.CurrentInputMode == InputType.Keyboard;
        public static bool IsMouseButtonEnabled(MouseInput key) => Game.IsControlEnabled(0, (Control)key) && Game.CurrentInputMode == InputType.Keyboard;
        public static bool IsMouseButtonPressed(MouseInput key) => Game.IsControlPressed(0, (Control)key) && Game.CurrentInputMode == InputType.Keyboard;
        public static bool IsDisableMouseButtonJustPressed(MouseInput key) => Game.IsDisabledControlJustPressed(0, (Control)key) && Game.CurrentInputMode == InputType.Keyboard;
        public static bool IsDisabledMouseButtonJustReleased(MouseInput key) => Game.IsDisabledControlJustReleased(0, (Control)key) && Game.CurrentInputMode == InputType.Keyboard;
        public static bool IsMouseButtonJustPressed(MouseInput key) => Game.IsControlJustPressed(0, (Control)key) && Game.CurrentInputMode == InputType.Keyboard;
        public static bool IsMouseButtonJustPressedRegardlessOfDiable(MouseInput key) => IsMouseButtonPressed(key) || IsDisabledMouseButtonJustReleased(key) && Game.CurrentInputMode == InputType.Keyboard;

        public static bool LastInputWasController()
        {
            return Function.Call<bool>(Hash._IS_USING_KEYBOARD, 2);
        }
    }
    
    public class InputType
    {
        public static InputMode Keyboard = InputMode.MouseAndKeyboard;
        public static InputMode Controller = InputMode.GamePad;
    }

    // Made from the FiveM Control List (https://docs.fivem.net/docs/game-references/controls/)
    public enum KeyboardKeys
    {
        Z = 20,
        V = 0,
        S = 8,
        D = 9,
        F = 23,
        C = 26,
        B = 29,
        W = 32,
        A = 34,
        Q = 44,
        R = 45,
        G = 47,
        H = 74,
        E = 38,
        M = 244,
        T = 245,
        Y = 246,
        N = 249,
        K = 311,
        PageUp = 10,
        PageDown = 11,
        LeftAlt = 19,
        LeftShift = 21,
        Spacebar = 22,
        LeftControl = 36,
        Tab = 37,
        OpenBracket = 39,
        ClosedBracket = 40,
        Period = 81,
        Comma = 82,
        Equals = 83,
        Minus = 84,
        X = 105,
        NumbPad6 = 107,
        NumbPad4 = 108,
        NumbPad5 = 110,
        NumbPad8 = 112,
        NumbPad7 = 117,
        NumbPad9 = 118,
        NumpadPlus = 314,
        NumpadMinus = 315,
        Insert = 121,
        CapsLock = 137,
        One = 157,
        Two = 158,
        Six = 159,
        Three = 160,
        Seven = 161,
        Eight = 162,
        Nine = 163,
        Four = 164,
        Five = 165,
        F1 = 288,
        F2 = 289,
        F5 = 166,
        F6 = 167,
        F7 = 168,
        F8 = 169,
        F3 = 170,
        F9 = 56,
        F10 = 57,
        ArrowUp = 172,
        ArrowDown = 173,
        ArrowLeft = 174,
        ArrowRight = 175,
        Delete = 178,
        L = 182,
        Enter = 191,
        P = 199,
        Escape = 200,
        Home = 212,
        Tilde = 243,
        ForwardSlash = 243,
        BackTick = 243,
    }
    // Made from the FiveM Control List (https://docs.fivem.net/docs/game-references/controls/) 
    public enum MouseInput
    {
        MouseRight = 1,
        MouseDown = 2,
        MouseWheelDown = 14,
        MouseWheelUp = 15,
        LeftMouseButton = 24,
        RightMouseButton = 25,
        ScrollWheelButton = 348
    }
    // Made from the FiveM Control List (https://docs.fivem.net/docs/game-references/controls/) 
    public enum ControllerButtons
    {
        PlayStationTriagle = 56,
        PlayStationSquare = 296,
        PlayStationCross = 298,
        PlayStationCircle = 225,
        PlayStationR1 = 90,
        PlayStationR2 = 129,
        PlayStationL1 = 185,
        PlayStationL2 = 207,
        PlayStationOptions = 318,
        PlayStationShare = 310,
        PlayStationDPadUp = 42,
        PlayStationDPadDown = 43,
        PlayStationDPadLeft = 47,
        PlayStationDPadRight = 46,
        PlayStationLeftStick = 28,
        PlayStationRightStick = 26,
        XboxY = 49,
        XboxX = 22,
        XboxA = 18,
        XboxB = 45,
        XboxBack = 0,
        XboxStart = 199,
        XboxRightButton = 69,
        XboxLeftButton = 89,
        XboxRightTrigger = 10,
        XboxLeftTrigger = 11,
        XboxLeftStick = 8,
        XboxRightStick = 12,
    }
    // Made from the FiveM Control List (https://docs.fivem.net/docs/game-references/controls/) 
    public enum ControlType
    {
        Player = 0,
        Unknown = 1,
        FrontEnd = 2
    }
}
