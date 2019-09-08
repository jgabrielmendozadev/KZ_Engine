using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace KZ.Keyboard {
    public class KeyboardKey : Selectable {

        [Space(10, order = 0)]
        [Header("References", order = 1)]
        //[SerializeField] Image _imgBack = null;
        [SerializeField] Text _txtTitle = null;


        public enum KeyType { letter, special }
        [Header("Settings")]
        [SerializeField] KeyType _keyType = KeyType.letter;
        [SerializeField] string _title = "";
        [SerializeField] int _textSize = 30;

        //if letter
        [SerializeField] string _textToAdd = "";
        //if special
        [SerializeField] SpecialKeyboardKey _specialKey = SpecialKeyboardKey.Backspace;



        CompleteKeyboard _myKB;
        public void SetKeyboard(CompleteKeyboard keyboard) {
            _myKB = keyboard;
        }


        public override void OnPointerDown(PointerEventData evData) {
            base.OnPointerDown(evData);
            actions[_keyType](_myKB, this);
        }

        static readonly Dictionary<KeyType, Action<CompleteKeyboard, KeyboardKey>> actions =
            new Dictionary<KeyType, Action<CompleteKeyboard, KeyboardKey>>() {
                { KeyType.letter, (kb, k) => kb.KeyPressed(k._textToAdd) },
                { KeyType.special, (kb, k) => kb.KeyPressed(k._specialKey) }
            };


#if UNITY_EDITOR
        protected override void OnValidate() {
            base.OnValidate();
            _txtTitle.text = _title;
            _txtTitle.fontSize = _textSize;
        }
#endif


#if UNITY_EDITOR
        [CustomEditor(typeof(KeyboardKey))]
        public class KeyboardKeyEditor : Editor {
        
            public override void OnInspectorGUI() {
                DrawPropertiesExcluding(serializedObject, "_textToAdd", "_special", "m_SpriteState", "m_AnimationTriggers");
                EditorGUILayout.PropertyField(serializedObject.FindProperty(
                    (serializedObject.FindProperty("_keyType").enumValueIndex == 0) ? "_textToAdd" : "_special"
                ));
                serializedObject.ApplyModifiedProperties();
                serializedObject.Update();
            }
        }
#endif

    }

}