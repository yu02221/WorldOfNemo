using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follow : MonoBehaviour
{
    public GameObject FollowTarget;

    public float offsetX = 0f;
    public float offsetY = 0f;
    public float offsetZ = 0f;

    public float FollowSpeed = 10.0f;

    Vector3 TargetPos;

    void FixedUpdate()
    {
        TargetPos = new Vector3(
            FollowTarget.transform.position.x + offsetX,
            FollowTarget.transform.position.y + offsetY,
            FollowTarget.transform.position.z + offsetZ
            );

        transform.position = Vector3.Lerp(transform.position, TargetPos, Time.deltaTime * FollowSpeed);
    }

}
