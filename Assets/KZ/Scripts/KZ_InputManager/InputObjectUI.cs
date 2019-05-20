using UnityEngine;
using UnityEngine.EventSystems;

namespace KZ {
    public class InputObjectUI : MonoBehaviour, IInputObject, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler {

        #region IInputObject Implementation
        public InputType GetInputType() { return _type; }
        public KeyCode GetKeyCode() { return default(KeyCode); }

        public bool GetInput() {
            return _inputState;
        }
        #endregion


        [SerializeField] string _commandName = "";
        [SerializeField] InputType _type;
        bool _inputState;

        void Start() { InputManager.AssignInput(_commandName, this); }
        void OnDestroy() { InputManager.RemoveInput(_commandName, this); }

        bool _pressed;
        public void OnPointerDown(PointerEventData e) {
            switch (_type) {
                case InputType.Hold:
                    _inputState = true;
                    break;
                case InputType.Down:
                    _inputState = true;
                    Utility.ExecuteInFrames(1, () => _inputState = false);
                    break;
            }
            _pressed = true;
        }

        public void OnPointerUp(PointerEventData e) {
            switch (_type) {
                case InputType.Hold:
                    _inputState = false;
                    break;
                case InputType.Up:
                    if (_pressed) {
                        _inputState = true;
                        Utility.ExecuteInFrames(1, () => _inputState = false);
                    }
                    break;
            }
            _pressed = false;
        }

        public void OnPointerExit(PointerEventData e) {
            OnPointerUp(e);
            //if (_type == InputType.Hold) _inputState = false;
            //_pressed = false;
        }
    }

}