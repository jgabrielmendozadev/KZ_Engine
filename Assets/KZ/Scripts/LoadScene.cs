using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace KZ {
    public class LoadScene : MonoBehaviour {

        private IEnumerator Start() {
            yield return new WaitForEndOfFrame(); // wait 1 frame, to render the images

            InitializeGameSettings();
            OnResetApp();
            DevConsole.AddButton(x => ResetApp(), "reset");
            
            yield return new WaitForEndOfFrame();
            SceneManager.LoadScene(1); //load first game scene
        }

        static void InitializeGameSettings() {

            //Init KZ_SETTINGS (reading logs, to show them in the dev console)
            List<Action> logs = new List<Action>();
            Application.LogCallback onLogReceived = (c, s, t) => logs.Add(() => DevConsole.PrintLog(c, s, t));
            Application.logMessageReceived += onLogReceived;
            KZ_Settings.Initialize();
            Application.logMessageReceived -= onLogReceived;

            //Init DEVELOPER CONSOLE
            DevConsole.Initialize();
            if (KZ_Settings.GetValue("useDevConsole", false))
                logs.ForEach(l => l());


        }

#if UNITY_EDITOR
        public static bool isInitialized = false;
        [RuntimeInitializeOnLoadMethod]
        static void InitializeGame() {
            SceneManager.LoadScene(0);
            OnResetApp += () => isInitialized = true;
        }
#endif


        ///<summary>Destroy/Clear/Dispose actions</summary>
        public static event Action OnResetApp = delegate { };


        //Loads this first scene
        public static void ResetApp() {
            Debug.Log("App reset");
            SceneManager.LoadScene(0);
        }

    }
}