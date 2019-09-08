using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class LocalizableText : MonoBehaviour {

    Text txt;
    [SerializeField] string textID = "";


    public void SetText() {
        txt = GetComponent<Text>();
        txt.text = LocalizationManager.GetInstance().GetText(textID);
    }

    private void OnEnable() => SetText();
    
}