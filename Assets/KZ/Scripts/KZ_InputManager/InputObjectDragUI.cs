using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;

namespace KZ {
    public class InputObjectDragUI : MonoBehaviour, IInputObjectUIDrag
             , IEndDragHandler, IDragHandler
             , IPointerDownHandler, IPointerUpHandler {


        [SerializeField] string _dragName = "";

        void Start() {
            InputManager.AssignInputDragUI(_dragName, this);
        }
        void LateUpdate() {
            _deltaPosition = Vector2.zero;
        }
        void OnDestroy() {
            InputManager.RemoveInputDragUI(this);
        }

        Vector2 _deltaPosition = Vector2.zero;
        Vector2 _lastPosition = Vector2.zero;
        int _currentPointerId = -1;

        //BEGIN
        public void OnPointerDown(PointerEventData e) {
            if (_currentPointerId != -1) return;
            _currentPointerId = e.pointerId;
            DragBegin(e.position);
        }
        //UPDATE VALUE
        public void OnDrag(PointerEventData e) {
            if (_currentPointerId == e.pointerId)
                DragUpdate(e.position);
        }
        //END
        public void OnEndDrag(PointerEventData e) {
            if (_currentPointerId == e.pointerId)
                DragEnd();
        }
        //END 
        public void OnPointerUp(PointerEventData e) {
            if (_currentPointerId == e.pointerId)
                DragEnd();
        }
        

        void DragBegin(Vector2 position) {
            _lastPosition = position;
        }
        void DragUpdate(Vector2 position) {
            _deltaPosition = position - _lastPosition;
            _deltaPosition.x /= Screen.width;
            _deltaPosition.y /= Screen.height;
            _lastPosition = position;
        }
        void DragEnd() {
            _currentPointerId = -1;
            _deltaPosition = _lastPosition = Vector2.zero;
        }



        public string GetUIDragName() {
            return _dragName;
        }

        public float sensivity = 50;
        public Vector2 GetDeltaPosition() {
            return _deltaPosition * sensivity;
        }
    }
}