using UnityEngine;

[RequireComponent(typeof(UnityEngine.UI.Text))]
public class LocalizableText : MonoBehaviour {

    [SerializeField] UnityEngine.UI.Text txt;
    [SerializeField] string title;

    //void Start() { GetText(); }

    public void SetText(string s) { txt.text = s; }

    public void GetText() { SetText(LocalizationManager.GetInstance().GetText(title)); }

    private void OnEnable() { GetText(); }
}