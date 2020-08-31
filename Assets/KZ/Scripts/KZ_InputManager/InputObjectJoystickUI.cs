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
            _rtCanvas = GetComponentInParent<Canvas>().GetComponent<RectTransform>();
            InputManager.AssignJoystick(_joystickName, this);
        }
        void OnDestroy() {
            InputManager.RemoveJoystick(this);
        }

        protected RectTransform _rtCanvas;
        int _currentPointerId = -1;
        Vector3 _pos = new Vector3();

        //BEGIN
        public void OnPointerDown(PointerEventData e) {
            if (_currentPointerId != -1) return;
            _currentPointerId = e.pointerId;
            BeginValue(e.position);
        }
        //UPDATE VALUE
        public void OnDrag(PointerEventData e) {
            if (_currentPointerId == e.pointerId) 
                UpdateValue(e.position);
        }
        //END
        public void OnEndDrag(PointerEventData e) {
            if (_currentPointerId == e.pointerId)
                EndValue();
        }
        //END
        public void OnPointerUp(PointerEventData e) {
            if (_currentPointerId == e.pointerId)
                EndValue();
        }


        protected virtual void BeginValue(Vector2 position) {
            UpdateValue(position);
        }
        protected virtual void UpdateValue(Vector2 position) {
            _pos.x = (position.x / Screen.width) * _rtCanvas.sizeDelta.x * backGraphic.lossyScale.x;
            _pos.y = (position.y / Screen.height) * _rtCanvas.sizeDelta.y * backGraphic.lossyScale.y;

            var radius = backGraphic.sizeDelta.x * backGraphic.lossyScale.x * 0.5f;
            var dir = Vector3.ClampMagnitude(_pos - backGraphic.position, radius);

            inputGraphic.position = backGraphic.position + dir;

            currentValue = dir / radius;
        }
        protected virtual void EndValue() {
            _currentPointerId = -1;
            currentValue = Vector2.zero;
            inputGraphic.localPosition = Vector3.zero;
        }

    }

    
}

