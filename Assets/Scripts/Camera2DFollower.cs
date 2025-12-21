using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera2DFollower : MonoBehaviour
{
 
    public Transform target;      // ตัวผู้เล่น
    public float smoothSpeed = 5f;
    public Vector3 offset;

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPosition = target.position + offset;
        Vector3 smoothPosition = Vector3.Lerp(
            transform.position,
            new Vector3(desiredPosition.x, desiredPosition.y, transform.position.z),
            smoothSpeed * Time.deltaTime
        );

        transform.position = smoothPosition;
    }
}


