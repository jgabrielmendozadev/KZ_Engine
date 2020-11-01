using KZ.Managers;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KZ.Managers.Settings;
using KZ.Managers.Behaviour;

namespace KZ.Managers {
    public static class Locator {

        //leave static:
        //files manager - STATIC

        //create singleton managers:
        //kz_settings - DONE
        //devconsole
        //events
        //behaviour - DONE
        //settings
        //scenemanager
        //fade transition manager
        //sound manager
        //game states / app flow mgr

        public static KZ_Settings settings { get; private set; }

        public static BehaviourManager behaviourManager { get; private set; }

        static void Initialize() {

            settings = new KZ_Settings();

            behaviourManager = BehaviourManager.Create();
            if (behaviourManager != null) behaviourManager.Initialize();
        }

        static void Dispose() {
            behaviourManager?.Dispose();
            behaviourManager = null;
        }

    }


    public interface IService {
        void Initialize();
        void Dispose();
    }
}