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
       

        //BEGIN
        public void OnPointerDown(PointerEventData e) {
            usingThis = true;
            UpdateWithPosition(e.position);
        }
        //UPDATE VALUE
        public void OnDrag(PointerEventData e) {
            if (usingThis) UpdateWithPosition(e.position);
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

        void UpdateWithPosition(Vector2 position) {
            Vector3 pos = position;
            pos.x = (position.x / Screen.width) * rtCanvas.sizeDelta.x * thisGraphic.lossyScale.x;
            pos.y = (position.y / Screen.height) * rtCanvas.sizeDelta.y * thisGraphic.lossyScale.y;

            var radius = thisGraphic.sizeDelta.x * thisGraphic.lossyScale.x * 0.5f;
            var dir = Vector3.ClampMagnitude(pos - thisGraphic.position, radius);

            inputGraphic.position = thisGraphic.position + dir;

            currentValue = dir / radius;
        }
        void EndValue() {
            currentValue = Vector2.zero;
            inputGraphic.localPosition = Vector3.zero;
        }

    }

    
}

