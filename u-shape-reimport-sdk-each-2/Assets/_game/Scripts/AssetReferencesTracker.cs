
#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;
using System.Linq;

public static class AssetReferencesTracker
{
    const string MI_FIND_REFERENCES_IN_PROJECT = "Assets/Find References In Project";
    const string MI_FIND_UNREFERENCED_ASSETS = "Assets/Find Unreferenced Assets";

    #region FIND_UNREFERENCED_ASSETS
    [MenuItem(MI_FIND_UNREFERENCED_ASSETS, false, 26)]
    static void FindUnreferncedAssets()
    {
        string[] selectedFolderPaths =
            Selection.GetFiltered<DefaultAsset>(SelectionMode.Assets)
            .Select(AssetDatabase.GetAssetPath)
            .Where(AssetDatabase.IsValidFolder)
            .ToArray();

        string[] allProviderPaths = GetPathsOfAllDependentcyProviderAsset();
        Object[] unreferencedAssets =
            AssetDatabase.FindAssets("", selectedFolderPaths)
            .Select(AssetDatabase.GUIDToAssetPath)
            .Where(assetPath => !allProviderPaths.Contains(assetPath) && !AssetDatabase.IsValidFolder(assetPath))
            .Select(AssetDatabase.LoadMainAssetAtPath)
            .ToArray();
        ShowObjectsInProjectWindow(unreferencedAssets);
    }

    [MenuItem(MI_FIND_UNREFERENCED_ASSETS, true)]
    static bool FIND_UNREFERENCED_ASSETS_Validate()
    {
        DefaultAsset[] objects = Selection.GetFiltered<DefaultAsset>(SelectionMode.Assets);
        return objects.Length > 0 && objects.Any(obj => AssetDatabase.IsValidFolder(AssetDatabase.GetAssetPath(obj)));
    }
    #endregion

    #region FIND_REFERENCES_IN_PROJECT

    [MenuItem(MI_FIND_REFERENCES_IN_PROJECT, false, 25)]
    static void Find()
    {
        var referenceCache = GetProvider_DependentAssetsDictionary();
        Object activeObject = Selection.activeObject;
        string path = AssetDatabase.GetAssetPath(activeObject);
        Debug.Log("Find refs: " + activeObject.name, activeObject);
        if (referenceCache.ContainsKey(path))
        {
            ShowObjectsInProjectWindow(referenceCache[path].Select(x => AssetDatabase.LoadMainAssetAtPath(x)).ToArray());
        }
        else
        {
            Debug.LogWarning("No references.");
        }

        referenceCache.Clear();
    }
    [MenuItem(MI_FIND_REFERENCES_IN_PROJECT, true)]
    static bool FIND_REFERENCES_IN_PROJECT_Validate()
    {
        Object activeObj = Selection.activeObject;
        return activeObj != null && !AssetDatabase.IsValidFolder(AssetDatabase.GetAssetPath(activeObj));
    }
    #endregion

    static string[] GetPathsOfAllDependentcyProviderAsset()
        => AssetDatabase.FindAssets("")
            .Select(AssetDatabase.GUIDToAssetPath)
            .Where(path => !AssetDatabase.IsValidFolder(path))
            .SelectMany(path => AssetDatabase.GetDependencies(path, false))
            .Distinct().ToArray();

    /// <summary>
    /// Like its name.
    /// </summary>
    /// <returns>
    /// A dictionary with:
    /// - Key -> Path of provider.
    /// - Value -> A set of paths to dependent assets.
    /// </returns>
    static Dictionary<string, HashSet<string>> GetProvider_DependentAssetsDictionary()
    {
        var res = new Dictionary<string, HashSet<string>>();

        foreach (string guid in AssetDatabase.FindAssets(""))
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            string[] dependencies = AssetDatabase.GetDependencies(assetPath, false);
            foreach (var dependency in dependencies)
            {
                if (res.ContainsKey(dependency))
                {
                    res[dependency].Add(assetPath);
                }
                else
                {
                    res[dependency] = new HashSet<string>() { assetPath };
                }
            }
        }
        return res;
    }

    static void ShowObjectsInProjectWindow(IEnumerable<Object> objects)
    {
        int[] instanceIDs = objects.Select(o => o.GetInstanceID()).ToArray();
        System.Type prjBrowserType = typeof(Editor).Assembly.GetType("UnityEditor.ProjectBrowser");
        Object[] prjBrowsers = Resources.FindObjectsOfTypeAll(prjBrowserType);
        MethodInfo method = prjBrowserType.GetMethod("ShowObjectsInList", BindingFlags.Instance | BindingFlags.NonPublic);

        if (prjBrowsers.Length == 0)
        {
            ShowObjectsInternal(
                OpenNewProjectBrowser(prjBrowserType),
                method,
                instanceIDs
            );
        }
        else
        {
            foreach (var prjBrowser in prjBrowsers)
            {
                ShowObjectsInternal(
                    prjBrowser,
                    method,
                    instanceIDs
                );
            }
        }
    }

    static void ShowObjectsInternal(Object projectBrowser, MethodInfo method, int[] instanceIDs)
    {
        // Set 2 column layout
        if (new SerializedObject(projectBrowser).FindProperty("m_ViewMode").enumValueIndex != 1)
        {
            projectBrowser
                .GetType()
                .GetMethod("SetTwoColumns", BindingFlags.Instance | BindingFlags.NonPublic)
                .Invoke(projectBrowser, null);
        }

        method.Invoke(projectBrowser, new[] { instanceIDs });
    }

    static EditorWindow OpenNewProjectBrowser(System.Type projectBrowserType)
    {
        EditorWindow projectBrowser = EditorWindow.GetWindow(projectBrowserType);
        projectBrowser.Show();
        MethodInfo init = projectBrowserType.GetMethod("Init", BindingFlags.Instance | BindingFlags.Public);
        init.Invoke(projectBrowser, null);
        return projectBrowser;
    }
}
#endif