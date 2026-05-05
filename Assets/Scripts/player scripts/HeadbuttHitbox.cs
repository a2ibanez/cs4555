using System.Collections.Generic;
using UnityEngine;

public class HeadbuttHitbox : MonoBehaviour
{
    public float stunDuration = 2f;

    private Collider hitboxCollider;
    private BoxCollider boxCollider;
    private readonly HashSet<EnemyMovement> hitEnemies = new HashSet<EnemyMovement>();

    private void Awake()
    {
        hitboxCollider = GetComponent<Collider>();
        boxCollider = GetComponent<BoxCollider>();

        if (hitboxCollider != null)
        {
            hitboxCollider.isTrigger = true;
        }

        DisableHitbox();
    }

    public void EnableHitbox()
    {
        hitEnemies.Clear();

        if (hitboxCollider != null)
        {
            hitboxCollider.enabled = true;
        }

        CheckCurrentOverlaps();
    }

    public void DisableHitbox()
    {
        if (hitboxCollider != null)
        {
            hitboxCollider.enabled = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        TryStun(other);
    }

    private void OnTriggerStay(Collider other)
    {
        TryStun(other);
    }

    private void TryStun(Collider other)
    {
        EnemyMovement enemy = other.GetComponentInParent<EnemyMovement>();
        if (enemy == null || hitEnemies.Contains(enemy))
        {
            return;
        }

        hitEnemies.Add(enemy);
        enemy.Stun(stunDuration);
    }

    private void CheckCurrentOverlaps()
    {
        if (boxCollider == null)
        {
            return;
        }

        Vector3 worldCenter = boxCollider.transform.TransformPoint(boxCollider.center);
        Vector3 worldHalfExtents = Vector3.Scale(boxCollider.size, boxCollider.transform.lossyScale) * 0.5f;
        Collider[] overlaps = Physics.OverlapBox(
            worldCenter,
            worldHalfExtents,
            boxCollider.transform.rotation,
            ~0,
            QueryTriggerInteraction.Collide
        );

        foreach (Collider overlap in overlaps)
        {
            TryStun(overlap);
        }
    }
}
