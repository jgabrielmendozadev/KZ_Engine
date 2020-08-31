using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace KZ {
    public class InputObjectFloatingJoystickUI : InputObjectJoystickUI {

        protected override void Start() {
            base.Start();
            inputGraphic.gameObject.SetActive(false);
            backGraphic.gameObject.SetActive(false);
        }
        protected override void BeginValue(Vector2 position) {
            backGraphic.position = position;
            inputGraphic.gameObject.SetActive(true);
            backGraphic.gameObject.SetActive(true);
            base.BeginValue(position);
        }
        protected override void UpdateValue(Vector2 position) {
            base.UpdateValue(position);
        }
        protected override void EndValue() {
            inputGraphic.gameObject.SetActive(false);
            backGraphic.gameObject.SetActive(false);
            base.EndValue();
        }

    }
}