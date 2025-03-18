using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Level Data", order = 1)]
public class LevelData : ScriptableObject
{
    [SerializeField]
    public List<TransformData> transformDataList = new List<TransformData>();
}

[Serializable]
public class TransformData
{
    public Vector3 position;
    public Vector3 eulerAngle;
    public Vector3 scale;

    public TransformData(Vector3 position, Vector3 eulerAngle, Vector3 scale)
    {
        this.position = position;
        this.eulerAngle = eulerAngle;
        this.scale = scale;
    }
}
