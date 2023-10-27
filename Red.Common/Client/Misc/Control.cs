using CitizenFX.Core;
using CitizenFX.Core.Native;
using static CitizenFX.Core.Native.API;

namespace Red.Common.Client.Misc
{
    public class Controls
    {
        public static bool IsOnScreenKeyboardActive() => UpdateOnscreenKeyboard() == 3;
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

        public static bool IsButtonEnabled(int key, int inputGroup = 0) => Game.IsControlEnabled(inputGroup, (Control)key) && Game.CurrentInputMode == InputType.Controller;
        public static void IsButtonEnabled(Control key) => IsButtonEnabled((int)key, ControlType.Player);
        public static void IsButtonEnabled(Control key, int inputGroup) => IsButtonEnabled((int)key, inputGroup);
        public static void IsButtonEnabled(KeyboardKeys key, int inputGroup) => IsButtonEnabled((Control)key, inputGroup);
        public static void IsButtonEnabled(int key, ControlType inputGroup = ControlType.Player) => IsButtonEnabled(key, (int)inputGroup);
        public static void IsButtonEnabled(Control key, ControlType inputGroup = ControlType.Player) => IsButtonEnabled(key, (int)inputGroup);
        public static void IsButtonEnabled(KeyboardKeys key, ControlType inputGroup = ControlType.Player) => IsButtonEnabled(key, (int)inputGroup);

        public static bool IsButtonPressed(int key, int inputGroup = 0) => Game.IsControlPressed(inputGroup, (Control)key) && Game.CurrentInputMode == InputType.Controller;
        public static bool IsButtonPressed(Control key) => IsButtonPressed((int)key, ControlType.Player);
        public static bool IsButtonPressed(Control key, int inputGroup = 0) => IsButtonPressed((int)key, inputGroup);
        public static bool IsButtonPressed(KeyboardKeys key, int keyGroup = 0) => IsButtonPressed((Control)key, keyGroup);
        public static bool IsButtonPressed(int key, ControlType inputGroup = ControlType.Player) => IsButtonPressed(key, (int)inputGroup);
        public static bool IsButtonPressed(Control key, ControlType inputGroup = ControlType.Player) => IsButtonPressed(key, (int)inputGroup);
        public static bool IsButtonPressed(KeyboardKeys key, ControlType inputGroup = ControlType.Player) => IsButtonPressed(key, (int)inputGroup);

        public static bool IsDisableButtonJustPressed(int key, int inputGroup = 0) => Game.IsDisabledControlJustPressed(inputGroup, (Control)key) && Game.CurrentInputMode == InputType.Controller;
        public static bool IsDisableButtonJustPressed(Control key) => IsDisableButtonJustPressed(key, ControlType.Player);
        public static bool IsDisableButtonJustPressed(Control key, int inputGroup = 0) => IsDisableButtonJustPressed((int)key, inputGroup);
        public static bool IsDisableButtonJustPressed(KeyboardKeys key, int inputGroup = 0) => IsDisableButtonJustPressed((Control)key, inputGroup);
        public static bool IsDisableButtonJustPressed(int key, ControlType inputGroup = ControlType.Player) => IsDisableButtonJustPressed(key, (int)inputGroup);
        public static bool IsDisableButtonJustPressed(Control key, ControlType inputGroup = ControlType.Player) => IsDisableButtonJustPressed(key, (int)inputGroup);
        public static bool IsDisableButtonJustPressed(KeyboardKeys key, ControlType inputGroup = ControlType.Player) => IsDisableButtonJustPressed(key, (int)inputGroup);

        public static bool IsDisabledButtonJustReleased(int key, int inputGroup = 0) => Game.IsDisabledControlJustReleased(inputGroup, (Control)key) && Game.CurrentInputMode == InputType.Controller;
        public static bool IsDisabledButtonJustReleased(Control key) => IsDisabledButtonJustReleased(key, ControlType.Player);
        public static bool IsDisabledButtonJustReleased(Control key, int inputGroup = 0) => IsDisabledButtonJustReleased((int)key, inputGroup);
        public static bool IsDisabledButtonJustReleased(KeyboardKeys key, int inputGroup = 0) => IsDisabledButtonJustReleased((Control)key, inputGroup);
        public static bool IsDisabledButtonJustReleased(int key, ControlType inputGroup = ControlType.Player) => IsDisabledButtonJustReleased(key, (int)inputGroup);
        public static bool IsDisabledButtonJustReleased(Control key, ControlType inputGroup = ControlType.Player) => IsDisabledButtonJustReleased(key, (int)inputGroup);
        public static bool IsDisabledButtonJustReleased(KeyboardKeys key, ControlType inputGroup = ControlType.Player) => IsDisabledButtonJustReleased(key, (int)inputGroup);

        public static bool IsButtonJustPressed(int key, int inputGroup = 0) => Game.IsControlJustPressed(inputGroup, (Control)key) && Game.CurrentInputMode == InputType.Controller;
        public static bool IsButtonJustPressed(Control key) => IsButtonJustPressed(key, ControlType.Player);
        public static bool IsButtonJustPressed(Control key, int inputGroup = 0) => IsButtonJustPressed((int)key, inputGroup);
        public static bool IsButtonJustPressed(KeyboardKeys key, int inputGroup = 0) => IsButtonJustPressed((Control)key, inputGroup);
        public static bool IsButtonJustPressed(int key, ControlType inputGroup = ControlType.Player) => IsButtonJustPressed(key, (int)inputGroup);
        public static bool IsButtonJustPressed(Control key, ControlType inputGroup = ControlType.Player) => IsButtonJustPressed(key, (int)inputGroup);
        public static bool IsButtonJustPressed(KeyboardKeys key, ControlType inputGroup = ControlType.Player) => IsButtonJustPressed(key, (int)inputGroup);

        public static bool IsButtonJustPressedRegardlessOfDisabled(int key, int inputGroup = 0) => IsKeyPressed(key, inputGroup) || IsDisabledKeyJustReleased(key, inputGroup) && Game.CurrentInputMode == InputType.Controller;
        public static bool IsButtonJustPressedRegardlessOfDisabled(Control key) => IsButtonJustPressedRegardlessOfDisabled(key, ControlType.Player);
        public static bool IsButtonJustPressedRegardlessOfDisabled(Control key, int inputGroup = 0) => IsButtonJustPressedRegardlessOfDisabled((int)key, inputGroup);
        public static bool IsButtonJustPressedRegardlessOfDisabled(KeyboardKeys key, int inputGroup = 0) => IsButtonJustPressedRegardlessOfDisabled((Control)key, inputGroup);
        public static bool IsButtonJustPressedRegardlessOfDisabled(int key, ControlType inputGroup = ControlType.Player) => IsButtonJustPressedRegardlessOfDisabled(key, (int)inputGroup);
        public static bool IsButtonJustPressedRegardlessOfDisabled(Control key, ControlType inputGroup = ControlType.Player) => IsButtonJustPressedRegardlessOfDisabled(key, (int)inputGroup);
        public static bool IsButtonJustPressedRegardlessOfDisabled(KeyboardKeys key, ControlType inputGroup = ControlType.Player) => IsButtonJustPressedRegardlessOfDisabled(key, (int)inputGroup);

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
}
