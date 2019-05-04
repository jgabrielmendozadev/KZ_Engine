using UnityEngine;
using System;
//using Object = UnityEngine.Object;

namespace KZ.GameObjectMoveExtensions {
    public static class MoveExtensions {

        public static void MoveTo(this GameObject go, Vector3 targetPosition, float timeAnim, AnimationCurve curve) {
            foreach (var mh in go.GetComponents<MovementHandler>())
                UnityEngine.Object.Destroy(mh);
            go.AddComponent<MovementHandler>().SetValues(targetPosition, go.transform.rotation, timeAnim, curve, delegate { });
        }
        public static void MoveTo(this GameObject go, Vector3 targetPosition, float timeAnim, AnimationCurve curve, Action onComplete) {
            foreach (var mh in go.GetComponents<MovementHandler>())
                UnityEngine.Object.Destroy(mh);
            go.AddComponent<MovementHandler>().SetValues(targetPosition, go.transform.rotation, timeAnim, curve, onComplete);
        }
        public static void MoveTo(this GameObject go, Vector3 targetPosition, float timeAnim) {
            foreach (var mh in go.GetComponents<MovementHandler>())
                UnityEngine.Object.Destroy(mh);
            go.AddComponent<MovementHandler>().SetValues(targetPosition, go.transform.rotation, timeAnim, AnimationCurve.Linear(0, 0, 1, 1), delegate { });
        }
        public static void MoveTo(this GameObject go, Vector3 targetPosition, float timeAnim, Action onComplete) {
            foreach (var mh in go.GetComponents<MovementHandler>())
                UnityEngine.Object.Destroy(mh);
            go.AddComponent<MovementHandler>().SetValues(targetPosition, go.transform.rotation, timeAnim, AnimationCurve.Linear(0, 0, 1, 1), onComplete);
        }
        public static void MoveTo(this GameObject go, Vector3 targetPosition, Quaternion targetRotation, float timeAnim, AnimationCurve curve) {
            foreach (var mh in go.GetComponents<MovementHandler>())
                UnityEngine.Object.Destroy(mh);
            go.AddComponent<MovementHandler>().SetValues(targetPosition,targetRotation, timeAnim, curve, delegate { });
        }
        public static void MoveTo(this GameObject go, Vector3 targetPosition, Quaternion targetRotation, float timeAnim, AnimationCurve curve, Action onComplete) {
            foreach (var mh in go.GetComponents<MovementHandler>())
                UnityEngine.Object.Destroy(mh);
            go.AddComponent<MovementHandler>().SetValues(targetPosition, targetRotation, timeAnim, curve, onComplete);
        }
        public static void MoveTo(this GameObject go, Vector3 targetPosition, Quaternion targetRotation, float timeAnim) {
            foreach (var mh in go.GetComponents<MovementHandler>())
                UnityEngine.Object.Destroy(mh);
            go.AddComponent<MovementHandler>().SetValues(targetPosition, targetRotation, timeAnim, AnimationCurve.Linear(0, 0, 1, 1), delegate { });
        }
        public static void MoveTo(this GameObject go, Vector3 targetPosition, Quaternion targetRotation, float timeAnim, Action onComplete) {
            foreach (var mh in go.GetComponents<MovementHandler>())
                UnityEngine.Object.Destroy(mh);
            go.AddComponent<MovementHandler>().SetValues(targetPosition, targetRotation, timeAnim, AnimationCurve.Linear(0, 0, 1, 1), onComplete);
        }

        class MovementHandler : MonoBehaviour {
            Vector3 _p0, _p1; //starting position, ending position
            Quaternion _q0, _q1; //starting rotation, ending rotation
            float _t, _totalTime;
            AnimationCurve _curve;
            Action _onComplete;

            public void SetValues(Vector3 _targetPos,Quaternion _targetRotation, float timeAnim, AnimationCurve curve, Action onComplete) {
                _p0 = transform.position;
                _p1 = _targetPos;
                _q0 = transform.rotation;
                _q1 = _targetRotation;
                _t = 0;
                _totalTime = timeAnim;
                _curve = curve;
                _onComplete = onComplete;
            }

            void Update() {
                _t += Time.deltaTime;
                MoveTowardsTargetWithValue(_curve.Evaluate(_t / _totalTime));
                if (_t >= _totalTime) {
                    _onComplete();
                    Destroy(this);
                }
            }

            void MoveTowardsTargetWithValue(float value) {
                transform.position = Vector3.Lerp(_p0, _p1, value);
                transform.rotation = Quaternion.Lerp(_q0, _q1, value);
            }
        }
    }
}