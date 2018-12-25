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
    public Transform gun, gunTip;

    Animator playerAC;
    CapsuleCollider playerCollider;
    LineRenderer line;
    float gunOffset;

    private void Awake()
    {
        playerAC = GetComponent<Animator>();
        playerCollider = GetComponent<CapsuleCollider>();
        line = gunTip.GetComponent<LineRenderer>();
        line.enabled = false;
    }

    private void Start()
    {
        gunOffset = Vector3.Distance(gun.position, gunTip.position);
    }

    void Update()
    {
        var x = CrossPlatformInputManager.GetAxis("Horizontal");
        var y = CrossPlatformInputManager.GetAxis("Vertical");

        playerAC.SetFloat("S", x);
        playerAC.SetFloat("W", y);

        transform.position = Vector3.Lerp(transform.position, transform.position + new Vector3(x, 0, y), 1 / movementSmoothness);

        var nearByEnemies = Physics.OverlapSphere(transform.position, checkEnemyRadius, LayerMask.GetMask("Enemy"), QueryTriggerInteraction.Collide);
        Vector3 localPos;
        var closestPoint = FindClosest(nearByEnemies, out localPos);
        if (nearByEnemies.Length > 0)
            StartCoroutine(Fire(closestPoint));
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(closestPoint - transform.position), .4f);
    }

    Vector3 FindClosest(Collider[] colliders, out Vector3 localPos)
    {
        float closest = float.MaxValue;
        Vector3 closestPoint = transform.forward;
        localPos = transform.localPosition;
        foreach (var item in colliders)
        {
            float temp;
            if ((temp = Vector3.Distance(transform.position, item.transform.position)) < closest)
            {
                closest = temp;
                closestPoint = item.transform.position;
                localPos = item.transform.localPosition;
            }
        }
        return closestPoint;
    }

    IEnumerator Fire(Vector3 enemyPos)
    {
        line.enabled = true;
        line.SetPositions(new Vector3[] { gun.position + gun.forward * gunOffset + Vector3.up * 2, enemyPos + Vector3.up * 2 });
        yield return new WaitForEndOfFrame();
        line.enabled = false;
    }
}
