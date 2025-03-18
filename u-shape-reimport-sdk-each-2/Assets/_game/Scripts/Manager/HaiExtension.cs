using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class HaiExtension 
{
    public static Transform GetChildWithTag(this Transform parent, string tagName)
    {
        foreach (Transform tf in parent.transform)
        {
            if (tf.tag == tagName) { return tf; }
        }
        return null;
    }

    public static Transform[] GetChilds(this Transform parent)
    {
        List<Transform> list = new List<Transform>();

        foreach (Transform child in parent)
        {
            list.Add(child);
        }

        return list.ToArray();
    }

    public static Transform[] GetChildsWithTag(this Transform parent, string tagName)
    {
        List<Transform> list = new List<Transform>();

        foreach (Transform child in parent)
        {
            if (child.tag == tagName)
            {
                list.Add(child);
            }

            // Gọi đệ quy để lấy các grandchild
            Transform[] childResults = GetChildsWithTag(child, tagName);
            list.AddRange(childResults);
        }

        return list.ToArray();
    }

    public static bool IsCollideUshape(this Transform bar)
    {
        float scaleOffset = 1.05f;
        return Physics.CheckBox(
            bar.transform.position, // Vị trí của prefab U cần kiểm tra
            bar.transform.lossyScale / 2 * scaleOffset, // Kích thước box, chia 2 vì CheckBox sử dụng half-extents
            bar.transform.rotation,
            LayerMask.GetMask("bar"), // Layer của prefab U cần kiểm tra
            QueryTriggerInteraction.Collide // Bỏ qua các trigger nếu có
        );
    }

    public static Vector3 GetLocalDir(this Transform tf, Vector3 dir)
    {
        if(Vector3.Angle(tf.up, dir) < 0.1f)
        {
            return Vector3.up;
        }
        if (Vector3.Angle(-tf.up, dir) < 0.1f)
        {
            return -Vector3.up;
        }
        if (Vector3.Angle(tf.right, dir) < 0.1f)
        {
            return Vector3.right;
        }
        if(Vector3.Angle(-tf.right, dir) < 0.1f)
        {
            return -Vector3.right;
        }
        if (Vector3.Angle(tf.forward, dir) < 0.1f)
        {
            return Vector3.forward;
        }
        if (Vector3.Angle(-tf.forward, dir) < 0.1f)
        {
            return -Vector3.forward;
        }
        return Vector3.zero;
    }

    public static bool Contains<T>(this T[] arr, T tt)
    {
        foreach(T t in arr)
        {
            if(tt.Equals(t)) return true;
        }
        return false;
    }

    public static void RotateAroundAxis(this Transform tf, float angle, Vector3 axis)
    {
        tf.Rotate(axis, angle, Space.Self);
    }

    public static void Shuffle<T>(T[] array)
    {
        System.Random random = new System.Random();

        // Iterate through the array in reverse order
        for (int i = array.Length - 1; i > 0; i--)
        {
            // Generate a random index within the remaining unshuffled portion of the array
            int randomIndex = random.Next(0, i + 1);

            // Swap the elements at the current index and the randomly chosen index
            T temp = array[i];
            array[i] = array[randomIndex];
            array[randomIndex] = temp;
        }
    }

    public static void Shuffle<T>(List<T> list)
    {
        System.Random random = new System.Random();

        // Iterate through the array in reverse order
        for (int i = list.Count - 1; i > 0; i--)
        {
            // Generate a random index within the remaining unshuffled portion of the array
            int randomIndex = random.Next(0, i + 1);

            // Swap the elements at the current index and the randomly chosen index
            T temp = list[i];
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }

    public static void RandomRot(this UShape u, int loopTime, int[] randomArr)
    {
        switch (randomArr[loopTime - 1])
        {
            case 1:
                u.TF.localRotation = Quaternion.Euler(new Vector3(0, 0, 0));
                return;
            case 2:
                u.TF.localRotation = Quaternion.Euler(new Vector3(0, 0, 0));
                u.TF.RotateAroundAxis(90f, Vector3.up);
                return;
            case 3:
                u.TF.localRotation = Quaternion.Euler(new Vector3(180, 0, 0));
                return;
            case 4:
                u.TF.localRotation = Quaternion.Euler(new Vector3(180, 0, 0));
                u.TF.RotateAroundAxis(90f, Vector3.up);
                return;
            case 5:
                u.TF.localRotation = Quaternion.Euler(new Vector3(90, 0, 0));
                return;
            case 6:
                u.TF.localRotation = Quaternion.Euler(new Vector3(90, 0, 0));
                u.TF.RotateAroundAxis(90f, Vector3.up);
                return;
            case 7:
                u.TF.localRotation = Quaternion.Euler(new Vector3(-90, 0, 0));
                return;
            case 8:
                u.TF.localRotation = Quaternion.Euler(new Vector3(-90, 0, 0));
                u.TF.RotateAroundAxis(90f, Vector3.up);
                return;
            case 9:
                u.TF.localRotation = Quaternion.Euler(new Vector3(0, 0, 90));
                return;
            case 10:
                u.TF.localRotation = Quaternion.Euler(new Vector3(0, 0, 90));
                u.TF.RotateAroundAxis(90f, Vector3.up);
                return;
            case 11:
                u.TF.localRotation = Quaternion.Euler(new Vector3(0, 0, -90));
                return;
            case 12:
                u.TF.localRotation = Quaternion.Euler(new Vector3(0, 0, -90));
                u.TF.RotateAroundAxis(90f, Vector3.up);
                return;

            default: return;
        }
    }

    public static void EffectBounce(this Transform tf, float targetScale)
    {
        tf
            .DOScale(tf.localScale * targetScale, 0.2f)
            .SetEase(Ease.InSine)
            .OnComplete(() =>
            {
                tf
                    .DOScale(tf.localScale / targetScale, 0.2f)
                    .SetEase(Ease.OutSine);
            });
    }

    public static void SetParentAndReset(this Transform tf, Transform parent)
    {
        tf.SetParent(parent);
        tf.localPosition = Vector3.zero;
        tf.localRotation = Quaternion.identity;
        tf.localScale = Vector3.one;
    }

}
