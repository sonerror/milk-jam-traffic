#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.UIElements;
using System.Linq;
using UnityEditor;

public class SaveLevelData : Singleton<SaveLevelData>
{
    public Level[] levels;

    [Button]
    public void SaveLevelDatas()
    {
        StartCoroutine(I_SaveLevelDatas());
    }

    public IEnumerator I_SaveLevelDatas()
    {
        for (int i = 0; i < levels.Length; i++)
        {
            GameObject levelObj = Instantiate(levels[i].gameObject);
            //LevelManager.Ins.InstantiateLevel(i);
            //GameObject levelObj = FindObjectOfType<Level>().gameObject;
            LevelData levelData = LevelDataCollection.Ins.levelDatas[i];

            Debug.Log(levelData);
            Debug.Log(levelData.transformDataList);

            UShape[] ushapes = GameObject.FindObjectsOfType<UShape>();
            List<UShape> ushapeList = ushapes.ToList();

            ushapes = GameObject.FindObjectsOfType<UShape>();

            if(ushapes.Length > 0) // neu sua level thi moi save (vi leveldata da co data r)
            {
                levelData.transformDataList.Clear();
                Debug.Log("ushapes.Length " + ushapes.Length);
                foreach (UShape u in ushapes)
                {
                    levelData.transformDataList.Add(new TransformData(u.transform.position, u.transform.eulerAngles, u.transform.lossyScale));
                    EditorUtility.SetDirty(levelData);
                }
                yield return new WaitForEndOfFrame();
                yield return new WaitForEndOfFrame();
                yield return new WaitForEndOfFrame();
                Destroy(levelObj);
            }

            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
        }
        yield return null;
    }
}
#endif