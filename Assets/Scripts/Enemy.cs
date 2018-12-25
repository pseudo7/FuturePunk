using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public float fireRate = 1f;
    public int enemyHealth = 10;
    public Transform gunTip;

    Transform playerTransform;
    Animator enemyAC;
    NavMeshAgent enemyNMA;
    Coroutine findingPlayerCoroutine;
    Animation enemyAnim;
    LineRenderer line;
    float countdown;

    void Awake()
    {
        enemyHealth = Constants.ENEMY_HEALTH;
        line = gunTip.GetComponent<LineRenderer>();
        line.enabled = false;
        enemyAnim = GetComponent<Animation>();
        enemyAC = GetComponent<Animator>();
        enemyNMA = GetComponent<NavMeshAgent>();
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        findingPlayerCoroutine = StartCoroutine(GotoPlayerCoroutine());
    }

    private void LateUpdate()
    {

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
        FireAtRate();
    }

    private void FireAtRate()
    {
        if (countdown > 1 / fireRate)
            StartCoroutine(Fire());
        else countdown += Time.deltaTime;
    }

    IEnumerator Fire()
    {
        countdown = 0;
        line.enabled = true;
        Handheld.Vibrate();
        var player = playerTransform.GetComponent<PlayerMovement>();
        if (player.playerHealth > 0)
            if (player.PlayerHit())
                yield break;
        line.SetPositions(new Vector3[] { gunTip.position, playerTransform.position + new Vector3(0, gunTip.position.y, 0) });
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        line.enabled = false;
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
