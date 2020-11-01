using KZ.Managers;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using SceneManager = KZ.Managers.SceneManager;

namespace KZ {
    public class LoadScene : MonoBehaviour {

        private IEnumerator Start() {

            yield return new WaitForEndOfFrame(); // wait 1 frame, to render the splash image

            OnResetApp();
            InitializeGameSettings();
            AppTransition.Initialize();

            //TODO: move this to locator? app flow?
            //DevConsole.AddButton(x => ResetApp(), "reset"); 
            Locator.Initialize();
        }

        static void InitializeGameSettings() { }

#if UNITY_EDITOR
        [RuntimeInitializeOnLoadMethod]
        static void InitializeGame() {
            Debug.Log("App INIT");
            SceneManager.LoadSceneInstant(SceneManager.SPLASH);
        }
#endif


        ///<summary>Destroy/Clear/Dispose actions</summary>
        public static event Action OnResetApp = delegate { };


        //Loads this first scene
        public static void ResetApp() {
            Debug.Log("App reset");
            SceneManager.LoadScene(SceneManager.SPLASH);
        }

    }
}