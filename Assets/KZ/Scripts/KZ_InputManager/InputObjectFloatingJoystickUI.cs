using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace KZ {
    public class InputObjectFloatingJoystickUI : InputObjectJoystickUI {

        protected override void Start() {
            inputGraphic.gameObject.SetActive(false);
            backGraphic.gameObject.SetActive(false);
            base.Start();
        }
        protected override void BeginValue(Vector2 position) {
            inputGraphic.gameObject.SetActive(true);
            backGraphic.gameObject.SetActive(true);
            backGraphic.position = position;
            UpdateValue(position);
        }
        protected override void UpdateValue(Vector2 position) {
            Vector3 pos = new Vector3();

            pos.x = (position.x / Screen.width) * rtCanvas.sizeDelta.x * backGraphic.lossyScale.x;
            pos.y = (position.y / Screen.height) * rtCanvas.sizeDelta.y * backGraphic.lossyScale.y;

            var radius = backGraphic.sizeDelta.x * backGraphic.lossyScale.x * 0.5f;
            var dir = Vector3.ClampMagnitude(pos - backGraphic.position, radius);

            inputGraphic.position = backGraphic.position + dir;

            currentValue = dir / radius;
        }
        protected override void EndValue() {
            inputGraphic.gameObject.SetActive(false);
            backGraphic.gameObject.SetActive(false);
            currentValue = Vector2.zero;
            inputGraphic.localPosition = Vector3.zero;
        }

    }
}