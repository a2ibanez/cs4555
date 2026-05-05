using UnityEngine;
using TMPro;

public class UIArrowPoint : MonoBehaviour
{
    public Transform player;
    public Transform cameraTransform;
    public NavigationManager navigationManager;
    public TMPro.TextMeshProUGUI distanceText;
    public Transform ArrowImage;

    void Awake()
    {
        ResolveMissingReferences();
    }

    // Update is called once per frame
    void Update()
    {
        ResolveMissingReferences();

        if (navigationManager == null || player == null || distanceText == null || ArrowImage == null)
            return;

        if (navigationManager.currentTarget == null)
        {
            distanceText.text = "";
            return;
        }

        Vector3 direction = navigationManager.currentTarget.position - player.position;
        direction.y = 0f;

        float distance = direction.magnitude;
        distanceText.text = Mathf.Round(distance) + "m";

        Transform directionReference = cameraTransform != null ? cameraTransform : Camera.main?.transform;
        if (directionReference == null) { return; }

        Vector3 cameraForward = directionReference.forward;
        cameraForward.y = 0f;

        float signedAngle = Vector3.SignedAngle(cameraForward, direction, Vector3.up);

        ArrowImage.localRotation = Quaternion.Euler(0f, 0f, -signedAngle + 180f);
    }

    void ResolveMissingReferences()
    {
        if (player == null)
        {
            Player activePlayer = FindFirstObjectByType<Player>();
            if (activePlayer != null)
            {
                player = activePlayer.transform;
            }
        }

        if (navigationManager == null)
        {
            navigationManager = FindFirstObjectByType<NavigationManager>();
        }

        if (ArrowImage == null)
        {
            ArrowImage = transform;
        }
    }
}
