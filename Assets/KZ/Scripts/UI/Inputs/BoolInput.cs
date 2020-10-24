using System;
using UnityEngine;
using UnityEngine.UI;

namespace KZ.UI {
    public class BoolInput : MonoBehaviour {

        #region SINGLETON
        public static BoolInput instance { get; private set; }
        void AwakeSingleton() {
            if (instance != null) Destroy(instance.gameObject);
            instance = this;
        }
        #endregion

        [SerializeField] GameObject _container = null;
        [SerializeField] Text _txtTitle = null;


        void Awake() {
            AwakeSingleton();
            CloseInput();
        }

        event Action<bool> OnSubmit = delegate { };

        public void OpenInput(string title, Action<bool> OnSubmit) {
            _container.SetActive(true);
            _txtTitle.text = title;
            this.OnSubmit = OnSubmit;
        }

        public void CloseInput() {
            OnSubmit = delegate { };
            _container.SetActive(false);
        }

        public void BtnAnswer(bool answer) {
            OnSubmit(answer);
            CloseInput();
        }
    }
}