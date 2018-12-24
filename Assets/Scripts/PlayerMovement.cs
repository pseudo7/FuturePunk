using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class PlayerMovement : MonoBehaviour
{
    public float movementSmoothness = 15f;
    public float checkEnemyRadius = 20f;

    public Animator playerAC;

    CapsuleCollider playerCollider;

    private void Awake()
    {
        playerAC = GetComponent<Animator>();
        playerCollider = GetComponent<CapsuleCollider>();
    }

    void Update()
    {
        var x = CrossPlatformInputManager.GetAxis("Horizontal");
        var y = CrossPlatformInputManager.GetAxis("Vertical");

        playerAC.SetFloat("S", x);
        playerAC.SetFloat("W", y);

        transform.position = Vector3.Lerp(transform.position, transform.position + new Vector3(x, 0, y), 1 / movementSmoothness);

        if (Physics.CheckSphere(transform.position, checkEnemyRadius, LayerMask.GetMask("Enemy")))
            transform.LookAt(ClosestPosition(Physics.OverlapSphere(transform.position, checkEnemyRadius, LayerMask.GetMask("Enemy"))));
    }

    Vector3 ClosestPosition(Collider[] colliders)
    {
        float closest = float.MaxValue;
        Vector3 closestPoint = Vector3.one;
        foreach (var item in colliders)
        {
            float temp;
            if ((temp = Vector3.Distance(transform.position, item.transform.position)) < closest)
            {
                closest = temp;
                closestPoint = item.transform.position;
            }
        }
        return closestPoint;
    }
}
