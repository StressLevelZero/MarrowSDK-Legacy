#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

public class MetaFileFixer : EditorWindow {

    static string lastPath = "";

    [MenuItem("Tools/Meta File Updater")]
    public static void Fix()
    {
        string originalAssemblyC = EditorUtility.OpenFolderPanel("Select the original prefab's Assets folder.", lastPath, "");
        lastPath = originalAssemblyC;
        var autoFix = EditorUtility.DisplayDialog("Auto-fix", "Automatically import resources that are missing", "Yes", "No");
        string newAssemblyC = $"{Application.dataPath}";
        MetaFileUpdater.Program.Fix(originalAssemblyC, newAssemblyC, autoFix);
        EditorUtility.DisplayDialog("Program result:", "Finished", "OK");
    }
}
#endif