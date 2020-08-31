using KZ;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;
using UnityEngine;

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
        //buildPlayerOptions.scenes = new[] { "Assets/Scene1.unity", "Assets/Scene2.unity" };

        var path = "KZBUILD/" + buildTarget.ToString();
        if (Directory.Exists(path)) Directory.Delete(path);

        buildPlayerOptions.locationPathName = path + "/jubileitor" + fileExtension;
        buildPlayerOptions.target = buildTarget;
        buildPlayerOptions.options = BuildOptions.None;

        BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
        BuildSummary summary = report.summary;

        if (summary.result == BuildResult.Succeeded) {
            Debug.Log("Build succeeded: " + summary.totalSize + " bytes, " + buildTarget.ToString());
            Debug.Log(summary.outputPath);
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
}