using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateLight : MonoBehaviour
{
    public float rotSpeed;
    public DayAndNight dn;
    void Update()
    {
        rotSpeed = dn.timeSpeed * 0.3f;
        transform.Rotate(Vector3.right, (rotSpeed) * Time.deltaTime);
    }
}
