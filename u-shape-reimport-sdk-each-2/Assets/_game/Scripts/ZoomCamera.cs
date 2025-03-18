using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoomCamera : MonoBehaviour
{
    public float touchesPrePosDiff, touchesCurPosDiff, zoomModifier;
    public Vector2 firstTouchPrePos, secondTouchPrePos;
    public float zoomWheelSpeed;
    public float minOrthoSize;
    public float maxOrthoSize;
    public float zoomMobileSpeed;
    void Update()
    {
        // 2 fingers
        if (Input.touchCount == 2)
        {
            if (TutorialZoom.Ins.isShowing && !DataManager.Ins.playerData.isPassedTutorialZoom) {
                TutorialZoom.Ins.Hide();
                DataManager.Ins.playerData.isPassedTutorialZoom = true;
                AFSendEvent.SendEvent("af_tutorial_completion", new Dictionary<string, string>()
                {
                    {"af_success", "true"},
                    {"af_tutorial_id", "2" }
                });
            } 

            Touch firstTouch = Input.GetTouch(0);
            Touch secondTouch = Input.GetTouch(1);

            firstTouchPrePos = firstTouch.position - firstTouch.deltaPosition;
            secondTouchPrePos = secondTouch.position - secondTouch.deltaPosition;

            touchesPrePosDiff = (firstTouchPrePos - secondTouchPrePos).sqrMagnitude;
            touchesCurPosDiff = (firstTouch.position - secondTouch.position).sqrMagnitude;

            float deltaPos = (firstTouch.deltaPosition - secondTouch.deltaPosition).sqrMagnitude;
            deltaPos = Mathf.Clamp(deltaPos, 0, 400f);
            zoomModifier = deltaPos * zoomMobileSpeed * Time.deltaTime;

            if (touchesPrePosDiff > touchesCurPosDiff)
            {
                Camera.main.orthographicSize += zoomModifier;
            }
            else if (touchesPrePosDiff < touchesCurPosDiff)
            {
                if ((Camera.main.orthographicSize - zoomModifier) < minOrthoSize)
                {
                    Camera.main.orthographicSize = minOrthoSize;
                }
                else
                {
                    Camera.main.orthographicSize -= zoomModifier;
                }

            }
            Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize, minOrthoSize, maxOrthoSize);
        }

        // wheel scroll
        float newOrthographicSize = Camera.main.orthographicSize - Input.GetAxis("Mouse ScrollWheel") * zoomWheelSpeed * Time.deltaTime;
        Camera.main.orthographicSize = Mathf.Clamp(newOrthographicSize, minOrthoSize, maxOrthoSize);
        Debug.LogError(Mathf.Clamp(newOrthographicSize, minOrthoSize, maxOrthoSize));

    }
}
