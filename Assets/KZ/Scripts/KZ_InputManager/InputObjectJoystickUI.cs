using System;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

namespace KZ {
    public class InputObjectJoystickUI : MonoBehaviour, IInputObjectJoystick
         , IEndDragHandler, IDragHandler
         , IPointerDownHandler, IPointerUpHandler  
        {

        #region IInputObjectJoystick Implementation
        public Vector2 GetInput() { return currentValue; }
        public string GetJoystickName() { return _joystickName; }
        #endregion


        [SerializeField] RectTransform thisGraphic = null;
        [Header("InputObject")]
        [SerializeField] string _joystickName = "";
        Vector2 currentValue = Vector2.zero;
        [Header("Graphics")]
        [SerializeField] RectTransform inputGraphic = null;

        
        void Start() {
            rtCanvas = GetComponentInParent<Canvas>().GetComponent<RectTransform>();
            InputManager.AssignJoystick(_joystickName, this);
        }

        void OnDestroy() {
            InputManager.RemoveJoystick(this);
        }

        bool usingThis = false;
        RectTransform rtCanvas;
       
        void EndValue() {
            currentValue = Vector2.zero;
            inputGraphic.localPosition = Vector3.zero;
        }


        //BEGIN
        public void OnPointerDown(PointerEventData e) {
            usingThis = true;
            UpdateWithId(e.pointerId);
        }
        //UPDATE VALUE
        public void OnDrag(PointerEventData e) {
            if (usingThis) UpdateWithId(e.pointerId);
        }
        //END
        public void OnEndDrag(PointerEventData e) {
            usingThis = false;
            EndValue();
        }
        //END
        public void OnPointerUp(PointerEventData e) {
            usingThis = false;
            EndValue();
        }


        void UpdateWithId(int id) {
            if (id == -1) {
                UpdateValue(InputManager.GetMousePosition());
                return;
            }
            else {
                foreach (var t in Input.touches) {
                    if (t.fingerId == id) {
                        UpdateValue(
                            (t.position.x / Screen.width).Clamp(),
                            (t.position.y / Screen.height).Clamp());
                        return;
                    }
                }
            }
            Debug.LogWarning("error reading mouse position");
        }

        Vector3 mousePos = Vector3.zero;
        void UpdateValue(float x, float y) {
            mousePos.x = x;
            mousePos.y = y;
            UpdateValue(mousePos);
        }
        void UpdateValue(Vector3 mousePos) {
            mousePos.x *= rtCanvas.sizeDelta.x * thisGraphic.lossyScale.x;
            mousePos.y *= rtCanvas.sizeDelta.y * thisGraphic.lossyScale.y;

            var radius = thisGraphic.sizeDelta.x * thisGraphic.lossyScale.x * 0.5f;
            var dir = Vector3.ClampMagnitude(mousePos - thisGraphic.position, radius);

            inputGraphic.position = thisGraphic.position + dir;

            currentValue = dir / radius;
        }
        
    }

    
}

