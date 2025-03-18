using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ModelCollection : Singleton<ModelCollection>
{
    public SkinModelsStruct[] skinModels;

#if UNITY_EDITOR
    public UShape_Model GetRandomModel(int first, int last)
    {
        if ((first >= 0 && first < 10)&& (last >= 0 && last < 10) && (first <= last))
        {
            return PrefabUtility.InstantiatePrefab(skinModels[0].models[Random.Range(first, last + 1)]) as UShape_Model;
        }
        return null;
    }
#endif
}