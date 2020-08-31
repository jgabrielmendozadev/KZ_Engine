using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using USM = UnityEngine.SceneManagement.SceneManager;

namespace KZ {
    public static class SceneManager {



        public static void LoadSceneInstant(string sceneName) {
            USM.LoadScene(sceneName);
        }

        public static bool LoadScene(string sceneName, float fadeOutTime = 0.4f, float fadeInTime = 1.0f) {
            var at = AppTransition.instance;
            if (at != null && !at.isTransitioning) {
                at.AnimateTransition(fadeOutTime, fadeInTime, () => USM.LoadScene(sceneName), null);
                return true;
            }
            return false;
        }



        const string
            SPLASH = "0.LoadScene",
            GAME = "1.IntroGame";

        public static void LoadSplashScene() => LoadSceneInstant(SPLASH);
        public static void LoadGameScene() => LoadScene(GAME);

        public static void LoadIntroScene() {
            if (KZ_Settings.GetValue("skipIntro", DefaultKZValues.skipIntro))
                LoadGameScene();
            else
                IntroAnim_KZ.Show(0, 0.5f, LoadGameScene);
        }
    }
}