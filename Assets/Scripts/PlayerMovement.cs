using System;
using System.Collections;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class PlayerMovement : MonoBehaviour
{
    [HideInInspector] public int playerHealth;
    public float movementSmoothness = 15f;
    public float checkEnemyRadius = 20f;
    public float fireRate;
    public Transform gunTip;

    Animator playerAC;
    CapsuleCollider playerCollider;
    LineRenderer line;
    float countdown;
    float origXScale;
    private void Awake()
    {
        playerHealth = Constants.PLAYER_HEALTH;
        playerAC = GetComponent<Animator>();
        playerCollider = GetComponent<CapsuleCollider>();
        line = gunTip.GetComponent<LineRenderer>();
        line.enabled = false;
        origXScale = transform.GetChild(0).localScale.x;
    }

    void Update()
    {
        if (Utility.isGameOver)
            return;

        var x = CrossPlatformInputManager.GetAxis("Horizontal");
        var y = CrossPlatformInputManager.GetAxis("Vertical");

        playerAC.SetFloat("S", x);
        playerAC.SetFloat("W", y);

        transform.position = Vector3.Lerp(transform.position, transform.position + new Vector3(x, 0, y), 1 / movementSmoothness);

        var nearByEnemies = Physics.OverlapSphere(transform.position, checkEnemyRadius, LayerMask.GetMask("Enemy"), QueryTriggerInteraction.Collide);
        var closestPoint = FindClosest(nearByEnemies);
        if (nearByEnemies.Length > 0)
            FireAtRate(closestPoint);
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(closestPoint.position - transform.position), .4f);
    }

    private void LateUpdate()
    {
        transform.GetChild(0).LookAt(Camera.main.transform);
    }

    private void FireAtRate(Transform enemyTransform)
    {
        if (countdown > 1 / fireRate)
        {
            countdown = 0;

            StartCoroutine(Fire(enemyTransform));
        }
        else countdown += Time.deltaTime;
    }

    public bool PlayerHit()
    {
        UpdateHealthBar(--playerHealth);

        if (playerHealth <= 0)
        {
            Utility.isGameOver = true;
            foreach (var col in GetComponents<Collider>())
                col.enabled = false;

            Handheld.Vibrate();
            Handheld.Vibrate();
            Handheld.Vibrate();
            playerAC.SetTrigger("Death");
            GameManager.Instance.RestartLevel(1f);
            return true;
        }
        return false;
    }

    void UpdateHealthBar(int health)
    {
        transform.GetChild(0).localScale = new Vector3(origXScale * health / (float)Constants.PLAYER_HEALTH, .2f, 1f);
    }

    Transform FindClosest(Collider[] colliders)
    {
        float closest = float.MaxValue;
        Transform closestPoint = transform;
        foreach (var item in colliders)
        {
            float temp;
            if ((temp = Vector3.Distance(transform.position, item.transform.position)) < closest)
            {
                closest = temp;
                closestPoint = item.transform;
            }
        }
        return closestPoint;
    }

    IEnumerator Fire(Transform enemyTransform)
    {
        var enemy = enemyTransform.GetComponent<Enemy>();
        if (enemy.enemyHealth > 0)
            if (enemy.EnemyHit())
                yield break;
        line.enabled = true;
        line.SetPositions(new Vector3[] { gunTip.position, enemyTransform.position + new Vector3(0, gunTip.position.y, 0) + enemyTransform.forward });
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        line.enabled = false;
    }
}
