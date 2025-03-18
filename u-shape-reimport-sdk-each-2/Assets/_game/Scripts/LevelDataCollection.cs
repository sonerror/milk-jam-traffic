using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class LevelDataCollection : Singleton<LevelDataCollection>
{
    public LevelData[] levelDatas;

#if UNITY_EDITOR
    [Button()]
    public void Save()
    {
        foreach (var levelData in levelDatas)
        {
            EditorUtility.SetDirty(levelData);
        }
    }
#endif
}
