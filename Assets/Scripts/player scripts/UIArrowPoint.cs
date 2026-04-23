using UnityEngine;
using TMPro;

public class UIArrowPoint : MonoBehaviour
{
    public Transform player;
    public Transform cameraTransform;
    public NavigationManager navigationManager;
    public TMPro.TextMeshProUGUI distanceText;

    // Update is called once per frame
    void Update()
    {

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
        if (directionReference == null)
        {
            return;
        }

        Vector3 cameraForward = directionReference.forward;
        cameraForward.y = 0f;

        float signedAngle = Vector3.SignedAngle(cameraForward, direction, Vector3.up);

        transform.localRotation = Quaternion.Euler(0f, 0f, -signedAngle + 180f);
    }
}
