using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T m_Ins;

    public static T Ins
    {
        get
        {
            if (m_Ins == null)
            {
                m_Ins = FindObjectOfType<T>();
            }
            return m_Ins;
        }
    }

}
