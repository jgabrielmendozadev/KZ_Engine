using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace KZ {
    public class InputObjectJoystickUI : MonoBehaviour, IInputObjectJoystick , IPointerDownHandler {

        #region IInputObjectJoystick Implementation
        public Vector2 GetInput() { return currentValue; }
        public string GetJoystickName() { return _joystickName; }
        #endregion


        [SerializeField] RectTransform thisGraphic;
        [Header("InputObject")]
        [SerializeField] string _joystickName = "";
        [SerializeField] Vector2 currentValue = Vector2.zero;
        [Header("Graphics")]
        [SerializeField] RectTransform inputGraphic;

        
       


        void Start() {
            _state = State_Idle;
            InputManager.AssignJoystick(_joystickName, this);
        }
        void Update() {
            _state();
        }
        void OnDestroy() {
            InputManager.RemoveJoystick(this);
        }


        Action _state = delegate { };

        void State_Idle() { }
        void State_Hold() {
            if (!Input.GetKey(KeyCode.Mouse0)) {
                currentValue = Vector2.zero;
                _state = State_Idle;
                inputGraphic.localPosition = Vector3.zero;
            }
            else {
                var mousePos = Input.mousePosition;
                var dir1 = (mousePos - transform.position);
                var dir2 = dir1.normalized;
                var value = thisGraphic.sizeDelta.x / 2;

                if (dir1.magnitude > value)
                    dir1 = dir2 * value;

                inputGraphic.localPosition = dir1;
                currentValue = dir1 / value;
            }
        }


        public void OnPointerDown(PointerEventData e) {
            _state = State_Hold;
        }

    }

    
}

