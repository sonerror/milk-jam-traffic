using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextOutline : MonoBehaviour
{
    public TextMeshProUGUI txt;
    public float width;
    public Color color;





    private void Awake()
    {
        txt = GetComponent<TextMeshProUGUI>();
    }

    private void OnValidate()
    {
        txt.outlineWidth = width;   
        txt.color = color;
    }
}
