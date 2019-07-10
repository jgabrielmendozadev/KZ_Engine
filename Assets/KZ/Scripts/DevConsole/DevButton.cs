using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DevButton : MonoBehaviour, IPointerClickHandler {


    Action<DevButton> _onClick = delegate { };

    [SerializeField] Text _txtTitle = null;
    [SerializeField] RawImage _imgBackground = null;

    public string title {
        get { return _txtTitle.text; }
        set { _txtTitle.text = value; }
    }
    public Color textColor {
        get { return _txtTitle.color; }
        set { _txtTitle.color = value; }
    }
    public Color backColor {
        get { return _imgBackground.color; }
        set { _imgBackground.color = value; }
    }


    public void Initialize() {
        title = "";
        textColor = Color.black;
        backColor = Color.white;
    }

    public DevButton SetAction(Action<DevButton> onClick) {
        _onClick = onClick;
        return this;
    }

    public void OnPointerClick(PointerEventData eventData) {
        _onClick(this);
    }
}