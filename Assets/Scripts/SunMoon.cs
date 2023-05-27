using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunMoon : MonoBehaviour
{
    public float rotSpeed;

    public GameObject Target;

    public float offsetX = 0.0f;
    public float offsetY = -50f;
    public float offsetZ = 0.0f;

    public float SKYSpeed = 10.0f;

    Vector3 TargetPos;

    public DayAndNight dn;

    private void Update()
    {
        rotSpeed = dn.timeSpeed * 0.3f;
        transform.Rotate(Vector3.right, (rotSpeed) * Time.deltaTime);
    }

    void FixedUpdate()
    {
        TargetPos = new Vector3(
            Target.transform.position.x + offsetX,
            Target.transform.position.y + offsetY,
            Target.transform.position.z + offsetZ
            );

        transform.position = Vector3.Lerp(transform.position, TargetPos, Time.deltaTime * SKYSpeed);
    }
}

