using KZ.Managers;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KZ.Managers.Settings;
using KZ.Managers.Behaviour;
using KZ.StateMachine;

namespace KZ.Managers {
    public static class Locator {

        //leave static:
        //files manager

        // create singleton managers:
        // DONE - kz_settings
        // DONE - devconsole
        // - fade transition manager
        // - events
        // DONE - behaviour
        // ? - settings?
        // - scenemanager
        // - sound manager
        // TEMPORAL - game states / app flow mgr

        public static KZ_Settings settings { get; private set; }
        public static DevConsole devConsole { get; private set; }
        
        public static BehaviourManager behaviourManager { get; private set; }

        public static AppFlow appFlow { get; private set; }

        public static void Initialize() {

            settings = new KZ_Settings();

            devConsole = DevConsole.Create();
            devConsole.Initialize();

            behaviourManager = BehaviourManager.Create();
            behaviourManager.Initialize();

            appFlow = AppFlow.Create();
            appFlow.Inititalize();

            Debug.Log("locator initialized");

        }


        static void Dispose() {

            settings = null;

            devConsole.Dispose();
            devConsole = null;

            behaviourManager.Dispose();
            behaviourManager = null;

            appFlow.Dispose();
            appFlow = null;

            Debug.Log("locator disposed");

        }


    }


    public interface IService {
        void Initialize();
        void Dispose();
    }

}