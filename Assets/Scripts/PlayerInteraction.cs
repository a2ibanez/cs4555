using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class PlayerInteraction : MonoBehaviour
{
    public Transform holdPoint;

    private GameObject currentPackage;
    private GameObject nearbyPackage;

    public GameObject pickUpPrompt;

    private InputAction pickUpAction;
    private InputAction dropAction;

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

        currentPackage.GetComponent<Package>().OnPickedUp();

        currentPackage.transform.SetParent(holdPoint);
        currentPackage.transform.localPosition = Vector3.zero;

        currentPackage.transform.localRotation = Quaternion.identity;
        Rigidbody rb = currentPackage.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
        }

        pickUpPrompt.SetActive(false);
    }

    void DropPackage()
    {
        currentPackage.transform.SetParent(null);
        currentPackage.transform.position = holdPoint.position;

        Rigidbody rb = currentPackage.GetComponent<Rigidbody>();
        if (rb != null)        {
            rb.isKinematic = false;
        }
        
        currentPackage = null;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Package"))
        {
            nearbyPackage = other.gameObject;
            pickUpPrompt.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Package"))
        {
            nearbyPackage = null;
            pickUpPrompt.SetActive(false);
        }
    }

    public bool HasPackage() => currentPackage != null;
    public GameObject GetCurrentPackage() => currentPackage;
    public void RemoveCurrentPackage() => currentPackage = null;
}