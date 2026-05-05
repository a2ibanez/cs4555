using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class EnemyMovement : MonoBehaviour
{
    public Transform player;
    public float chaseRadius = 8f;
    public float stuckCheckDistance = 0.15f;
    public float stuckRecoveryTime = 0.75f;
    public float dazedSpinSpeed = 720f;
    public ParticleSystem headbuttParticles;
    private NavMeshAgent agent;
    private Animator animator;
    private Rigidbody enemyRigidbody;
    private bool isTouchingPlayer;
    private bool isStunned;
    private Vector3 lastPosition;
    private float stuckTimer;
    private Coroutine stunCoroutine;
    private RigidbodyConstraints originalConstraints;
    private bool originalIsKinematic;

    public bool IsStunned
    {
        get { return isStunned; }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        enemyRigidbody = GetComponent<Rigidbody>();

        if (enemyRigidbody != null)
        {
            originalConstraints = enemyRigidbody.constraints;
            originalIsKinematic = enemyRigidbody.isKinematic;
        }

        lastPosition = transform.position;

    }

    // Update is called once per frame
    void Update()
    {
        if (isStunned)
        {
            if (animator != null)
            {
                animator.SetBool("isRunning", false);
            }

            return;
        }

        if (player != null)
        {
            Vector3 direction = player.position - transform.position;
            direction.y = 0f;

            bool playerInChaseRange = direction.magnitude <= chaseRadius;
            bool shouldChase = playerInChaseRange && !isTouchingPlayer;

            agent.isStopped = !shouldChase;

            if (shouldChase)
            {
                agent.SetDestination(player.position);
                UpdateStuckRecovery();

                if (direction != Vector3.zero)
                {
                    Quaternion lookRotation = Quaternion.LookRotation(direction);
                    transform.rotation = Quaternion.Slerp(
                        transform.rotation,
                        lookRotation,
                        10f * Time.deltaTime
                    );
                }
            }
            else
            {
                stuckTimer = 0f;
                agent.ResetPath();
            }
        }

    bool isMoving = !agent.isStopped
        && agent.velocity.magnitude > 0.1f
        && agent.remainingDistance > agent.stoppingDistance;

    if (animator != null)
    {
        animator.SetBool("isRunning", isMoving);
    }

    lastPosition = transform.position;

    }

    private void OnCollisionEnter(Collision collision){
        if(collision.gameObject.CompareTag("player")){
            print("hit");
            isTouchingPlayer = true;
            agent.isStopped = true;
            if (animator != null)
            {
                animator.SetBool("isRunning", false);
            }
        }
        
    }

    private void OnCollisionExit(Collision collision){
        if(collision.gameObject.CompareTag("player")){
            isTouchingPlayer = false;
            agent.isStopped = false;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chaseRadius);
    }

    private void UpdateStuckRecovery()
    {
        if (agent.pathPending || agent.remainingDistance <= agent.stoppingDistance + 0.1f)
        {
            stuckTimer = 0f;
            return;
        }

        float movedDistance = Vector3.Distance(transform.position, lastPosition);
        if (movedDistance <= stuckCheckDistance && agent.velocity.sqrMagnitude < 0.01f)
        {
            stuckTimer += Time.deltaTime;
            if (stuckTimer >= stuckRecoveryTime)
            {
                agent.ResetPath();
                agent.SetDestination(player.position);
                stuckTimer = 0f;
            }
        }
        else
        {
            stuckTimer = 0f;
        }
    }

    public void Stun(float duration)
    {
        if (stunCoroutine != null)
        {
            StopCoroutine(stunCoroutine);
        }

        stunCoroutine = StartCoroutine(StunRoutine(duration));
    }

    private IEnumerator StunRoutine(float duration)
    {
        isStunned = true;
        isTouchingPlayer = false;
        stuckTimer = 0f;

        if (enemyRigidbody != null)
        {
            enemyRigidbody.linearVelocity = Vector3.zero;
            enemyRigidbody.angularVelocity = Vector3.zero;
            enemyRigidbody.constraints =
                RigidbodyConstraints.FreezePositionX |
                RigidbodyConstraints.FreezePositionY |
                RigidbodyConstraints.FreezePositionZ |
                RigidbodyConstraints.FreezeRotationX |
                RigidbodyConstraints.FreezeRotationZ;
            enemyRigidbody.isKinematic = true;
        }

        if (agent != null)
        {
            agent.isStopped = true;
            agent.ResetPath();
        }

        if (animator != null)
        {
            animator.SetBool("isRunning", false);
        }

        if (headbuttParticles != null)
        {
            headbuttParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            headbuttParticles.Play();
        }

        float stunTimer = 0f;
        while (stunTimer < duration)
        {
            Vector3 eulerAngles = transform.eulerAngles;
            transform.rotation = Quaternion.Euler(0f, eulerAngles.y, 0f);
            transform.Rotate(Vector3.up, dazedSpinSpeed * Time.deltaTime, Space.World);

            stunTimer += Time.deltaTime;
            yield return null;
        }

        Vector3 finalEulerAngles = transform.eulerAngles;
        transform.rotation = Quaternion.Euler(0f, finalEulerAngles.y, 0f);

        if (enemyRigidbody != null)
        {
            enemyRigidbody.isKinematic = originalIsKinematic;
            enemyRigidbody.constraints = originalConstraints;
            enemyRigidbody.linearVelocity = Vector3.zero;
            enemyRigidbody.angularVelocity = Vector3.zero;
        }

        isStunned = false;
        stunCoroutine = null;
    }
}
