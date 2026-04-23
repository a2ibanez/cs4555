using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement : MonoBehaviour
{
    public Transform player;
    public float chaseRadius = 8f;
    private NavMeshAgent agent;
    private Animator animator;
    private bool isTouchingPlayer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        

    }

    // Update is called once per frame
    void Update()
    {
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
                agent.ResetPath();
            }
        }

    bool isMoving = !agent.isStopped
        && agent.velocity.magnitude > 0.1f
        && agent.remainingDistance > agent.stoppingDistance;

    animator.SetBool("isRunning", isMoving);

    }

    private void OnCollisionEnter(Collision collision){
        if(collision.gameObject.CompareTag("player")){
            print("hit");
            isTouchingPlayer = true;
            agent.isStopped = true;
            animator.SetBool("isRunning", false);
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
}
