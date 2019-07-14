using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

class MyCustomBuildProcessor : IPreprocessBuildWithReport {

    public int callbackOrder { get { return default(int); } }

    public void OnPreprocessBuild(BuildReport report) {

        string
            pathEditor = KZ.FilesManager.GetPath_GameFiles(),
            pathBuild = Path.Combine(Path.GetDirectoryName(report.summary.outputPath), KZ.FilesManager.folderName);

        Directory.CreateDirectory(pathEditor);
        if (Directory.Exists(pathBuild))
            Directory.Delete(pathBuild, true);

        FileUtil.CopyFileOrDirectory(pathEditor, pathBuild);

        Debug.Log("Files copied from \"" + pathEditor + "\" to \"" + pathBuild + "\"");
    }

}