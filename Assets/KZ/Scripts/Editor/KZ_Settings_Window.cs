using UnityEditor;

public class KZ_Settings_Window : EditorWindow {

    [MenuItem("KZ Engine/Settings")]
    static void Init() { GetWindow<KZ_Settings_Window>().Show(); }


    public bool allowDevConsole;

    private void OnGUI() {
        allowDevConsole = EditorGUILayout.Toggle("Allow dev console", allowDevConsole);
        EditorPrefs.SetBool("AllowDevConsole", allowDevConsole);
    }


    
}