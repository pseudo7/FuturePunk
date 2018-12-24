using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    Transform playerTransform;

    NavMeshAgent enemyNMA;

    void Start()
    {
        enemyNMA = GetComponent<NavMeshAgent>();
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        StartCoroutine(GotoPlayerCoroutine());
    }

    IEnumerator GotoPlayerCoroutine()
    {
        while (gameObject.activeInHierarchy)
        {
            enemyNMA.SetDestination(playerTransform.position);
            yield return new WaitForSeconds(.5f);
        }
    }
}
