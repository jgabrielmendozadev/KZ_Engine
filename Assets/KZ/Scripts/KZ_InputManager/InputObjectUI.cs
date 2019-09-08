using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

namespace KZ {
    public class InputObjectUI : MonoBehaviour, IInputObject, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler {

        #region IInputObject Implementation
        public InputType GetInputType() { return _type; }
        public KeyCode GetKeyCode() { return default(KeyCode); }
        public bool GetInput() { return _inputState; }
        #endregion


        [SerializeField] string _commandName = "";
        [SerializeField] InputType _type = InputType.Down;
        bool _inputState;

        void Start() {
            AssignActions(this);
            InputManager.AssignInput(_commandName, this);
        }
        void OnDestroy() {
            InputManager.RemoveInput(_commandName, this);
        }
        void OnDisable() {
            _inputState = false;
        }

        Action<InputObjectUI> WhenDown = delegate { };
        Action<InputObjectUI> WhenUp = delegate { };
        Action<InputObjectUI> WhenEnter = delegate { };
        Action<InputObjectUI> WhenExit = delegate { };

        public void OnPointerDown(PointerEventData e) { WhenDown(this); }
        public void OnPointerUp(PointerEventData e) { WhenUp(this); }
        public void OnPointerEnter(PointerEventData e) { WhenEnter(this); }
        public void OnPointerExit(PointerEventData e) { WhenExit(this); }


        #region Static Initializers
        static void AssignActions(InputObjectUI io) {
            switch (io._type) {
                case InputType.Hold:
                    io.WhenDown = SetTrue;
                    io.WhenUp = SetFalse;
                    io.WhenEnter = x => io._inputState = Input.GetKey(KeyCode.Mouse0);
                    io.WhenExit = SetFalse;
                    break;
                case InputType.Down:
                    io.WhenDown = SetTrueOneFrame_Down;
                    io.WhenUp = delegate { };
                    io.WhenEnter = delegate { };
                    io.WhenExit = delegate { };
                    break;
                case InputType.Up:
                    bool isPressed = false;
                    io.WhenDown = x => isPressed = true;
                    io.WhenUp = x => { if (isPressed) SetTrueOneFrame_Up(x); };
                    io.WhenEnter = delegate { };
                    io.WhenExit = x => isPressed = false;
                    break;
            }
        }

        //Actions
        static void SetTrue(InputObjectUI io) { io._inputState = true; }
        static void SetFalse(InputObjectUI io) { io._inputState = false; }
        static void SetTrueOneFrame_Down(InputObjectUI io) { io.StartCoroutine(CR_SetTrueForOneFrame(io)); }
        static void SetTrueOneFrame_Up(InputObjectUI io) { io.StartCoroutine(CR_SetTrueForOneFrame(io)); }
        static IEnumerator CR_SetTrueForOneFrame(InputObjectUI io) {
            io._inputState = true;
            yield return new WaitForEndOfFrame();
            io._inputState = false;
        }
        #endregion

    }

}