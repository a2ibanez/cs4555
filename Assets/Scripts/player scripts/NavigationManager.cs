using UnityEngine;

public class NavigationManager : MonoBehaviour
{
    public PlayerInteraction playerInteraction;

    public GameObject currentPackageObject;
    public Transform deliveryZone;

    public Transform currentTarget;

    // Update is called once per frame
    void Update()
    {
        if (playerInteraction.HasPackage())
        {
            currentTarget = deliveryZone;
        }
        else
        {
            currentPackageObject = FindNearestPackage();

            if (currentPackageObject != null)
            {
                currentTarget = currentPackageObject.transform;
            }
            else
            {
                currentTarget = null;
            }
        }
    }

    public void SetPackageTarget(GameObject package)
    {
        currentPackageObject = package;
    }

    public void ClearPackageTarget(GameObject package)
    {
        if (currentPackageObject == package)
        {
            currentPackageObject = null;
        }
    }

    GameObject FindNearestPackage()
    {
        GameObject[] packages = GameObject.FindGameObjectsWithTag("Package");

        GameObject nearest = null;
        float shortestDistance = Mathf.Infinity;

        foreach (GameObject package in packages)
        {
            if (package == null || !package.activeInHierarchy)
                continue;

            float distance = Vector3.Distance(
                playerInteraction.transform.position,
                package.transform.position
            );

            if (distance < shortestDistance)
            {
                shortestDistance = distance;
                nearest = package;
            }
        }

        return nearest;
    }

}
