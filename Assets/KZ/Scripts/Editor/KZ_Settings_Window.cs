using KZ;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

public class KZ_Settings_Window : EditorWindow {

    [MenuItem("KZ Engine/Settings")]
    static void Init() { GetWindow<KZ_Settings_Window>().Show(); }

    public KZEditorSettings currSettings;

    private Editor currentDataObjectEditor; 
    private void OnEnable() {
        currentDataObjectEditor = Editor.CreateEditor(currSettings);
    }
    private void OnGUI() {
        
        if (Application.isPlaying) {
            GUILayout.Label("Can't modify on play mode");
            return;
        }

        //SETTINGS
        currentDataObjectEditor.OnInspectorGUI();

        //BUILD
        EditorGUILayout.Space(20); 
        
        GUILayout.Label("BUILD:"); 

        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("Build Standalone"))
            Build(BuildTarget.StandaloneWindows64);

        if (GUILayout.Button("Build Android"))
            Build(BuildTarget.Android);

        EditorGUILayout.EndHorizontal();
    }

    void Build(BuildTarget buildTarget) {

        string fileExtension = "";

        switch (buildTarget) {
            case BuildTarget.Android: fileExtension = ".apk"; break;
            case BuildTarget.StandaloneWindows64: fileExtension = ".exe"; break;
            default: break;
        }

        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();

        var appName = Application.productName;
        var version = System.DateTime.Now.ToKzFormatNoStrings();
        buildPlayerOptions.locationPathName = "KZBUILD/" + buildTarget.ToString() + "/" + appName + "_" + version + fileExtension;
        buildPlayerOptions.target = buildTarget;
        buildPlayerOptions.options = BuildOptions.None;
        buildPlayerOptions.scenes = EditorBuildSettings.scenes
           .Where(s => s.enabled)
           .Select(s => s.path)
           .ToArray();

        BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
        BuildSummary summary = report.summary;

        if (summary.result == BuildResult.Succeeded) {
            Debug.Log("Build succeeded: " + summary.totalSize + " bytes, " + buildTarget.ToString());
            Debug.Log(summary.outputPath);
            Debug.Log("build time: " + (summary.buildEndedAt - summary.buildStartedAt).TotalSeconds);
            OpenInFolder(summary.outputPath);
        }

        if (summary.result == BuildResult.Failed) {
            Debug.Log("Build failed");
        }

    }

    void OpenInFolder(string path) {
        bool openInsidesOfFolder = false;

        // try windows
        string winPath = path.Replace("/", "\\"); // windows explorer doesn't like forward slashes

        if (Directory.Exists(winPath))  // if path requested is a folder, automatically open insides of that folder
            openInsidesOfFolder = true;
        
        try {
            System.Diagnostics.Process.Start("explorer.exe", (openInsidesOfFolder ? "/root," : "/select,") + winPath);
        }
        catch (System.ComponentModel.Win32Exception e) {
            // tried to open win explorer in mac
            // just silently skip error
            // we currently have no platform define for the current OS we are in, so we resort to this
            e.HelpLink = ""; // do anything with this variable to silence warning about not using it
        }
    }
}