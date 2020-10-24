using KZ;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public class KZ_Settings_Window : EditorWindow {

    [MenuItem("KZ Engine/Settings")]
    static void Init() {
        GetWindow<KZ_Settings_Window>().Show(); 
    }

    private void Awake() => LoadDefaultValues();

    public bool allowDevConsole;
    public bool skipIntro;


    private void OnGUI() {
        if (Application.isPlaying) {
            EditorGUILayout.Toggle("Allow dev console", allowDevConsole);
            EditorGUILayout.Toggle("Skip intro", skipIntro);
            GUILayout.Label("Can't modify on play mode");
            return;
        }

        var allowDevConsoleNew = EditorGUILayout.Toggle("Allow dev console", allowDevConsole);
        var skipIntroNew = EditorGUILayout.Toggle("skip intro", skipIntro);

        bool shouldSave = allowDevConsoleNew != allowDevConsole || skipIntroNew != skipIntro;

        allowDevConsole = allowDevConsoleNew;
        skipIntro = skipIntroNew;

        if (shouldSave) 
            SaveDefaultValues();
        if (GUILayout.Button("Save default values")) 
            SaveDefaultValues();


        EditorGUILayout.Space(20);

        EditorGUILayout.BeginHorizontal();
        
        GUILayout.Label("BUILD:");

        if (GUILayout.Button("Build Standalone")) 
            Build(BuildTarget.StandaloneWindows64);

        if (GUILayout.Button("Build Android")) 
            Build(BuildTarget.Android);
        
        EditorGUILayout.EndHorizontal();

        
    }

    void Build(BuildTarget buildTarget) {
        SaveDefaultValues();

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

    void LoadDefaultValues() {
        //FIND FLYWEIGHT OBJ
        var settingsScene = EditorBuildSettings.scenes.First();
        EditorSceneManager.OpenScene(settingsScene.path, OpenSceneMode.Additive);
        var scene = EditorSceneManager.GetSceneByPath(settingsScene.path);
        
        var fws = scene
            .GetRootGameObjects()
            .Select(go => go.GetComponent<DefaultKZValues>())
            .Where(fw => fw != null)
            .ToList();

        //SET VALUES
        if (fws.Count == 1) { //APB CODE
            var defValues = fws.First();
            allowDevConsole = defValues.allow_dev_console;
            skipIntro = defValues.skip_intro;
            Debug.Log("KZ_Settings_Window > default values loaded");
        }
        else {
            Debug.Log("Error trying to find FWDefaultValues in load scene");
        }

        //close scene
        EditorSceneManager.CloseScene(scene, true);
    }

    void SaveDefaultValues() {
        //FIND FLYWEIGHT OBJ
        var settingsScene = EditorBuildSettings.scenes.First();
        EditorSceneManager.OpenScene(settingsScene.path, OpenSceneMode.Additive);
        var scene = EditorSceneManager.GetSceneByPath(settingsScene.path);

        var fws = scene
            .GetRootGameObjects()
            .Select(go => go.GetComponent<DefaultKZValues>())
            .Where(fw => fw != null)
            .ToList();

        //SET VALUES
        if (fws.Count == 1) { //APB CODE
            var defValues = fws.First();
            defValues.allow_dev_console = allowDevConsole;
            defValues.skip_intro = skipIntro;
            EditorSceneManager.SaveScene(scene);
            Debug.Log("KZ_Settings_Window > default values saved");
        }
        else {
            Debug.Log("Error trying to find FWDefaultValues in load scene");
        }

        //close scene
        EditorSceneManager.CloseScene(scene, true);
    }

    void OpenInFolder(string path) {
        bool openInsidesOfFolder = false;

        // try windows
        string winPath = path.Replace("/", "\\"); // windows explorer doesn't like forward slashes

        if (System.IO.Directory.Exists(winPath)) // if path requested is a folder, automatically open insides of that folder
        {
            openInsidesOfFolder = true;
        }

        try
        {
            System.Diagnostics.Process.Start("explorer.exe", (openInsidesOfFolder ? "/root," : "/select,") + winPath);
        }
        catch (System.ComponentModel.Win32Exception e)
        {
            // tried to open win explorer in mac
            // just silently skip error
            // we currently have no platform define for the current OS we are in, so we resort to this
            e.HelpLink = ""; // do anything with this variable to silence warning about not using it
        }
    }
}