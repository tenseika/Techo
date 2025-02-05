using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class EnemyManager : MonoBehaviour
{
    [Header("Patrol Settings")]
    public float patrolRadius = 10f;
    public float patrolInterval = 3f;

    [Header("Detection Settings")]
    public float detectionRadius = 7f;
    public LayerMask playerLayer;

    [Header("Attack Settings")]
    public float attackRadius = 1.5f;
    public float attackCooldown = 2f;
    public int damage = 10;
    private float lastAttackTime;

    private NavMeshAgent navAgent;
    private Transform player;
    private float patrolTimer;
    private bool isChasing = false;
    private bool isInvestigating = false;
    private bool isLookingAround = false;
    private Vector3 lastNoisePosition;

    [Header("Chase Settings")]
    public float chaseSpeed = 5f;
    public float patrolSpeed = 2f;

    private Vector3 lastPosition;
    private float stuckTimer = 0f;
    private float maxStuckTime = 2.5f; // Jika diam selama 2.5 detik, cari rute baru

    private void Awake()
    {
        navAgent = GetComponent<NavMeshAgent>();
        navAgent.speed = patrolSpeed;
        lastPosition = transform.position;
    }

    private void Update()
    {
        DetectPlayer();

        if (isChasing)
        {
            ChasePlayer();
        }
        else if (isInvestigating)
        {
            InvestigateNoise();
        }
        else if (!isLookingAround) // Pastikan tidak mencari saat mengintai
        {
            Patrol();
        }

        if (player != null && Vector3.Distance(transform.position, player.position) <= attackRadius)
        {
            AttackPlayer();
        }

        CheckIfStuck(); // Cek apakah musuh stuck
    }

    private void Patrol()
    {
        patrolTimer += Time.deltaTime;

        if (patrolTimer >= patrolInterval && !isChasing && !isInvestigating)
        {
            MoveToRandomPoint();
            patrolTimer = 0f;
        }
    }

    private void MoveToRandomPoint()
    {
        for (int i = 0; i < 5; i++) // Coba cari titik hingga 5 kali
        {
            Vector3 randomDirection = Random.insideUnitSphere * patrolRadius;
            randomDirection += transform.position;
            randomDirection.y = transform.position.y; // Tetap di level yang sama

            if (NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, patrolRadius, NavMesh.AllAreas))
            {
                if (navAgent.SetDestination(hit.position))
                {
                    return; // Berhasil menemukan jalur
                }
            }
        }

        Debug.LogWarning("Enemy gagal menemukan titik patrol baru!");
    }

    private void DetectPlayer()
    {
        Collider[] players = Physics.OverlapSphere(transform.position, detectionRadius, playerLayer);

        if (players.Length > 0)
        {
            player = players[0].transform;
            isChasing = true;
            isInvestigating = false;
            isLookingAround = false;
            navAgent.speed = chaseSpeed;
        }
        else if (isChasing && player != null)
        {
            if (Vector3.Distance(transform.position, player.position) > detectionRadius)
            {
                isChasing = false;
                player = null;
                navAgent.speed = patrolSpeed;
            }
        }
    }

    private void ChasePlayer()
    {
        if (player != null && navAgent.enabled)
        {
            navAgent.SetDestination(player.position);
        }
    }

    private void AttackPlayer()
    {
        if (Time.time - lastAttackTime >= attackCooldown)
        {
            lastAttackTime = Time.time;

            if (player != null)
            {
                PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(damage);
                    Debug.Log($"Enemy attacked player for {damage} damage.");
                }
            }
        }
    }

    public void OnHearNoise(Vector3 noisePosition)
    {
        if (!isChasing)
        {
            lastNoisePosition = noisePosition;
            isInvestigating = true;
            isLookingAround = false;
            navAgent.SetDestination(lastNoisePosition);
            navAgent.speed = patrolSpeed;
        }
    }

    private void InvestigateNoise()
    {
        if (Vector3.Distance(transform.position, lastNoisePosition) <= 1f)
        {
            StartCoroutine(LookAround());
        }
    }

    private IEnumerator LookAround()
    {
        isInvestigating = false;
        isLookingAround = true;
        navAgent.isStopped = true;
        Debug.Log("Enemy sedang melihat sekitar...");
        yield return new WaitForSeconds(5f); // Diam selama 5 detik
        navAgent.isStopped = false;
        isLookingAround = false;
        Patrol();
    }

    private void CheckIfStuck()
    {
        if (Vector3.Distance(transform.position, lastPosition) < 0.1f) // Jika enemy tidak bergerak
        {
            stuckTimer += Time.deltaTime;

            if (stuckTimer >= maxStuckTime) // Jika stuck terlalu lama
            {
                Debug.Log("Enemy stuck, mencari jalur baru...");
                MoveToRandomPoint(); // Cari jalur baru
                stuckTimer = 0f;
            }
        }
        else
        {
            stuckTimer = 0f; // Reset timer jika enemy bergerak
        }

        lastPosition = transform.position;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, patrolRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackRadius);
    }
}
