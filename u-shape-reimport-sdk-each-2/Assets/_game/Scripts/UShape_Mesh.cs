using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UShape_Mesh : MonoBehaviour
{
    public UShape uShape;

    private void Start()
    {
        uShape = GetComponentInParent<UShape>();
    }

}
