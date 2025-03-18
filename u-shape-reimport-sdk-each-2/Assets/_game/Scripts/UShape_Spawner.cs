using System.Collections;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
public enum ContainerType
{
    Sphere, Cube
}

public class UShape_Spawner : EditorWindow
{
    UShape objToSpawn;
    UShape currentUshape; 
    Transform container;
    Transform[] spawnPositions;
    Transform spawnPos;
    int firstSizeIndex;
    int lastSizeIndex;
    int spawnQuantity;
    string prefabName;
    int spawnedCount;
    public static bool isSpawing;
    //sphere
    float spawnRadius;
    //cube
    float lengthX;
    float lengthY;
    float lengthZ;
    ContainerType containerType;
    Vector3 previousUshapePos;
    LevelData levelData;
    int count;

    [MenuItem("Tools/UShape Spawner")]
    public static void ShowWindow()
    {
        GetWindow(typeof(UShape_Spawner));
    }

    private void OnGUI()
    {
        //firstSizeIndex = EditorGUILayout.IntField("First Size Index : ", firstSizeIndex);
        //lastSizeIndex = EditorGUILayout.IntField("Last Size Index : ", lastSizeIndex);
        spawnQuantity = EditorGUILayout.IntField("Spawn Quantity : ", spawnQuantity);

        if (firstSizeIndex > 9)
        {
            firstSizeIndex = 9;
        }
 
        if (lastSizeIndex > 9)
        {
            lastSizeIndex = 9;
        }

        containerType = (ContainerType)EditorGUILayout.EnumPopup("Container Type : ",containerType);

        switch (containerType)
        {
            case ContainerType.Sphere:
                spawnRadius = EditorGUILayout.Slider("Spawn radius : ", spawnRadius, 0, 5);
                break;
            case ContainerType.Cube:
                lengthX = EditorGUILayout.FloatField("Length X : ",lengthX);
                lengthY = EditorGUILayout.FloatField("Length Y : ",lengthY);
                lengthZ = EditorGUILayout.FloatField("Length Z : ",lengthZ);
                break;
        }

        container = EditorGUILayout.ObjectField("container : ", container, typeof(Transform), true) as Transform;
        levelData = EditorGUILayout.ObjectField("level data : ", levelData, typeof(LevelData), true) as LevelData;
        //spawnPos = EditorGUILayout.ObjectField("spawnPos : ", spawnPos, typeof(Transform), true) as Transform;
        if(container != null) spawnPositions = container.GetChilds();
        if(spawnPositions==null || spawnPositions.Length == 0)
        {
            spawnPositions = new Transform[1];
            spawnPositions[0] = container;
        }

        EditorGUILayout.LabelField("==================================================================================================================");
        if (GUILayout.Button("SPAWN RANDOM OBJECTS"))
        {
            ModelCollection.Ins.StartCoroutine(I_SpawnObjectsRandom());
        }

        if (GUILayout.Button("CANCEL SPAWN"))
        {
            ModelCollection.Ins.StopAllCoroutines();
            isSpawing = false;
        }

        EditorGUILayout.LabelField("==================================================================================================================");
        if (GUILayout.Button("DELETE ALL USHAPES"))
        {
            UShape[] objs = GameObject.FindObjectsOfType<UShape>();
            foreach (UShape obj in objs)
            {
                DestroyImmediate(obj.gameObject);
            }
        }
        if (GUILayout.Button("COUNT"))
        {
            UShape[] objs = GameObject.FindObjectsOfType<UShape>();
            Debug.Log("Count = " + objs.Length);
        }

        EditorGUILayout.LabelField("==================================================================================================================");
        if (GUILayout.Button("LOAD LEVEL DATA"))
        {
            foreach(TransformData td in levelData.transformDataList)
            {
                GameObject obj = PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>("Assets/_game/Prefabs/ushape-Instance/Ushape1.prefab")) as GameObject;
                obj.transform.position = td.position;
                obj.transform.rotation = Quaternion.Euler(td.eulerAngle);
                obj.transform.localScale = td.scale;
                obj.transform.SetParent(container);
            }
            Debug.Log("SAVE LEVEL DATA");
        }
        if (GUILayout.Button("SAVE LEVEL DATA"))
        {
            levelData.transformDataList.Clear();
            UShape[] ushapes = GameObject.FindObjectsOfType<UShape>();
            Debug.Log("ushapes.Length " + ushapes.Length);
            foreach (UShape u in ushapes)
            {
                levelData.transformDataList.Add(new TransformData(u.transform.position, u.transform.eulerAngles, u.transform.lossyScale));
            }
            EditorUtility.SetDirty(levelData);
            Debug.Log("SAVE LEVEL DATA to " + levelData.name);
        }


    }

    IEnumerator I_SpawnObjectsRandom()
    {
        if (isSpawing) yield break;
        Debug.Log("spawning objects in parent " + /*spawnPos.name +*/ "........");
        isSpawing = true;
        spawnedCount = 0;
        for (int i = 0; i < spawnQuantity; i++)
        {
            if (container != null) SpawnObjectRandom(spawnPositions[i % spawnPositions.Length]);
            else SpawnObjectRandom(spawnPos);
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            //yield return new WaitForSecondsRealtime(0.01f);
        }
        Debug.Log("finish spawn objects : " + spawnedCount.ToString() + "/" + spawnQuantity.ToString() + " in parent " /*+ spawnPos.name*/);
        isSpawing = false;
        yield return null;
    }

    private UShape SpawnObject(Transform parent)
    {
        GameObject obj = PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>("Assets/_game/Prefabs/UShape.prefab")) as GameObject;
        currentUshape = obj.GetComponent<UShape>();
        /*
                #region long bar scale
                float longBarScale = uShape.longBar.transform.localScale.x;
                float previousScale = uShape.longBar.transform.localScale.x;
                EditorGUI.BeginChangeCheck();
                longBarScale = UnityEngine.Random.Range(2f, 10f); //need to random
                uShape.longBar.transform.localScale = new Vector3(longBarScale, 1, 1);
                if (longBarScale != previousScale) // Kiểm tra nếu giá trị slider đã thay đổi
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
                shortBarScale = UnityEngine.Random.Range(2f, 5f);//need to random
                uShape.shortBar1.transform.localScale = new Vector3(1, shortBarScale, 1);
                uShape.shortBar2.transform.localScale = new Vector3(1, shortBarScale, 1);
                if (shortBarScale != previousScale) // Kiểm tra nếu giá trị slider đã thay đổi
                {
                    float offset = shortBarScale - previousScalee;
                    uShape.longBar.transform.localPosition += new Vector3(0, offset / 2, 0);
                }
                #endregion

                #region color
                Array matArray = Enum.GetValues(typeof(MatType));
                do
                {
                    uShape.matType = (MatType)matArray.GetValue(UnityEngine.Random.Range(0, matArray.Length));
                }
                while (uShape.matType == MatType.Black);
                MeshRenderer[] meshRenderers = uShape.GetComponentsInChildren<MeshRenderer>();
                foreach (MeshRenderer meshRenderer in meshRenderers)
                {
                    meshRenderer.material = MaterialCollection.Ins.GetMat(uShape.matType);
                }
                #endregion
        */
        #region position after spawn
        currentUshape.transform.SetParent(parent);
        currentUshape.transform.localRotation = Quaternion.identity;
        #endregion
        return currentUshape;
    }

    public bool SpawnObjectRandom(Transform spawnPos)
    {
        UShape uShape;
        uShape = SpawnObject(spawnPos);
        UShape_Model uShape_Model = uShape.SpawnModel(firstSizeIndex, lastSizeIndex);
        uShape_Model.OnInit();
        uShape.OnInit();

        int loopPosNearPrevious = 0;
        int loopPosTimes = 0;

    RandomPos:;

        loopPosTimes++;
        Vector3 randomPos = Vector3.zero;

        /*if(previousUshapePos.sqrMagnitude > 0.01f) { // nếu spawn thành công ushape ngay trước đó
            randomPos = previousUshapePos + UnityEngine.Random.insideUnitSphere * 3; // random vị trí cách ushape trước đó một đoạn 3f
            while (loopPosNearPrevious < 50 && Vector3.Distance(randomPos, spawnPos.position) > spawnRadius)
            {
                randomPos = previousUshapePos + UnityEngine.Random.insideUnitSphere * 3; // random vị trí cách ushape trước đó một đoạn 3f
                loopPosNearPrevious++;
            }
        }

        if(loopPosNearPrevious == 50) // loop quá 50 lần mà vẫn không random được vị trí thỏa mãn gần ushape trước đó
        {
            randomPos = Vector3.zero;
        }

        if(randomPos.sqrMagnitude < 0.01f) // nếu spawn thất bại gần ushape trước đó thì spawn theo cube*/
        switch (containerType)
        {
            case ContainerType.Sphere:
                randomPos = spawnPos.position + UnityEngine.Random.insideUnitSphere * spawnRadius;
                break;
            case ContainerType.Cube:
                randomPos = spawnPos.position + spawnPos.right * Random.Range(-lengthX / 2, lengthX / 2) + spawnPos.up * Random.Range(-lengthY / 2, lengthY / 2) + spawnPos.forward * Random.Range(-lengthZ / 2, lengthZ / 2);
                break;
            default:
                randomPos = Vector3.zero;
                break;
        }

        //randomPos = new Vector3(HaiExtension.RoundToDot5(randomPos.x), HaiExtension.RoundToDot5(randomPos.y), HaiExtension.RoundToDot5(randomPos.z));

        uShape.TF.position = randomPos;
        uShape.TF.localScale = Vector3.one;

        int loopRotTimes = 0;
        int[] randomArr = new int[] {1,2,3,4,5,6,7,8,9,10,11,12};
        HaiExtension.Shuffle<int>(randomArr);
    
    RandomRot:;

        loopRotTimes++;

        //Vector3 randomRot = 90 * new Vector3(UnityEngine.Random.Range(-99, 99), UnityEngine.Random.Range(-99, 99), UnityEngine.Random.Range(-99, 99));
        //uShape.TF.localRotation = Quaternion.Euler(randomRot);
        uShape.RandomRot(loopRotTimes,randomArr);

        bool isCollide = false;
        bool isOppositeOtherUshapes = false;

        for (int i = 0; i < uShape.model.bars.Length; i++)
        {
            if (uShape.model.bars[i].IsCollideUshape())
            {
                isCollide = true;
                break;
            }
        }

        if (!isCollide)
        {
            isOppositeOtherUshapes = uShape.IsOppositeOtherUshapes();
        }

        if (loopPosTimes < 1000 && (isCollide || isOppositeOtherUshapes))
        {
            if(loopRotTimes < 12) goto RandomRot;
            else goto RandomPos;
        }

        if (loopPosTimes >= 1000 && (isCollide || isOppositeOtherUshapes))
        {
            previousUshapePos = Vector3.zero;
            DestroyImmediate(uShape.gameObject);
            return false;
        }

        previousUshapePos = uShape.TF.position;

        spawnedCount++;

        return true;
    }

    


}
#endif