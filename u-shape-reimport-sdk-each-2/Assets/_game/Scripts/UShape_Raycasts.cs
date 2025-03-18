using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UShape_Raycasts : MonoBehaviour
{
    public UShape uShape;
    public Transform[] origins;
    public List<UShape> hitUShapeList = new List<UShape>();
    public Vector3 hitPoint;
    public bool isRaycastingUshape;


    public void OnInit()
    {
        uShape = GetComponentInParent<UShape>();
        origins = GetComponentsInChildren<Transform>(); 
    }

    public Vector3 GetVectorRaycast()
    {
        Vector3 vectorRaycast = Vector3.zero;
        float minDistance = 1000f;
        isRaycastingUshape = false;
        foreach(Transform org in origins)
        {
            RaycastHit hit;
            LayerMask mask = LayerMask.GetMask("bar");
            if (Physics.Raycast(org.transform.position, uShape.GetDir(), out hit, 300f, mask)
                && hit.collider.gameObject.GetComponentInParent<UShape>().uShape_Raycasts != this)
            {
                if(hit.distance < minDistance)
                {
                    vectorRaycast = hit.point - org.position;
                    minDistance = hit.distance;
                    if(Vector3.Distance(uShape.topPoint.position + vectorRaycast, uShape.targetStuck.position) > 0.01f)
                    {
                        uShape.SetUpTargetStuck(uShape.topPoint.position + vectorRaycast);
                    }
                    isRaycastingUshape = true;
                }
            }
        }
        hitUShapeList.Clear();
        foreach (Transform org in origins)
        {
            RaycastHit hit; 
            LayerMask mask = LayerMask.GetMask("bar");
            if (Physics.Raycast(org.transform.position, uShape.GetDir(), out hit, 300f, mask))
            {
                if (Mathf.Abs(hit.distance - minDistance) < 0.1f)
                {
                    UShape u = hit.collider.gameObject.GetComponentInParent<UShape>();
                    if (!hitUShapeList.Contains(u))
                    {
                        hitUShapeList.Add(u);
                    }
                }
            }
        }
        return vectorRaycast;
    }

    /*private void OnDrawGizmos()
    {
        foreach (Transform org in origins)
        {
            Gizmos.DrawRay(org.transform.position, uShape.dir*10f);
        }
    }*/

    public bool IsAllHitUShapesFlying()
    {
        foreach(UShape u in hitUShapeList)
        {
            if (!u.isFlying)
            {
                return false;
            }
        }
        return true;
    }
}
