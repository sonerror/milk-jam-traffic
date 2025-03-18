using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ScrambleManager : Singleton<ScrambleManager>
{
    public List<UShape> ushapeList;
    public List<Vector3> ushapePositionList;


    public void ScrambleSomeRotations()
    {
        ushapeList.Clear();
        foreach (UShape u in LevelManager.Ins.currentLevel.uShapes)
        {
            if (u != null && u.gameObject.activeInHierarchy && !u.isFlying)
            {
                ushapeList.Add(u);
            }
        }
        for (int i = 0; i < ushapeList.Count; i++)
        {
            UShape u = ushapeList[i];
            int loopRotTimes = 0;
            int[] randomArr = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };
            HaiExtension.Shuffle<int>(randomArr);
            Vector3 initialRot = u.TF.eulerAngles;
        RandomRot:;
            loopRotTimes++;
            u.RandomRot(loopRotTimes, randomArr);
            bool isCollide = false;
            bool isOppositeOtherUshapes = false;
            foreach (var bar in u.model.bars)
            {
                if (bar.IsCollideUshape())
                {
                    isCollide = true;
                    break;
                }
            }

            if (!isCollide)
            {
                isOppositeOtherUshapes = u.IsOppositeOtherUshapes();
            }

            if (isCollide || isOppositeOtherUshapes)
            {
                if (loopRotTimes < 12) goto RandomRot;
                else u.TF.rotation = Quaternion.Euler(initialRot);
            }

        }
    }

    public void Scramble()
    {
        ushapeList = new List<UShape>();
        ushapePositionList = new List<Vector3>();
        foreach (UShape u in LevelManager.Ins.currentLevel.uShapes)
        {
            if (u != null  && !u.isFlying)
            {
                u.gameObject.SetActive(false);
                ushapeList.Add(u);
                ushapePositionList.Add(u.TF.position);
            }
        }
        HaiExtension.Shuffle<Vector3>(ushapePositionList);

        int t = 15;
        while (t-- > 0)
        {
            for (int i = 0; i < ushapeList.Count; i++)
            {
                UShape u = ushapeList[i];
                if (u != null && u.gameObject.activeInHierarchy) continue;
                u.gameObject.SetActive(true);
                u.TF.position = ushapePositionList[i];
                int loopRotTimes = 0;
                int[] randomArr = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };
                HaiExtension.Shuffle<int>(randomArr);
            RandomRot:;
                loopRotTimes++;
                u.RandomRot(loopRotTimes, randomArr);
                bool isCollide = false;
                bool isOppositeOtherUshapes = false;
                foreach (var bar in u.model.bars)
                {
                    if (bar.IsCollideUshape())
                    {
                        isCollide = true;
                        break;
                    }
                }

                if (!isCollide)
                {
                    isOppositeOtherUshapes = u.IsOppositeOtherUshapes();
                }

                if (isCollide || isOppositeOtherUshapes)
                {
                    if (loopRotTimes < 12) goto RandomRot;
                    else u.gameObject.SetActive(false);
                }

            }
        }
      
    }

}
