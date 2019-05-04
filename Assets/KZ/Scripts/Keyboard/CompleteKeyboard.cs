using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace KZ.Keyboard {

    public enum SpecialKeyboardKey { Clear, Backspace }
    public class CompleteKeyboard : MonoBehaviour {

        void Awake() {
            OnKeyPressed += delegate { PlayKeySound(); };
            OnSKeyPressed += delegate { PlayKeySound(); };
            Awake_Keys();
        }
        public void BtnClean() { OnSKeyPressed(SpecialKeyboardKey.Clear); }
        public void BtnBackspace() { OnSKeyPressed(SpecialKeyboardKey.Backspace); }

        public void KeyPressed(string t) { OnKeyPressed(t); }
        public void KeyPressed(Text txtString) { OnKeyPressed(txtString.text); }
        public void KeyPressed(SpecialKeyboardKey sKey) { OnSKeyPressed(sKey); }

        public event Action<string> OnKeyPressed = delegate {  };
        public event Action<SpecialKeyboardKey> OnSKeyPressed = delegate { };

        void PlayKeySound() { KZ.Audio.SoundManager.SFX_Play(sfx_click); }

        [SerializeField] AudioClip sfx_click = null;
        [SerializeField] List<KeyboardKey> _keys;

        void Awake_Keys() {
            foreach (var key in _keys)
                key.SetKeyboard(this);
        }

        #region KEYBOARD SETTINGS
        [Header("SETTINGS")]
        [SerializeField] bool editKeyboard = false;
        [SerializeField]
        ColorBlock cb = new ColorBlock() {
            normalColor = Color.white,
            highlightedColor = Color.white,
            pressedColor = Color.white,
            disabledColor = Color.white,
            colorMultiplier = 1,
            fadeDuration = 0.1f,
        };
        [SerializeField] Color _textColor = Color.black;
        [SerializeField] Font _font;


        private void OnValidate() {
            if (!editKeyboard) return;

            foreach (var k in _keys) {
                k.colors = cb;
                foreach (var txt in k.transform.GetComponentsInChildren<Text>()) {
                    txt.color = _textColor;
                    txt.font = _font;
                }
            }

        }
        #endregion
    }
}