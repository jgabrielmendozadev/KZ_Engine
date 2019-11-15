using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace KZ {
    /*
    InputManager.AssignKey can use 1 key, or a list of keys,
    where only the last one is checked with inputType, and all the rest are checked with "hold"
    */
    public static class InputManager {

        //public Var
        public static float MouseSensivity = 8;

        static Dictionary<string, List<IInputObject>> cmds = new Dictionary<string, List<IInputObject>>();

        //Get inputs
        public static Vector2 GetMouseMovement() {
            return new Vector2(
                Input.GetAxis("Mouse X") * MouseSensivity,
                Input.GetAxis("Mouse Y") * MouseSensivity
            );
        }

        /// <summary>normalized positions (0 - 1, 0 - 1)</summary>
        public static Vector2 GetMousePosition() {
            var r = Input.mousePosition;
            return new Vector2((r.x / Screen.width).Clamp(), (r.y / Screen.height).Clamp());
        }
        public static bool GetInput(string cmd) {
            return cmds.ContainsKey(cmd) ? cmds[cmd].Any(inpObj => inpObj.GetInput()) : false;
        }

        //Handle Inputs
        public static void AssignKey(string commandName, KeyCode key, InputType inputType = InputType.Down) {
            var preK = new List<KeyCode>();
            AssignKey_Internal(commandName, key, () => new InputObjectClass(key, preK, inputType));
        }
        public static void AssignKey(string commandName, List<KeyCode> keys, InputType inputType = InputType.Down) {
            var k = keys.Last();
            var preK = keys.Where(x => x != k).ToList();
            AssignKey_Internal(commandName, k, () => new InputObjectClass(k, preK, inputType));
        }
        static void AssignKey_Internal(string commandName, KeyCode mainKey, Func<InputObjectClass> factory) {
            if (!cmds.ContainsKey(commandName))
                cmds[commandName] = new List<IInputObject>();

            if (!cmds[commandName].Any(inpObj => inpObj.GetType() == typeof(InputObjectClass) && inpObj.GetKeyCode() == mainKey))
                cmds[commandName].Add(factory() );
            else
                Debug.LogWarning("Key already contained: \"" + mainKey + "\" for \"" + commandName + "\"");

            var asd = cmds
                .Where(kv => kv.Key != commandName)
                .Where(kv => kv.Value.Any(inpObj => inpObj.GetKeyCode() == mainKey))
                .ToList();

            if (asd != null && asd.Any())
                Debug.LogWarning("Keycode \"" + mainKey + "\" also triggers: " + asd.MakeString(x => x.Key, ","));
        }

        public static void RemoveKey(string commandName, KeyCode key) {
            if (cmds.ContainsKey(commandName)) {
                cmds[commandName] = cmds[commandName].Where(x => x.GetKeyCode() != key).ToList();
                if (cmds[commandName] == null || !cmds[commandName].Any())
                    cmds.Remove(commandName);
            }
        }
        


        public static void AssignInput(string commandName, IInputObject inpObj) {
            if (!cmds.ContainsKey(commandName))
                cmds[commandName] = new List<IInputObject>();
            cmds[commandName].Add(inpObj);
        }
        public static void RemoveInput(string commandName, IInputObject inpObj) {
            if (cmds.ContainsKey(commandName)) {
                cmds[commandName] = cmds[commandName].Where(x => x != inpObj).ToList();
                if (cmds[commandName] == null || !cmds[commandName].Any())
                    cmds.Remove(commandName);
            }
        }

        public static void ResetAllKeys() {
            cmds = new Dictionary<string, List<IInputObject>>();
        }


        public static void OnNextKeyPressed(Action<KeyCode> OnKeyDown) {
            KeyCode k = default(KeyCode);
            Func<bool> check = () => {
                if (Input.anyKey)
                    foreach (KeyCode key in Enum.GetValues(typeof(KeyCode)))
                        if (Input.GetKey(key)) { k = key; return true; }
                return false;
            };
            Action onCheck = () => OnKeyDown(k);
            Debug.Log("check started");
            Utility.ExecuteWhen(check, onCheck);
        }

        public static readonly Dictionary<InputType, Func<KeyCode, bool>> pressOperation =
            new Dictionary<InputType, Func<KeyCode, bool>> {
                { InputType.Down , Input.GetKeyDown},
                { InputType.Hold , Input.GetKey},
                { InputType.Up , Input.GetKeyUp}
            };


        #region Joystick
        static Dictionary<string, IInputObjectJoystick> _joysticks = new Dictionary<string, IInputObjectJoystick>();

        public static void AssignJoystick(string joystickName, IInputObjectJoystick uiJoystick) {
            if (!_joysticks.ContainsKey(joystickName))
                _joysticks[joystickName] = uiJoystick;
            else
                Debug.LogWarning("error trying to add a new joystick \"" + joystickName + "\", key already contained");
        }
        public static void RemoveJoystick(IInputObjectJoystick uiJoystick) {
            var name = uiJoystick.GetJoystickName();
            if (_joysticks.ContainsKey(name))
                _joysticks.Remove(name);
        }
        public static Vector2 GetJoystickInput(string joystickName) {
            if (_joysticks.ContainsKey(joystickName))
                return _joysticks[joystickName].GetInput();
            else
                return Vector2.zero;
        }
        #endregion



        #region DragUI
        static Dictionary<string, IInputObjectUIDrag> _UIDrags = new Dictionary<string, IInputObjectUIDrag>();

        public static void AssignInputDragUI(string uiDragName, IInputObjectUIDrag uiInputDrag) {
            if (!_UIDrags.ContainsKey(uiDragName))
                _UIDrags[uiDragName] = uiInputDrag;
            else
                Debug.LogWarning("error trying to add a new Input Drag UI object \"" + uiDragName + "\", key already contained");
        }
        public static void RemoveInputDragUI(IInputObjectUIDrag uiInputDrag) {
            var name = uiInputDrag.GetUIDragName();
            if (_UIDrags.ContainsKey(name))
                _UIDrags.Remove(name);
        }
        public static Vector2 GetUIDragInput(string UIInputDrag) {
            if (_UIDrags.ContainsKey(UIInputDrag))
                return _UIDrags[UIInputDrag].GetDeltaPosition();
            else
                return Vector2.zero;
        }
        #endregion
    }

    public enum InputType { Hold, Down, Up }

    public interface IInputObject {
        InputType GetInputType();
        KeyCode GetKeyCode();
        bool GetInput();
    }

    public class InputObjectClass : IInputObject {
        KeyCode _key;
        InputType _type;
        List<KeyCode> _preKeys;

        public InputType GetInputType() { return _type; }
        public KeyCode GetKeyCode() { return _key; }
        public List<KeyCode> GetAllKeys() { return _preKeys.ToList().AddReturn(_key); }
        public bool GetInput() {
            return
                _preKeys.All(InputManager.pressOperation[InputType.Hold])
                && InputManager.pressOperation[_type](_key);
        }
        public InputObjectClass(KeyCode key, List<KeyCode> preKeys,  InputType type) {
            _key = key;
            _type = type;
            _preKeys = preKeys;
        }
    }

    public interface IInputObjectJoystick {
        string GetJoystickName();
        Vector2 GetInput();
    }

    public interface IInputObjectUIDrag {
        string GetUIDragName();
        Vector2 GetDeltaPosition();
    }
}