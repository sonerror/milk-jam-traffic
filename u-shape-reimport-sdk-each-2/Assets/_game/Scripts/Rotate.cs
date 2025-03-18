using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{
    public float speed;

    private void Update()
    {
        this.transform.Rotate(0, 0, -speed*Time.deltaTime);
    }
}
