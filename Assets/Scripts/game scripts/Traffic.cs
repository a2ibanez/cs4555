using UnityEngine;

[DisallowMultipleComponent]
public class Traffic : MonoBehaviour
{
    public Transform[] waypoints;
    public float speed = 5f;
    public float respawnDelay = 4f;
    public bool hideWhileResetting = true;

    private Vector3 startPosition;
    private Quaternion startRotation;
    private Renderer[] cachedRenderers;
    private Collider[] cachedColliders;
    private int currentWaypointIndex;
    private float respawnTimer;
    private bool waitingToRespawn;

    void Awake()
    {
        if (waypoints == null || waypoints.Length == 0)
        {
            enabled = false;
            return;
        }

        startPosition = transform.position;
        startRotation = transform.rotation;
        cachedRenderers = GetComponentsInChildren<Renderer>(true);
        cachedColliders = GetComponentsInChildren<Collider>(true);
    }

    void Start()
    {
        currentWaypointIndex = 0;
    }

    void Update()
    {
        if (waitingToRespawn)
        {
            respawnTimer -= Time.deltaTime;
            if (respawnTimer <= 0f)
            {
                ResetVehicle();
            }

            return;
        }

        Vector3 targetPosition = waypoints[currentWaypointIndex].position;

        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPosition,
            speed * Time.deltaTime
        );

        Vector3 direction = targetPosition - transform.position;
        direction.y = 0f;
        if (direction.sqrMagnitude > 0.001f)
        {
            transform.rotation = Quaternion.LookRotation(direction.normalized, Vector3.up);
        }

        if (Vector3.Distance(transform.position, targetPosition) > 0.05f)
        {
            return;
        }

        currentWaypointIndex++;
        if (currentWaypointIndex >= waypoints.Length)
        {
            BeginRespawn();
        }
    }

    void BeginRespawn()
    {
        waitingToRespawn = true;
        respawnTimer = respawnDelay;

        if (hideWhileResetting)
        {
            SetVisible(false);
        }
    }

    void ResetVehicle()
    {
        transform.SetPositionAndRotation(startPosition, startRotation);
        currentWaypointIndex = 0;
        waitingToRespawn = false;

        if (hideWhileResetting)
        {
            SetVisible(true);
        }
    }

    void SetVisible(bool visible)
    {
        foreach (Renderer rendererComponent in cachedRenderers)
        {
            rendererComponent.enabled = visible;
        }

        foreach (Collider colliderComponent in cachedColliders)
        {
            colliderComponent.enabled = visible;
        }
    }
}
