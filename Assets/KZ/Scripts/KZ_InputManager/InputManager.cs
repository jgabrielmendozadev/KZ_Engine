using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace KZ {
    public static class InputManager {

        //public Var
        public static float MouseSensivity = 8;

        static Dictionary<string, List<IInputObject>> cmds = new Dictionary<string, List<IInputObject>>();

        //Get inputs
        public static  Vector2 GetMouseMovement() {
            return new Vector2(
                Input.GetAxis("Mouse X") * MouseSensivity,
                Input.GetAxis("Mouse Y") * MouseSensivity
            );
        }
        public static  Vector2 GetMousePosition() {
            var r = Input.mousePosition;
            return new Vector2((r.x / Screen.width).Clamp(), (r.y / Screen.height).Clamp());
        }
        public static bool GetInput(string cmd) {
            return cmds.ContainsKey(cmd) ? cmds[cmd].Any(inpObj => inpObj.GetInput()) : false;
        }


        //Handle Inputs
        public static void AssignKey(string commandName, KeyCode key, InputType inputType = InputType.Down) {
            if (!cmds.ContainsKey(commandName))
                cmds[commandName] = new List<IInputObject>();

            if (!cmds[commandName].Any(inpObj => inpObj.GetType() == typeof(InputObjectClass) && inpObj.GetKeyCode() == key))
                cmds[commandName].Add(new InputObjectClass(key, inputType));
            else
                Debug.LogWarning("Key already contained: \"" + key + "\" for \"" + commandName + "\"");

            var asd = cmds
                .Where(kv => kv.Key != commandName)
                .Where(kv => kv.Value.Any(inpObj => inpObj.GetKeyCode() == key))
                .ToList();

            if (asd != null && asd.Any())
                Debug.LogWarning("Keycode \"" + key + "\" also triggers: " + asd.MakeString(x => x.Key, ","));

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






        public static readonly Dictionary<InputType, Func<KeyCode, bool>> pressOperation =
            new Dictionary<InputType, Func<KeyCode, bool>> {
                { InputType.Down , Input.GetKeyDown},
                { InputType.Hold , Input.GetKey},
                { InputType.Up , Input.GetKeyUp}
            };
    }

    public enum InputType { Hold, Down, Up }

    public interface IInputObject {

        InputType GetInputType();
        KeyCode GetKeyCode();

        void SetValues(KeyCode key, InputType type);

        bool GetInput();

    }

    public class InputObjectClass : IInputObject {
        InputType _type;
        KeyCode _key;

        public InputType GetInputType() { return _type; }
        public KeyCode GetKeyCode() { return _key; }

        public bool GetInput() {
            return InputManager.pressOperation[_type](_key);
        }

        public void SetValues(KeyCode key, InputType type) {
            _type = type;
            _key = key;
        }

        //ctor
        public InputObjectClass(KeyCode key, InputType type) {
            SetValues(key, type);
        }
    }
}