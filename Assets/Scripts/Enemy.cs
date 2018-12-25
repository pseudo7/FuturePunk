using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public int enemyHealth = 10;

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
        enemyAnim.Play("Shoot");
    }

    public bool EnemyHit()
    {
        if (--enemyHealth <= 0)
        {
            foreach (var col in GetComponents<Collider>())
                col.enabled = false;
            enemyNMA.isStopped = true;
            enemyAnim.Stop();
            enemyAnim.Play(Random.Range(0, 2) % 2 == 0 ? "Death1" : "Death2", PlayMode.StopAll);
            Destroy(gameObject, 1f);
            return true;
        }
        return false;
    }

    private void OnTriggerStay(Collider other)
    {
        transform.LookAt(playerTransform);
    }

    private void OnTriggerExit(Collider other)
    {
        enemyNMA.isStopped = false;
        enemyAnim.Play("RunForward");
    }

    IEnumerator GotoPlayerCoroutine()
    {
        while (gameObject.activeInHierarchy)
        {
            enemyNMA.SetDestination(playerTransform.position);
            yield return new WaitForSeconds(.25f);
        }
    }
}
