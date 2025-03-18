using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
[CustomEditor(typeof(UShape))]
public class UshapeEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        UShape uShape = (UShape)target;
        /*
                #region long bar scale
                float longBarScale = uShape.longBar.transform.localScale.x;
                float previousScale = uShape.longBar.transform.localScale.x;
                EditorGUI.BeginChangeCheck();
                longBarScale = EditorGUILayout.Slider("Long Bar Scale", longBarScale, 1, 20);
                uShape.longBar.transform.localScale = new Vector3(longBarScale, 1, 1);
                if (EditorGUI.EndChangeCheck() && longBarScale != previousScale) // Kiểm tra nếu giá trị slider đã thay đổi
                {
                    float offset = longBarScale - previousScale;
                    uShape.shortBar1.transform.localPosition -= new Vector3(offset / 2, 0, 0);
                    uShape.shortBar2.transform.localPosition += new Vector3(offset / 2, 0, 0);
                }
                #endregion

                #region short bar scale
                float shortBarScale = uShape.shortBar1.transform.localScale.y;
                float previousScalee = uShape.shortBar1.transform.localScale.y;
                EditorGUI.BeginChangeCheck();
                shortBarScale = EditorGUILayout.Slider("Short Bar Scale", shortBarScale, 1, 10);
                uShape.shortBar1.transform.localScale = new Vector3(1, shortBarScale, 1);
                uShape.shortBar2.transform.localScale = new Vector3(1, shortBarScale, 1);
                if (EditorGUI.EndChangeCheck() && shortBarScale != previousScale) // Kiểm tra nếu giá trị slider đã thay đổi
                {
                    float offset = shortBarScale - previousScalee;
                    uShape.longBar.transform.localPosition += new Vector3(0, offset / 2, 0);
                }
                #endregion

                #region color
                MeshRenderer[] meshRenderers = uShape.GetComponentsInChildren<MeshRenderer>();
                foreach (MeshRenderer meshRenderer in meshRenderers)
                {
                    meshRenderer.material = MaterialCollection.Ins.GetMat(uShape.matType);
                }
                #endregion
        */

        #region rotate u shape
        if (GUILayout.Button("Rotate X - RED"))
        {
            uShape.TF.RotateAroundAxis(90f, Vector3.right);
        }
        if (GUILayout.Button("Rotate Y - GREEN"))
        {
            uShape.TF.RotateAroundAxis(90f, Vector3.up);
        }
        if (GUILayout.Button("Rotate Z - BLUE"))
        {
            uShape.TF.RotateAroundAxis(90f, Vector3.forward);
        }
        #endregion

        #region reset local rotation
        if (GUILayout.Button("Reset Local Rotation"))
        {
            uShape.transform.localRotation = Quaternion.identity;
        }
        #endregion

        #region log transform
        if (GUILayout.Button("Log transform"))
        {
            Debug.Log(uShape.gameObject.name);
            Debug.Log(uShape.gameObject.transform.position);
            Debug.Log(uShape.gameObject.transform.rotation.eulerAngles);
            Debug.Log(uShape.gameObject.transform.lossyScale);
        }
        #endregion
    }


}
#endif