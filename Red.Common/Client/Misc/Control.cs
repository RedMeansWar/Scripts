using CitizenFX.Core;
using CitizenFX.Core.Native;
using Mono.CSharp;
using static CitizenFX.Core.Native.API;

namespace Red.Common.Client.Misc
{
    public static class Controls
    {
        #region Methods
        public static bool IsOnScreenKeyboardActive() => UpdateOnscreenKeyboard() == 3;
        public static int UpdateKeyboard() => UpdateOnscreenKeyboard();
        public static int TimeSinceLastInput(int inputGroup = 0) => GetTimeSinceLastInput(inputGroup);
        public static int TimeSinceLastInput(ControlType inputGroup) => TimeSinceLastInput((int)inputGroup);

        public static bool IsControlJustPressedRegardlessOfDisabled(Control control, int inputGroup = 0) => Game.IsDisabledControlJustPressed(inputGroup, control) || IsDisabledControlJustPressed(0, (int)control);
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
