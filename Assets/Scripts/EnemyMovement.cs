using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement : MonoBehaviour
{
    public Transform player;
    private NavMeshAgent agent;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        if (player != null){
            agent.SetDestination(player.position);
        }
    }

    private void OnCollisionEnter(Collision collision){
        if(collision.gameObject.CompareTag("player")){
            print("hit");
            agent.isStopped = true;
        }
        
    }

    private void OnCollisionExit(Collision collision){
        if(collision.gameObject.CompareTag("player")){
            agent.isStopped = false;
        }
    }
}
