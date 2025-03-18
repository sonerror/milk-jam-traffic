using UnityEngine;

// Anti-Notch
public class UISafeArea : RectUnit
{
    struct SafeArea
    {
        public bool isCalculated;
        public Vector2 anchorMin;
        public Vector2 anchorMax;

        public void Calculate()
        {
            Rect safeArea = Screen.safeArea;
            float width = Screen.width;
            float height = Screen.height;
            anchorMin = safeArea.position;
            anchorMax = anchorMin + safeArea.size;
            anchorMin.x /= width;
            anchorMin.y /= height;
            anchorMax.x /= width;
            anchorMax.y /= height;
            isCalculated = true;
        }
    }

    static SafeArea safeArea;
    void Awake()
    {
        if (!safeArea.isCalculated)
        {
            safeArea.Calculate();
        }
        RTF.anchorMin = safeArea.anchorMin;
        RTF.anchorMax = safeArea.anchorMax;
    }
}
