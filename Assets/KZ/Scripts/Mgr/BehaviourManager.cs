using System;
using UnityEngine;

namespace KZ.Managers.Behaviour {
    public class BehaviourManager : MonoBehaviour, IService {

        static GameObject _go;
        public static BehaviourManager Create() {
            _go = new GameObject();
            DontDestroyOnLoad(_go);
            return _go.AddComponent<BehaviourManager>();
        }

        public void Initialize() { }
        public void Dispose() { Destroy(_go); }


        public event Action OnFixedUpdate = delegate { };
        public event Action OnUpdate = delegate { };
        public event Action OnLateUpdate = delegate { };

        private void FixedUpdate() => OnFixedUpdate();
        private void Update() => OnUpdate();
        private void LateUpdate() => OnLateUpdate();

    }
}