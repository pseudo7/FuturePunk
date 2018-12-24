using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class PlayerFollow : MonoBehaviour
{
    public Transform playerTransform;
    [Range(0f, 1f)]
    public float dampning = .6f;
    public float cameraSpeed = 5f;

    float horizontalDeadZone = 5f;
    float verticalDeadZone = 5f;

    Vector3 offset;
    Vector3 lastTargetPosition;

    private void Start()
    {
        offset = transform.position;
        lastTargetPosition = playerTransform.position;
    }

    void LateUpdate()
    {
        //var x = Mathf.Abs(offset.x - playerTransform.position.x) > horizontalDeadZone ? playerTransform.position.x : 0;
        //var z = Mathf.Abs(offset.z - playerTransform.position.z) > verticalDeadZone ? playerTransform.position.z : 0;
        //if (x > 0 || z > 0)
        transform.position = Vector3.LerpUnclamped(transform.position, offset + playerTransform.position, dampning);
    }
}
