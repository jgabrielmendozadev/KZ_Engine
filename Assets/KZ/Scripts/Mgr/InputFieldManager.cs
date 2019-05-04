using System;
using System.Linq;
using UnityEngine;
using KZ;
using KZ.UI;
using KZ.Keyboard;

public class InputFieldManager : MonoBehaviour {

    [SerializeField] InputFieldKZ[] inputFields = null;
    [SerializeField] CompleteKeyboard _myKB = null;
    
    InputFieldKZ lastInp = null;

    private void Awake() {
        //when one is selected, deselect the others
        Action<InputFieldKZ> onSel = x => {
            lastInp = x;
            inputFields.Where(y => y != x).ToAll(z => z.Deselect());
        };
        inputFields.ToAll(x => x.OnSelected += onSel);
        _myKB.OnKeyPressed += OnKeyPressed;
        _myKB.OnSKeyPressed += OnSKeyPressed;
    }

    private void OnEnable() {
        if (inputFields != null && inputFields.Any())
            inputFields.First().Select();
    }

    //Key pressed
    void OnKeyPressed(string s) {
        if (lastInp == null) return;
        lastInp.AddText(s);
    }

    //Special key pressed (backspace, clear)
    void OnSKeyPressed(SpecialKeyboardKey s) {
        if (lastInp == null) return;
        switch (s) {
            case SpecialKeyboardKey.Clear: lastInp.ClearText(); break;
            case SpecialKeyboardKey.Backspace: lastInp.DeleteChar(); break;
        }
    }
}



