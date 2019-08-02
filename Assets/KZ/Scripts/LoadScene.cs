using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace KZ {
    public class LoadScene : MonoBehaviour {

        private IEnumerator Start() {
            yield return new WaitForEndOfFrame(); // wait 1 frame, to render the images
            OnResetApp();
            DevConsole.AddButton(x => ResetApp(), "reset");
            yield return new WaitForEndOfFrame();
            SceneManager.LoadScene(1); //load first game scene
        }

        ///<summary>Destroy/Clear/Dispose actions</summary>
        public static event Action OnResetApp = delegate { };


        //Loads this first scene
        public static void ResetApp() {
            SceneManager.LoadScene(0);
        }

    }
}