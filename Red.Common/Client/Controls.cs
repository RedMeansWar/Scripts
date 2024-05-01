using CitizenFX.Core;
using System.Collections.Generic;
using static CitizenFX.Core.Native.API;

namespace Red.Common.Client
{
    public class Controls
    {
        #region Private Variables
        protected static readonly IReadOnlyList<Control> cameraControls = new List<Control>()
        {
            Control.LookBehind, Control.LookDown, Control.LookDownOnly, Control.LookLeft, Control.LookLeftOnly, Control.LookLeftRight, Control.LookRight,
            Control.LookRightOnly, Control.LookUp, Control.LookUpDown, Control.LookUpOnly, Control.ScaledLookDownOnly, Control.ScaledLookLeftOnly,
            Control.ScaledLookLeftRight, Control.ScaledLookUpDown, Control.ScaledLookUpOnly,  Control.VehicleDriveLook, Control.VehicleDriveLook2,
            Control.VehicleLookBehind, Control.VehicleLookLeft, Control.VehicleLookRight, Control.NextCamera, Control.VehicleFlyAttackCamera, Control.VehicleCinCam,
        };

        protected static readonly IReadOnlyList<Control> movementControls = new List<Control>()
        {
            Control.MoveDown, Control.MoveDownOnly, Control.MoveLeft, Control.MoveLeftOnly, Control.MoveLeftRight, Control.MoveRight, Control.MoveRightOnly,
            Control.MoveUp, Control.MoveUpDown, Control.MoveUpOnly, Control.VehicleFlyMouseControlOverride, Control.VehicleMouseControlOverride,
            Control.VehicleMoveDown, Control.VehicleMoveDownOnly, Control.VehicleMoveLeft, Control.VehicleMoveLeftRight, Control.VehicleMoveRight,
            Control.VehicleMoveRightOnly, Control.VehicleMoveUp, Control.VehicleMoveUpDown, Control.VehicleSubMouseControlOverride, Control.Duck, Control.SelectWeapon
        };

        protected static readonly IReadOnlyList<Control> attackControls = new List<Control>()
        {
            Control.Attack, Control.Attack2, Control.MeleeAttack1, Control.MeleeAttack2, Control.MeleeAttackAlternate, Control.MeleeAttackHeavy,
            Control.MeleeAttackLight, Control.MeleeBlock, Control.VehicleAttack, Control.VehicleAttack2, Control.VehicleFlyAttack, Control.VehicleFlyAttack2,
            Control.VehiclePassengerAttack
        };
        #endregion

        public static bool IsControlJustPressed(Control control, int inputGroup = 0) => Game.IsControlJustPressed(inputGroup, control) && Game.CurrentInputMode == InputMode.MouseAndKeyboard && UpdateOnscreenKeyboard() != 0;

        public static bool IsControlJustPressed(Key key, int inputGroup = 0) => Game.IsControlJustPressed(inputGroup, (Control)key) && Game.CurrentInputMode == InputMode.MouseAndKeyboard && UpdateOnscreenKeyboard() != 0;

        public static bool IsControlJustPressed(Button button, int inputGroup = 0) => Game.IsControlJustPressed(inputGroup, (Control)button) && Game.CurrentInputMode == InputMode.GamePad && UpdateOnscreenKeyboard() != 0;

        /// <summary>
        /// Determines if the control was pressed regardless of being disabled.
        /// </summary>
        /// <param name="control">The control that is specified.</param>
        /// <param name="inputGroup">The input group that is mouse & keyboard or controller.</param>
        /// <returns>The enabled key that is pressed regardless of being disabled.</returns>
        public static bool IsControlJustPressedRegardless(Control control, int inputGroup = 0) => Game.IsControlPressed(inputGroup, control) || Game.IsDisabledControlPressed(inputGroup, control) && Game.CurrentInputMode == InputMode.MouseAndKeyboard && UpdateOnscreenKeyboard() != 0;

        public static bool IsControlJustPressedRegardless(Key key, int inputGroup = 0) => Game.IsControlPressed(inputGroup, (Control)key) || Game.IsDisabledControlPressed(inputGroup, (Control)key) && Game.CurrentInputMode == InputMode.MouseAndKeyboard && UpdateOnscreenKeyboard() != 0;

        public static bool IsControlJustPressedRegardless(Button button, int inputGroup = 0) => Game.IsControlPressed(inputGroup, (Control)button) || Game.IsDisabledControlPressed(inputGroup, (Control)button) && Game.CurrentInputMode == InputMode.GamePad && UpdateOnscreenKeyboard() != 0;

        /// <summary>
        /// Determines if the control was just pressed.
        /// </summary>
        /// <param name="control">The control that is specified.</param>
        /// <param name="inputGroup">The input group that is mouse & keyboard or controller.</param>
        /// <returns>The enabled key that is pressed.</returns>
        public static bool IsControlPressed(Control control, int inputGroup = 0) => Game.IsControlPressed(inputGroup, control) && Game.CurrentInputMode == InputMode.MouseAndKeyboard && UpdateOnscreenKeyboard() != 0;

        public static bool IsControlPressed(Key key, int inputGroup = 0) => Game.IsControlPressed(inputGroup, (Control)key) && Game.CurrentInputMode == InputMode.MouseAndKeyboard && UpdateOnscreenKeyboard() != 0;

        public static bool IsControlPressed(Button button, int inputGroup = 2) => Game.IsControlPressed(inputGroup, (Control)button) && Game.CurrentInputMode == InputMode.GamePad && UpdateOnscreenKeyboard() != 0;

        /// <summary>
        /// Determines if the control was pressed regardless of being disabled.
        /// </summary>
        /// <param name="control">The control that is specified.</param>
        /// <param name="inputGroup">The input group that is mouse & keyboard or controller.</param>
        /// <returns>The enabled key that is pressed regardless of being disabled.</returns>
        public static bool IsControlPressedRegardless(Control control, int inputGroup = 0) => Game.IsControlPressed(inputGroup, control) || Game.IsDisabledControlPressed(inputGroup, control) && Game.CurrentInputMode == InputMode.MouseAndKeyboard && UpdateOnscreenKeyboard() != 0;

        public static bool IsControlPressedRegardless(Key key, int inputGroup = 0) => Game.IsControlPressed(inputGroup, (Control)key) || Game.IsDisabledControlPressed(inputGroup, (Control)key) && Game.CurrentInputMode == InputMode.MouseAndKeyboard && UpdateOnscreenKeyboard() != 0;

        public static bool IsControlPressedRegardless(Button button, int inputGroup = 2) => Game.IsControlPressed(inputGroup, (Control)button) || Game.IsDisabledControlPressed(inputGroup, (Control)button) && Game.CurrentInputMode == InputMode.GamePad && UpdateOnscreenKeyboard() != 0;

        /// <summary>
        /// Checks if a disabled control was just released.
        /// </summary>
        /// <param name="control">The control that is specified.</param>
        /// <param name="padIndex">The input group that is mouse & keyboard or controller.</param>
        /// <returns>The disabled control just released value.</returns>
        public static bool IsDisabledControlJustReleased(Control control, int inputGroup = 0) => Game.IsDisabledControlJustReleased(inputGroup, control) && Game.CurrentInputMode == InputMode.MouseAndKeyboard && UpdateOnscreenKeyboard() != 0;

        public static bool IsDisabledControlJustReleased(Key key, int inputGroup = 0) => Game.IsDisabledControlJustReleased(inputGroup, (Control)key) && Game.CurrentInputMode == InputMode.MouseAndKeyboard && UpdateOnscreenKeyboard() != 0;

        public static bool IsDisabledControlJustReleased(Button button, int inputGroup = 2) => Game.IsDisabledControlJustReleased(inputGroup, (Control)button) && Game.CurrentInputMode == InputMode.GamePad && UpdateOnscreenKeyboard() != 0;


        /// <summary>
        /// Determines if a disable control was just pressed
        /// </summary>
        /// <param name="control">The control that is specified.</param>
        /// <param name="inputGroup">The input group that is mouse & keyboard or controller.</param>
        /// <returns>The disabled key that is just pressed.</returns>
        public static bool IsDisabledControlJustPressed(Control control, int inputGroup = 0) => Game.IsDisabledControlJustPressed(inputGroup, control) && Game.CurrentInputMode == InputMode.MouseAndKeyboard && UpdateOnscreenKeyboard() != 0;

        public static bool IsDisabledControlJustPressed(Key key, int inputGroup = 0) => Game.IsDisabledControlJustPressed(inputGroup, (Control)key) && Game.CurrentInputMode == InputMode.MouseAndKeyboard && UpdateOnscreenKeyboard() != 0;

        public static bool IsDisabledControlJustPressed(Button button, int inputGroup = 2) => Game.IsDisabledControlJustPressed(inputGroup, (Control)button) && Game.CurrentInputMode == InputMode.GamePad && UpdateOnscreenKeyboard() != 0;

        /// <summary>
        /// Checks if a disabled control was just pressed.
        /// </summary>
        /// <param name="control">The control that is specified.</param>
        /// <param name="padIndex">The input group that is mouse & keyboard or controller.</param>
        /// <returns>The disabled control just pressed value.</returns>
        public static bool IsDisabledControlPressed(Control control, int inputGroup = 0) => Game.IsDisabledControlPressed(inputGroup, control) && Game.CurrentInputMode == InputMode.MouseAndKeyboard && UpdateOnscreenKeyboard() != 0;

        public static bool IsDisabledControlPressed(Key key, int inputGroup = 0) => Game.IsDisabledControlPressed(inputGroup, (Control)key) && Game.CurrentInputMode == InputMode.MouseAndKeyboard && UpdateOnscreenKeyboard() != 0;

        public static bool IsDisabledControlPressed(Button button, int inputGroup = 2) => Game.IsDisabledControlPressed(inputGroup, (Control)button) && Game.CurrentInputMode == InputMode.GamePad && UpdateOnscreenKeyboard() != 0;

        /// <summary>
        /// Determines if the control was just released
        /// </summary>
        /// <param name="control">The control that is specified.</param>
        /// <param name="inputGroup">The input group that is mouse & keyboard or controller.</param>
        /// <returns>The enabled key that is released.</returns>
        public static bool IsControlJustReleased(Control control, int inputGroup = 0) => Game.IsControlJustReleased(inputGroup, control) && Game.CurrentInputMode == InputMode.MouseAndKeyboard && UpdateOnscreenKeyboard() != 0;

        public static bool IsControlJustReleased(Key key, int inputGroup = 0) => Game.IsControlJustReleased(inputGroup, (Control)key) && Game.CurrentInputMode == InputMode.MouseAndKeyboard && UpdateOnscreenKeyboard() != 0;

        public static bool IsControlJustReleased(Button button, int inputGroup = 2) => Game.IsControlJustReleased(inputGroup, (Control)button) && Game.CurrentInputMode == InputMode.GamePad && UpdateOnscreenKeyboard() != 0;

        /// <summary>
        /// Determines if the control was released
        /// </summary>
        /// <param name="control">The control that is specified.</param>
        /// <param name="inputGroup">The input group that is mouse & keyboard or controller.</param>
        /// <returns>The enabled key that is released.</returns>
        public static bool IsControlReleased(Control control, int inputGroup = 0) => Game.IsControlJustReleased(inputGroup, control) && Game.CurrentInputMode == InputMode.MouseAndKeyboard && UpdateOnscreenKeyboard() != 0;

        public static bool IsControlReleased(Key key, int inputGroup = 0) => Game.IsControlJustReleased(inputGroup, (Control)key) && Game.CurrentInputMode == InputMode.MouseAndKeyboard && UpdateOnscreenKeyboard() != 0;

        public static bool IsControlReleased(Button button, int inputGroup = 2) => Game.IsControlJustReleased(inputGroup, (Control)button) && Game.CurrentInputMode == InputMode.GamePad && UpdateOnscreenKeyboard() != 0;

        /// Disables a certain key for a frame.
        /// </summary>
        /// <param name="control">The control to disable</param>
        /// <param name="index">The input group that is mouse & keyboard or controller.</param>
        public static void DisableControlThisFrame(Control control, int index = 0) => Game.DisableControlThisFrame(index, control);

        public static void DisableControlThisFrame(Key key, int index = 0) => Game.DisableControlThisFrame(index, (Control)key);

        public static void DisableControlThisFrame(Button button, int index = 2) => Game.DisableControlThisFrame(index, (Control)button);

        /// <summary>
        /// Disables specific player controls for the current frame, optionally including camera controls.
        /// </summary>
        /// <param name="cameraMovement">Whether to disable camera controls in addition to movement controls.</param>
        public static void DisableMovementControls(bool cameraMovement)
        {
            // Disable camera controls if requested.
            if (cameraMovement)
            {
                foreach (Control control in cameraControls)
                {
                    // Disable the control for both the player and third-person camera.
                    Game.DisableControlThisFrame(0, control);  // Player
                    Game.DisableControlThisFrame(2, control);  // Third-person camera
                }
            }

            // Disable movement controls.
            foreach (Control control in movementControls)
            {
                // Disable the control for both the player and third-person camera.
                Game.DisableControlThisFrame(0, control);
                Game.DisableControlThisFrame(2, control);
            }
        }

        /// <summary>
        /// Checks if the last input was a controller input.
        /// </summary>
        /// <returns></returns>
        public static bool LastInputWasController() => IsUsingKeyboard(2);
    }

    public enum Key
    {
        ESC = 322, F1 = 288, F2 = 289, F3 = 170, F5 = 166, F6 = 167, F7 = 168, F8 = 169, F9 = 56, F10 = 57,
        TILDE = 243, ONE = 157, TWO = 158, THREE = 160, FOUR = 164, FIVE = 165, SIX = 159, SEVEN = 161, EIGHT = 162, NINE = 163, MINUS = 84, EQUAL = 83, BACKSPACE = 194,
        TAB = 37, Q = 44, W = 32, E = 38, R = 45, T = 245, Y = 246, U = 303, P = 199, OPENBRACKET = 39, CLOSEBRACKET = 40, ENTER = 18,
        CAPS = 137, A = 34, S = 8, D = 9, F = 23, G = 47, H = 74, K = 311, L = 182,
        LEFTSHIFT = 21, Z = 20, X = 73, C = 26, V = 0, B = 29, N = 249, M = 244, COMMA = 82, PERIOD = 81,
        LEFTCTRL = 36, LEFTALT = 19, SPACE = 22, RIGHTCTRL = 70,
        HOME = 213, PAGEUP = 316, PAGEDOWN = 317, DELETE = 178,
        LEFT = 174, RIGHT = 175, TOP = 27, DOWN = 173,
        NENTER = 201, N4 = 108, N5 = 60, N6 = 107, NADD = 96, NMINUS = 97, N7 = 117, N8 = 61, N9 = 118
    }

    public enum Button
    {
        XBoxA = 191, XBoxB = 194, XBoxY = 192, XBoxX = 193, DpadLeft = 189, DpadRight = 190, DpadUp = 188,
        DpadDown = 187, RT = 228, LT = 229, LB = 226, RB = 227, L3 = 230, R3 = 231, SELECT, PAUSE = 199,
        PsX = XBoxA, PsCircle = XBoxB, PsSquare = XBoxX, PsTriangle = XBoxY, L2 = LB, R2 = RB
    }
}
