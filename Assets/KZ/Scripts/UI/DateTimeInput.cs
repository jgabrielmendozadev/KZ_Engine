using System;
using UnityEngine;
using UnityEngine.UI;

namespace KZ.UI {

    public class DateTimeInput : MonoBehaviour {

        #region SINGLETON
        public static DateTimeInput instance { get; private set; }
        void AwakeSingleton() {
            if (instance != null) Destroy(instance.gameObject);
            instance = this;
        }
        #endregion

        [SerializeField]
        GameObject _container = null;
        [SerializeField]
        InputField
            inpY = null,
            inpM = null,
            inpD = null,
            inph = null,
            inpm = null,
            inps = null;

        DateTime dt = DateTime.Now;

        private void Awake() {
            AwakeSingleton();
            CloseInputDate();
            Subscribe();
        }

        void Subscribe() {
            inpY.onValueChanged.AddListener(ValidateDateTime);
            inpM.onValueChanged.AddListener(ValidateDateTime);
            inpD.onValueChanged.AddListener(ValidateDateTime);
            inph.onValueChanged.AddListener(ValidateDateTime);
            inpm.onValueChanged.AddListener(ValidateDateTime);
            inps.onValueChanged.AddListener(ValidateDateTime);
        }
        void Unsubscribe() {
            inpY.onValueChanged.RemoveListener(ValidateDateTime);
            inpM.onValueChanged.RemoveListener(ValidateDateTime);
            inpD.onValueChanged.RemoveListener(ValidateDateTime);
            inph.onValueChanged.RemoveListener(ValidateDateTime);
            inpm.onValueChanged.RemoveListener(ValidateDateTime);
            inps.onValueChanged.RemoveListener(ValidateDateTime);
        }
        private void ValidateDateTime(string p) {
            int Y = IntTryParse(inpY.text, dt.Year);
            int M = IntTryParse(inpM.text, dt.Month).Clamp(1, 12);
            int D = IntTryParse(inpD.text, dt.Day).Clamp(1, DateTime.DaysInMonth(Y, M));

            int h = IntTryParse(inph.text, dt.Hour).Clamp(0, 23);
            int m = IntTryParse(inpm.text, dt.Minute).Clamp(0, 59);
            int s = IntTryParse(inps.text, dt.Second).Clamp(0, 59);
            dt = new DateTime(Y, M, D, h, m, s);
            PasteValues();
        }

        void PasteValues() {
            Unsubscribe();
            inpY.text = dt.Year.ToString("0");
            inpM.text = dt.Month.ToString("0");
            inpD.text = dt.Day.ToString("0");
            inph.text = dt.Hour.ToString("0");
            inpm.text = dt.Minute.ToString("0");
            inps.text = dt.Second.ToString("0");
            Subscribe();
        }

        int IntTryParse(string s, int defaultValue = 0) {
            int r = defaultValue;
            int.TryParse(s, out r);
            return r;
        }


        event Action<DateTime> OnSubmit = delegate { };

        public void BtnSubmit() {
            OnSubmit(dt);
            CloseInputDate();
        }

        public void OpenInputDate(DateTime dateTime, Action<DateTime> OnSubmit) {
            dt = dateTime;
            PasteValues();
            _container.SetActive(true);
            this.OnSubmit = OnSubmit;
        }
        public void CloseInputDate() {
            OnSubmit = delegate { };
            _container.SetActive(false);
        }
    }
}