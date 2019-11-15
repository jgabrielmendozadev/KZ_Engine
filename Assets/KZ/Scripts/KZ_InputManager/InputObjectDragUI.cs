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
            deltaPosition = Vector2.zero;
        }
        void OnDestroy() {
            InputManager.RemoveInputDragUI(this);
        }

        bool usingThis = false;
        Vector2 deltaPosition = Vector2.zero;
        Vector2 lastPosition = Vector2.zero;

        //BEGIN
        public void OnPointerDown(PointerEventData e) {
            DragBegin(e.position);
        }
        //UPDATE VALUE
        public void OnDrag(PointerEventData e) {
            if (usingThis) DragUpdate(e.position);
        }
        //END
        public void OnEndDrag(PointerEventData e) {
            DragEnd();
        }
        //END 
        public void OnPointerUp(PointerEventData e) {
            DragEnd();
        }

        void DragBegin(Vector2 position) {
            usingThis = true;
            lastPosition = position;
        }
        void DragUpdate(Vector2 position) {
            deltaPosition = position - lastPosition;
            deltaPosition.x /= Screen.width;
            deltaPosition.y /= Screen.height;
            lastPosition = position;
        }
        void DragEnd() {
            usingThis = false;
            deltaPosition = lastPosition = Vector2.zero;
        }
        


        public string GetUIDragName() {
            return _dragName;
        }

        public float sensivity = 50;
        public Vector2 GetDeltaPosition() {
            return deltaPosition * sensivity;
        }
    }
}