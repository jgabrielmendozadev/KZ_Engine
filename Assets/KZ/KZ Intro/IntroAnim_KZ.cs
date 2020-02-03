using System;

namespace KZ {

    public class IntroAnim_KZ : UnityEngine.MonoBehaviour {

        //Const
        const string SCENE_NAME = "0.KZ";

        //Static
        static Action _OnAnimationEnd;
        public static void Show(float fadeOutTime, float fadeInTime, Action OnAnimationEnd) {
            _OnAnimationEnd = OnAnimationEnd;
            SceneManager.LoadScene(SCENE_NAME, fadeOutTime, fadeInTime);
        }

        //Instance
        void OnFinishAnimation() {
            _OnAnimationEnd?.Invoke();
            _OnAnimationEnd = null;
        }


    }

}