using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement : MonoBehaviour
{
    public Transform player;
    private NavMeshAgent agent;
    private Animator animator;
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
        agent.SetDestination(player.position);

        
        Vector3 direction = player.position - transform.position;
        direction.y = 0f;

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

    bool isMoving = !agent.isStopped
        && agent.velocity.magnitude > 0.1f
        && agent.remainingDistance > agent.stoppingDistance;

    animator.SetBool("isRunning", isMoving);

    }

    private void OnCollisionEnter(Collision collision){
        if(collision.gameObject.CompareTag("player")){
            print("hit");
            agent.isStopped = true;
            animator.SetBool("isRunning", false);
        }
        
    }

    private void OnCollisionExit(Collision collision){
        if(collision.gameObject.CompareTag("player")){
            agent.isStopped = false;
        }
    }
}
