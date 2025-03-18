using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RectUnit : MonoBehaviour
{
    RectTransform m_RTF;
    public RectTransform RTF
    {
        get
        {
            if (m_RTF == null)
            {
                m_RTF = transform as RectTransform;
            }
            return m_RTF;
        }
    }
}
