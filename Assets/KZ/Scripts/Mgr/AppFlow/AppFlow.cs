using KZ.StateMachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KZ.Managers {
    public class AppFlow : StateMachine<string> {
        private AppFlow() { }

        public static AppFlow Create() {
            var r = new AppFlow();
            return r;
        }

        public void Inititalize() {

            //states
            var intro = new State_Intro(this, "intro");
            var mainMenu = new State_mainMenu(this, "main menu");

            //transitions
            AddTransition(intro, "intro anim ended", mainMenu);

            //TODO change this
            if (DefaultKZValues.settings.skipIntro)
                Begin(mainMenu, true);
            else
                Begin(intro, true);

            //updates
            Locator.behaviourManager.OnUpdate += Update;
            goTest = new GameObject();
            GameObject.DontDestroyOnLoad(goTest);
        }

        public void Dispose() {
            if (Locator.behaviourManager != null)
                Locator.behaviourManager.OnUpdate -= Update;
            GameObject.Destroy(goTest);
        }

        public override void Update() {
            base.Update();
            if (goTest != null)
                goTest.name = "STATE NAME: " + currentState.name;
        }

        //TODO: delete this!
        GameObject goTest;





    }
}