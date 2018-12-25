using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class PlayerMovement : MonoBehaviour
{
    public float movementSmoothness = 15f;
    public float checkEnemyRadius = 20f;
    public float fireRate;

    Animator playerAC;
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

        //if (Physics.CheckSphere(transform.position, checkEnemyRadius, LayerMask.GetMask("Enemy"), QueryTriggerInteraction.Collide))
        //transform.LookAt(ClosestPosition(Physics.OverlapSphere(transform.position, checkEnemyRadius, LayerMask.GetMask("Enemy"))));
        var nearByEnemies = Physics.OverlapSphere(transform.position, checkEnemyRadius, LayerMask.GetMask("Enemy"), QueryTriggerInteraction.Collide);
        var closestPoint = FindClosest(nearByEnemies);
        if (nearByEnemies.Length > 0)
            Fire(closestPoint);
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(closestPoint - transform.position), .4f);
    }

    Vector3 FindClosest(Collider[] colliders)
    {
        float closest = float.MaxValue;
        Vector3 closestPoint = transform.forward;
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

    private void Fire(Vector3 enemyPos)
    {

    }
}
