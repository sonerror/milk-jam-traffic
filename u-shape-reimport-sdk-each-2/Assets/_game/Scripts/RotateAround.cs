using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateAround : MonoBehaviour
{
    public float rotateMouseSpeed;
    private Vector3 lastMousePosition;
    private bool isMouseDragging;

    private void Update()
    {
        if (DataManager.Ins.playerData.currentStageIndex == 0) return;

        if (Input.GetMouseButtonDown(0))
        {
            isMouseDragging = true;
            lastMousePosition = Input.mousePosition;
        }
        else if (Input.GetMouseButtonUp(0) || Input.touchCount == 2)
        {
            isMouseDragging = false;
        }

        if (isMouseDragging && Input.touchCount < 2/* && !UI_HoverTest.ins.isHoveringUI*/)
        {
            Vector3 currentMousePosition = Input.mousePosition;

            // Calculate the difference in mouse position to determine rotation
            float deltaX = currentMousePosition.x - lastMousePosition.x;
            float deltaY = currentMousePosition.y - lastMousePosition.y;

            // Calculate the rotation
            Quaternion rotationX = Quaternion.Euler(deltaY * rotateMouseSpeed * Time.deltaTime, 0f, 0f);
            Quaternion rotationY = Quaternion.Euler(0f, -deltaX * rotateMouseSpeed * Time.deltaTime, 0f);

            // Apply the rotation to the GameObject
            transform.rotation = rotationY * transform.rotation;
            transform.rotation = rotationX * transform.rotation;

            lastMousePosition = currentMousePosition;
        }

    }
}
