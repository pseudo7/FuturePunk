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
    public Transform gunTip;

    Animator playerAC;
    CapsuleCollider playerCollider;
    LineRenderer line;
    float countdown;

    private void Awake()
    {
        playerAC = GetComponent<Animator>();
        playerCollider = GetComponent<CapsuleCollider>();
        line = gunTip.GetComponent<LineRenderer>();
        line.enabled = false;
    }

    void Update()
    {
        var x = CrossPlatformInputManager.GetAxis("Horizontal");
        var y = CrossPlatformInputManager.GetAxis("Vertical");

        playerAC.SetFloat("S", x);
        playerAC.SetFloat("W", y);

        transform.position = Vector3.Lerp(transform.position, transform.position + new Vector3(x, 0, y), 1 / movementSmoothness);

        var nearByEnemies = Physics.OverlapSphere(transform.position, checkEnemyRadius, LayerMask.GetMask("Enemy"), QueryTriggerInteraction.Collide);
        Transform closestTransform;
        var closestPoint = FindClosest(nearByEnemies, out closestTransform);
        if (nearByEnemies.Length > 0)
            FireAtRate(closestPoint);
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(closestPoint - transform.position), .4f);
    }

    private void FireAtRate(Vector3 enemyPos)
    {
        if (countdown > 1 / fireRate)
        {
            countdown = 0;

            StartCoroutine(Fire(enemyPos));
        }
        else countdown += Time.deltaTime;
    }

    Vector3 FindClosest(Collider[] colliders, out Transform closestTransform)
    {
        float closest = float.MaxValue;
        Vector3 closestPoint = transform.forward;
        closestTransform = transform;
        foreach (var item in colliders)
        {
            float temp;
            if ((temp = Vector3.Distance(transform.position, item.transform.position)) < closest)
            {
                closest = temp;
                closestPoint = item.transform.position;
                closestTransform = item.transform;
            }
        }
        return closestPoint;
    }

    IEnumerator Fire(Vector3 enemyPos)
    {
        line.enabled = true;
        line.SetPositions(new Vector3[] { gunTip.position, enemyPos + new Vector3(0, gunTip.position.y, 0) /*+ Vector3.up * gunTip.position.y*/ });
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        line.enabled = false;
    }
}
