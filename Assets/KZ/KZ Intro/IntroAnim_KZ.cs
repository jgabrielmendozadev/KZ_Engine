using System;

namespace KZ {

    public class IntroAnim_KZ : UnityEngine.MonoBehaviour {

        //Static
        public static Action OnAnimationEnd;

        //Instance
        void OnFinishAnimation() {
            OnAnimationEnd?.Invoke();
            OnAnimationEnd = null;
        }

    }

}