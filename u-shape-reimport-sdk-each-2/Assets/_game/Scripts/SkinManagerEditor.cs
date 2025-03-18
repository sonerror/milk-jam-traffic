#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SkinManager))]
public class SkinManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        SkinManager skinManager = (SkinManager)target;

        // Add a button to the inspector
        if (GUILayout.Button("Change Skin"))
        {
            // Call the ChangeSkin method when the button is clicked
            skinManager.ChangeSkin();
        }
    }
}
#endif
