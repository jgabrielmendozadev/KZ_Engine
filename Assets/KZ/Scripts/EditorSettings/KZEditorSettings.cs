using UnityEngine;

[CreateAssetMenu(fileName = "KZEditorSettingsData", menuName = "ScriptableObjects/KZ Editor Settings", order = 1)]
public class KZEditorSettings : ScriptableObject {
    public bool allowDevConsole;
    public bool skipIntro;
}