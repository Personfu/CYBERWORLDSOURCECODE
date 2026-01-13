using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

namespace ClubPenguin.Input
{
    public class KeyCodeInput : Input<ButtonInputResult>
    {
        public KeyCode[] Keys = new KeyCode[0];

        private KeyCode[] mutableKeys;

        public KeyCode PrimaryKey
        {
            get
            {
                return (mutableKeys.Length != 0) ? mutableKeys[0] : KeyCode.None;
            }
        }

        public override void Initialize(KeyCodeRemapper keyCodeRemapper)
        {
            mutableKeys = new KeyCode[Keys.Length];
            for (int i = 0; i < Keys.Length; i++)
            {
                mutableKeys[i] = keyCodeRemapper.GetKeyCode(Keys[i]);
            }
            base.Initialize(keyCodeRemapper);
        }

        protected override bool process(int filter)
        {
            bool flag = false;
            bool flag2 = false;
            KeyCode[] array = mutableKeys;
            foreach (KeyCode key in array)
            {
                // Map KeyCode to Keyboard key in the new Input System
                var keyControl = GetKeyControl(key);
                if (keyControl != null)
                {
                    flag2 |= keyControl.wasPressedThisFrame;
                    flag |= keyControl.isPressed;
                }
            }
            inputEvent.WasJustPressed = (flag2 && !inputEvent.IsHeld);
            inputEvent.WasJustReleased = (!flag && inputEvent.IsHeld);
            inputEvent.IsHeld = (flag || flag2);
            return inputEvent.IsHeld || inputEvent.WasJustReleased;
        }

        // Helper: Maps UnityEngine.KeyCode to UnityEngine.InputSystem.KeyControl
        private static KeyControl GetKeyControl(KeyCode keyCode)
        {
            if (Keyboard.current == null)
                return null;

            switch (keyCode)
            {
                case KeyCode.A: return Keyboard.current.aKey;
                case KeyCode.B: return Keyboard.current.bKey;
                case KeyCode.C: return Keyboard.current.cKey;
                case KeyCode.D: return Keyboard.current.dKey;
                case KeyCode.E: return Keyboard.current.eKey;
                case KeyCode.F: return Keyboard.current.fKey;
                case KeyCode.G: return Keyboard.current.gKey;
                case KeyCode.H: return Keyboard.current.hKey;
                case KeyCode.I: return Keyboard.current.iKey;
                case KeyCode.J: return Keyboard.current.jKey;
                case KeyCode.K: return Keyboard.current.kKey;
                case KeyCode.L: return Keyboard.current.lKey;
                case KeyCode.M: return Keyboard.current.mKey;
                case KeyCode.N: return Keyboard.current.nKey;
                case KeyCode.O: return Keyboard.current.oKey;
                case KeyCode.P: return Keyboard.current.pKey;
                case KeyCode.Q: return Keyboard.current.qKey;
                case KeyCode.R: return Keyboard.current.rKey;
                case KeyCode.S: return Keyboard.current.sKey;
                case KeyCode.T: return Keyboard.current.tKey;
                case KeyCode.U: return Keyboard.current.uKey;
                case KeyCode.V: return Keyboard.current.vKey;
                case KeyCode.W: return Keyboard.current.wKey;
                case KeyCode.X: return Keyboard.current.xKey;
                case KeyCode.Y: return Keyboard.current.yKey;
                case KeyCode.Z: return Keyboard.current.zKey;

                case KeyCode.Alpha0: return Keyboard.current.digit0Key;
                case KeyCode.Alpha1: return Keyboard.current.digit1Key;
                case KeyCode.Alpha2: return Keyboard.current.digit2Key;
                case KeyCode.Alpha3: return Keyboard.current.digit3Key;
                case KeyCode.Alpha4: return Keyboard.current.digit4Key;
                case KeyCode.Alpha5: return Keyboard.current.digit5Key;
                case KeyCode.Alpha6: return Keyboard.current.digit6Key;
                case KeyCode.Alpha7: return Keyboard.current.digit7Key;
                case KeyCode.Alpha8: return Keyboard.current.digit8Key;
                case KeyCode.Alpha9: return Keyboard.current.digit9Key;

                case KeyCode.Space: return Keyboard.current.spaceKey;
                case KeyCode.Return: return Keyboard.current.enterKey;
                case KeyCode.Escape: return Keyboard.current.escapeKey;
                case KeyCode.Tab: return Keyboard.current.tabKey;
                case KeyCode.BackQuote: return Keyboard.current.backquoteKey;
                case KeyCode.Minus: return Keyboard.current.minusKey;
                case KeyCode.Equals: return Keyboard.current.equalsKey;
                case KeyCode.LeftBracket: return Keyboard.current.leftBracketKey;
                case KeyCode.RightBracket: return Keyboard.current.rightBracketKey;
                case KeyCode.Backslash: return Keyboard.current.backslashKey;
                case KeyCode.Semicolon: return Keyboard.current.semicolonKey;
                case KeyCode.Quote: return Keyboard.current.quoteKey;
                case KeyCode.Comma: return Keyboard.current.commaKey;
                case KeyCode.Period: return Keyboard.current.periodKey;
                case KeyCode.Slash: return Keyboard.current.slashKey;

                case KeyCode.LeftShift: return Keyboard.current.leftShiftKey;
                case KeyCode.RightShift: return Keyboard.current.rightShiftKey;
                case KeyCode.LeftControl: return Keyboard.current.leftCtrlKey;
                case KeyCode.RightControl: return Keyboard.current.rightCtrlKey;
                case KeyCode.LeftAlt: return Keyboard.current.leftAltKey;
                case KeyCode.RightAlt: return Keyboard.current.rightAltKey;
                case KeyCode.LeftCommand: return Keyboard.current.leftMetaKey;
                case KeyCode.RightCommand: return Keyboard.current.rightMetaKey;
                case KeyCode.CapsLock: return Keyboard.current.capsLockKey;

                case KeyCode.UpArrow: return Keyboard.current.upArrowKey;
                case KeyCode.DownArrow: return Keyboard.current.downArrowKey;
                case KeyCode.LeftArrow: return Keyboard.current.leftArrowKey;
                case KeyCode.RightArrow: return Keyboard.current.rightArrowKey;

                case KeyCode.F1: return Keyboard.current.f1Key;
                case KeyCode.F2: return Keyboard.current.f2Key;
                case KeyCode.F3: return Keyboard.current.f3Key;
                case KeyCode.F4: return Keyboard.current.f4Key;
                case KeyCode.F5: return Keyboard.current.f5Key;
                case KeyCode.F6: return Keyboard.current.f6Key;
                case KeyCode.F7: return Keyboard.current.f7Key;
                case KeyCode.F8: return Keyboard.current.f8Key;
                case KeyCode.F9: return Keyboard.current.f9Key;
                case KeyCode.F10: return Keyboard.current.f10Key;
                case KeyCode.F11: return Keyboard.current.f11Key;
                case KeyCode.F12: return Keyboard.current.f12Key;

                // Extend as needed for other keys
                default: return null;
            }
        }
    }
}