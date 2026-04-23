using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class PlayerInteraction : MonoBehaviour
{
    public Transform holdPoint;
    public float dropForwardOffset = 1.25f;
    public float dropGroundCheckHeight = 3f;
    public LayerMask dropGroundLayers = ~0;

    private GameObject currentPackage;
    private GameObject nearbyPackage;

    public GameObject pickUpPrompt;

    private InputAction pickUpAction;
    private InputAction dropAction;

    public NavigationManager navigationManager;

    void Awake()
    {
        // Create actions manually (no asset needed)
        pickUpAction = new InputAction("PickUp", binding: "<Keyboard>/q");
        dropAction = new InputAction("Drop", binding: "<Keyboard>/e");
    }

    void OnEnable()
    {
        pickUpAction.Enable();
        dropAction.Enable();
    }

    void OnDisable()
    {
        pickUpAction.Disable();
        dropAction.Disable();
    }

    void Update()
    {
        if (pickUpAction.WasPressedThisFrame() && nearbyPackage != null && currentPackage == null)
        {
            PickUpPackage();
        }
        else if (dropAction.WasPressedThisFrame() && currentPackage != null)
        {
            DropPackage();
        }
    }

    void PickUpPackage()
    {
        currentPackage = nearbyPackage;
        nearbyPackage = null;

        Package package = currentPackage.GetComponent<Package>();
        if (package != null)
        {
            package.OnPickedUp();
        }

        currentPackage.transform.SetParent(holdPoint);
        currentPackage.transform.localPosition = Vector3.zero;

        currentPackage.transform.localRotation = Quaternion.identity;
        Rigidbody rb = currentPackage.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
        }

        UpdatePickUpPrompt();

        if (navigationManager != null)
        {
            navigationManager.ClearPackageTarget(currentPackage);
        }
    }

    void DropPackage()
    {
        GameObject packageToDrop = currentPackage;
        currentPackage = null;

        Vector3 dropPosition = GetDropPosition(packageToDrop);

        packageToDrop.transform.SetParent(null);
        packageToDrop.transform.position = dropPosition;

        Collider packageCollider = packageToDrop.GetComponent<Collider>();
        if (packageCollider != null)
        {
            packageCollider.enabled = true;
        }

        Rigidbody rb = packageToDrop.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
        }

        if (navigationManager != null)
        {
            navigationManager.SetPackageTarget(packageToDrop);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Package"))
        {
            if (currentPackage != null)
            {
                return;
            }

            nearbyPackage = other.gameObject;
            UpdatePickUpPrompt();

            if (navigationManager != null)
            {
                navigationManager.SetPackageTarget(other.gameObject);
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Package"))
        {
            nearbyPackage = null;
            UpdatePickUpPrompt();
        }
    }

    public bool HasPackage() => currentPackage != null;
    public GameObject GetCurrentPackage() => currentPackage;
    public void RemoveCurrentPackage() => currentPackage = null;

    void UpdatePickUpPrompt()
    {
        if (pickUpPrompt != null)
        {
            pickUpPrompt.SetActive(nearbyPackage != null && currentPackage == null);
        }
    }

    Vector3 GetDropPosition(GameObject packageToDrop)
    {
        Vector3 forward = transform.forward;
        forward.y = 0f;

        if (forward.sqrMagnitude < 0.001f)
        {
            forward = Vector3.forward;
        }

        forward.Normalize();

        Vector3 dropPosition = transform.position - forward * dropForwardOffset;
        Vector3 rayStart = dropPosition + Vector3.up * dropGroundCheckHeight;

        if (Physics.Raycast(rayStart, Vector3.down, out RaycastHit hit, dropGroundCheckHeight * 2f, dropGroundLayers, QueryTriggerInteraction.Ignore))
        {
            dropPosition = hit.point;
            dropPosition.y += GetPackageHalfHeight(packageToDrop);
        }

        return dropPosition;
    }

    float GetPackageHalfHeight(GameObject packageToDrop)
    {
        Renderer packageRenderer = packageToDrop.GetComponentInChildren<Renderer>();
        if (packageRenderer != null)
        {
            return packageRenderer.bounds.extents.y;
        }

        Collider packageCollider = packageToDrop.GetComponent<Collider>();
        if (packageCollider != null && packageCollider.enabled)
        {
            return packageCollider.bounds.extents.y;
        }

        return 0.25f;
    }
}
