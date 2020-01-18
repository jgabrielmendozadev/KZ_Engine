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
        public Vector2 GetInput() => currentValue;
        public string GetJoystickName() => _joystickName;
        #endregion


        [Header("InputObject")]
        [SerializeField] string _joystickName = "";
        protected Vector2 currentValue = Vector2.zero;
        [Header("Graphics")]
        [SerializeField] protected RectTransform backGraphic = null;
        [SerializeField] protected RectTransform inputGraphic = null;

        
        protected virtual void Start() {
            rtCanvas = GetComponentInParent<Canvas>().GetComponent<RectTransform>();
            InputManager.AssignJoystick(_joystickName, this);
        }
        void OnDestroy() {
            InputManager.RemoveJoystick(this);
        }

        bool usingThis = false;
        protected RectTransform rtCanvas;
       

        //BEGIN
        public void OnPointerDown(PointerEventData e) {
            usingThis = true;
            BeginValue(e.position);
        }
        //UPDATE VALUE
        public void OnDrag(PointerEventData e) {
            if (usingThis) UpdateValue(e.position);
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


        protected virtual void BeginValue(Vector2 position) {
            UpdateValue(position);
        }
        protected virtual void UpdateValue(Vector2 position) {
            Vector3 pos = new Vector3();
            pos.x = (position.x / Screen.width) * rtCanvas.sizeDelta.x * backGraphic.lossyScale.x;
            pos.y = (position.y / Screen.height) * rtCanvas.sizeDelta.y * backGraphic.lossyScale.y;

            var radius = backGraphic.sizeDelta.x * backGraphic.lossyScale.x * 0.5f;
            var dir = Vector3.ClampMagnitude(pos - backGraphic.position, radius);

            inputGraphic.position = backGraphic.position + dir;

            currentValue = dir / radius;
        }
        protected virtual void EndValue() {
            currentValue = Vector2.zero;
            inputGraphic.localPosition = Vector3.zero;
        }

    }

    
}

