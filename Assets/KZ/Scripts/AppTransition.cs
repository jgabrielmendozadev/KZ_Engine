using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace KZ {
    public class AppTransition : MonoBehaviour {

        public static void Initialize() {
            var appTransition = Instantiate(Resources.Load<AppTransition>("UI/AppTransition"));
            DontDestroyOnLoad(appTransition.gameObject);
            LoadScene.OnResetApp += appTransition.Dispose;
        }

        void Dispose() {
            Destroy(gameObject);
            LoadScene.OnResetApp -= Dispose;
        }

        public bool isTransitioning { get; private set; }

        public void AnimateTransition(float fadeOutTime, float fadeInTime, Action onTransitionPoint, Action onEnd) {
            isTransitioning = true;

            //end transition
            Action onFadeIn = () => {
                isTransitioning = false;
                onEnd?.Invoke();
            };

            //transition point
            Action onFadeOut = () => {
                onTransitionPoint?.Invoke();
                FadeIn(fadeInTime, onFadeIn);
            };

            //start transition
            FadeOut(fadeOutTime, onFadeOut);
        }

        void FadeOut(float time, Action onEnd) {
            StartCoroutine(CR_Fade(0f, 1f, time, onEnd));
        }

        void FadeIn(float time, Action onEnd) {
            StartCoroutine(CR_Fade(1f, 0f, time, onEnd));
        }

        [SerializeField] RawImage imgBlock = null;
        [SerializeField] List<RawImage> allImages = new List<RawImage>();
        void SetValue(float newValue) {
            allImages.ForEach(img => img.color = img.color.SetA(newValue));
            imgBlock.raycastTarget = newValue != 0;
        }

        IEnumerator CR_Fade(float from, float to, float time, Action onEnd) {
            float n = 0;
            float t = 0;
            SetValue(from);
            yield return new WaitForEndOfFrame();
            while (n < 1) {
                t += Time.deltaTime;
                n = (t / time).Clamp();
                SetValue(Mathf.Lerp(from, to, n));
                yield return new WaitForEndOfFrame();
            }
            SetValue(to);
            onEnd?.Invoke();
        }

    }
}