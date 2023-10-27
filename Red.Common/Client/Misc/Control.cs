using CitizenFX.Core;
using CitizenFX.Core.Native;
using static CitizenFX.Core.Native.API;

namespace Red.Common.Client.Misc
{
    public class Controls
    {
        #region Methods
        public static bool IsOnScreenKeyboardActive() => UpdateOnscreenKeyboard() == 3;
        public static int UpdateKeyboard() => UpdateOnscreenKeyboard();
        public static int TimeSinceLastInput(int inputGroup = 0) => GetTimeSinceLastInput(inputGroup);
        public static int TimeSinceLastInput(ControlType inputGroup) => TimeSinceLastInput((int)inputGroup);
        #endregion

        #region Control Extensions
        public static bool IsControlPressedRegardlessOfDisabled(Control key) => IsControlPressed(0, (int)key) || IsDisabledControlJustReleased(0, (int)key) && Game.CurrentInputMode == InputMode.MouseAndKeyboard;
        public static bool IsControlPressedRegardlessOfDisabled(Control key, int index) => IsControlPressed(index, (int)key) || IsDisabledControlJustReleased(index, (int)key) && Game.CurrentInputMode == InputMode.MouseAndKeyboard;
        public static bool IsControlPressedRegardlessOfDisabled(int key) => IsControlPressed(0, key) || IsDisabledControlJustReleased(0, key) && Game.CurrentInputMode == InputMode.MouseAndKeyboard;
        public static bool IsControlPressedRegardlessOfDisabled(int key, int index) => IsControlPressed(index, key) || IsDisabledControlJustReleased(index, key) && Game.CurrentInputMode == InputMode.MouseAndKeyboard;

        public static bool IsControllerButtonPressed(ControllerButtons button) => IsControlPressed(0, (int)button) && Game.CurrentInputMode == InputMode.GamePad;
        public static bool IsControllerButtonJustPressed(ControllerButtons button) => IsControlJustPressed(0, (int)button) && Game.CurrentInputMode == InputMode.GamePad;
        public static bool IsControllerButtonJustReleased(ControllerButtons button) => IsDisabledControlJustReleased(0, (int)button) && Game.CurrentInputMode == InputMode.GamePad;
        public static bool IsControllerButtonPressedRegardlessOfDisabled(ControllerButtons button) => IsControlPressed(0, (int)button) || IsDisabledControlJustReleased(0, (int)button) && Game.CurrentInputMode == InputMode.GamePad;
        #endregion

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
