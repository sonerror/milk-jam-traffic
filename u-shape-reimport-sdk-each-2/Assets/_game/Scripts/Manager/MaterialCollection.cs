using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialCollection : Singleton<MaterialCollection>
{
    public Material[] mats;
    public Material GetMat(MatType type)
    {
        return mats[(int)type];
    }

    public Material GetRandomMat(MatType[] mats)
    {
        int rd = UnityEngine.Random.Range(0, mats.Length);
        return GetMat(mats[rd]);
        
    }
    
}
