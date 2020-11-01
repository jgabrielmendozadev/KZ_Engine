using USM = UnityEngine.SceneManagement.SceneManager;

namespace KZ.Managers {
    public static class SceneManager {



        public static void LoadSceneInstant(string sceneName) {
            USM.LoadScene(sceneName);
        }

        /// <summary>example: LoadScene(SceneManager.INTRO)</summary>
        public static bool LoadScene(string sceneName, float fadeOutTime = 0.4f, float fadeInTime = 1.0f) {
            var at = AppTransition.instance;
            if (at != null && !at.isTransitioning) {
                at.AnimateTransition(fadeOutTime, fadeInTime, () => USM.LoadScene(sceneName), null);
                return true;
            }
            return false;
        }



        public const string
            SPLASH = "0.LoadScene",
            INTRO_KZ = "0.KZ",
            MAIN_MENU = "1.MainMenu";
        

    }
}