using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    Transform playerTransform;
    Animator enemyAC;
    NavMeshAgent enemyNMA;
    Coroutine findingPlayerCoroutine;
    Animation enemyAnim;

    void Awake()
    {
        enemyAnim = GetComponent<Animation>();
        enemyAC = GetComponent<Animator>();
        enemyNMA = GetComponent<NavMeshAgent>();
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        findingPlayerCoroutine = StartCoroutine(GotoPlayerCoroutine());
    }

    private void OnTriggerEnter(Collider other)
    {
        enemyNMA.isStopped = true;
        //if (findingPlayerCoroutine != null)
        //    StopCoroutine(findingPlayerCoroutine);
        //enemyNMA.SetDestination(transform.position);
        //enemyAC.SetBool("Shoot", true);
        enemyAnim.Play("Shoot");
    }

    //private void OnCollisionEnter(Collision collision)
    //{
    //    Debug.Log(collision.collider.name);
    //}


    private void OnTriggerStay(Collider other)
    {
        transform.LookAt(playerTransform);
    }

    private void OnTriggerExit(Collider other)
    {
        enemyNMA.isStopped = false;

        //findingPlayerCoroutine = StartCoroutine(GotoPlayerCoroutine());
        //enemyAC.SetBool("Shoot", false);
        enemyAnim.Play("RunForward");
    }

    //private void OnCollisionExit(Collision collision)
    //{
    //    findingPlayerCoroutine = StartCoroutine(GotoPlayerCoroutine());
    //}

    IEnumerator GotoPlayerCoroutine()
    {
        while (gameObject.activeInHierarchy)
        {
            enemyNMA.SetDestination(playerTransform.position);
            yield return new WaitForSeconds(.25f);
        }
    }
}
