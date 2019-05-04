using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace KZ.UI {
    public class InputFieldKZ : MonoBehaviour, IPointerDownHandler {

#if UNITY_EDITOR
        [UnityEditor.MenuItem("GameObject/KZ/Input Field", false, 0)]
        static InputFieldKZ Create(UnityEditor.MenuCommand mc) {
            Color normalColor = new Color(0.85f, 0.85f, 0.85f, 1);
            GameObject g = new GameObject("inpKZ_");
            UnityEditor.GameObjectUtility.SetParentAndAlign(g, mc.context as GameObject);
            UnityEditor.Undo.RegisterCreatedObjectUndo(g, "Create " + g.name);
            UnityEditor.Selection.activeObject = g;

            Image image = g.AddComponent<Image>();
            image.sprite = UnityEditor.AssetDatabase
                .GetBuiltinExtraResource<Sprite>("UI/Skin/InputFieldBackground.psd");
            image.type = Image.Type.Sliced;
            image.rectTransform.sizeDelta = new Vector2(160, 30);
            image.color = normalColor;

            Text field = new GameObject("txtField").AddComponent<Text>();
            Text caret = new GameObject("txtCaret").AddComponent<Text>();

            field.transform.SetParent(g.transform);
            caret.transform.SetParent(g.transform);

            field.color = caret.color = Color.black;
            field.fontSize = caret.fontSize = 14;
            field.alignment = caret.alignment = TextAnchor.MiddleLeft;
            field.rectTransform.anchorMin = caret.rectTransform.anchorMin = Vector2.zero;
            field.rectTransform.anchorMax = caret.rectTransform.anchorMax = Vector2.one;
            field.transform.localScale = caret.transform.localScale = Vector3.one;

            Vector2 offset = Vector2.one * 5;
            field.rectTransform.offsetMin = caret.rectTransform.offsetMin = offset;
            field.rectTransform.offsetMax = caret.rectTransform.offsetMax = -offset;

            var kzInp = g.AddComponent<InputFieldKZ>();
            kzInp.settings = new InternalSettings() {
                imgBack = image,
                txtField = field,
                txtCaret = caret
            };
            kzInp.colors = new InpColors() {
                normal = normalColor,
                selected = Color.white
            };

            return kzInp;
        }
#endif

        //FIELDS
        const char caret = '|';
        [SerializeField] InternalSettings settings = null;
        [Serializable]
        class InternalSettings {
            public Image imgBack = null;
            public Text txtField = null;
            public Text txtCaret = null;
        }
        [SerializeField] InpColors colors = null;
        [Serializable]
        class InpColors {
            public Color
                normal = Color.white,
                selected = Color.white;
        }
        public event Action<InputFieldKZ> OnDeselected = delegate { };
        public event Action<InputFieldKZ> OnSelected = delegate { };
        public event Action<InputFieldKZ> OnValueChanged = delegate { };
        bool _isSelected = false;

        int _caretPosition = 0;

        public string text {
            get { return settings.txtField.text; }
            private set { settings.txtField.text = value; RestartCaretAnimation(); }
        }

        //INTERFACE IPointerDownHandler
        public void OnPointerDown(PointerEventData eventData) {
            Select();
        }

        //MODIFY TEXT
        public void SetText(string newText) {
            text = newText;
            settings.txtCaret.text = newText.Colorize(Color.clear) + caret;
            _caretPosition = newText.Length;
            OnValueChanged(this);
        }
        public void AddText(string s) {
            text = text.Insert(_caretPosition, s);
            settings.txtCaret.text = ((_caretPosition += s.Length) >= text.Length ?
                text : text.Remove(_caretPosition - 1))
                .Colorize(Color.clear) + caret;
            OnValueChanged(this);
        }
        public void DeleteChar() {
            if (_caretPosition > 0) {
                text = text.Remove(--_caretPosition, 1);
                settings.txtCaret.text = (_caretPosition >= text.Length ?
                    text : text.Remove(_caretPosition - 1))
                    .Colorize(Color.clear) + caret;
            }
            OnValueChanged(this);
        }
        public void ClearText() {
            text = "";
            settings.txtCaret.text = "" + caret;
            _caretPosition = 0;
            OnValueChanged(this);
        }

        //SELECT
        public void Select() {
            settings.imgBack.color = colors.selected;
            settings.txtCaret.text = text.Colorize(Color.clear) + caret;
            _caretPosition = text.Length;
            _isSelected = true;
            settings.txtCaret.gameObject.SetActive(false);
            RestartCaretAnimation();
            OnSelected(this);
        }
        public void Deselect() {
            settings.imgBack.color = colors.normal;
            StopAllCoroutines();
            _isSelected = false;
            OnDeselected(this);
            settings.txtCaret.gameObject.SetActive(false);
        }



        //CARET ANIMATION
        float caretBlinkRate = 0.75f;
        void RestartCaretAnimation() {
            StopAllCoroutines();
            if (_isSelected)
                StartCoroutine(CR_CaretBlink());
        }
        IEnumerator CR_CaretBlink() {
            settings.txtCaret.gameObject.SetActive(true);
            while (true) {
                yield return new WaitForSeconds(caretBlinkRate);
                settings.txtCaret.gameObject.ToggleActive();
            }
        }

    }
}