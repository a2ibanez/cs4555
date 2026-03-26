using UnityEngine;
using TMPro;

public class UIArrowPoint : MonoBehaviour
{
    public Transform player;
    public NavigationManager navigationManager;
    public TMPro.TextMeshProUGUI distanceText;

    // Update is called once per frame
    void Update()
    {

        if (navigationManager.currentTarget == null)
        {
            distanceText.text = "";
        }

        Vector3 direction = navigationManager.currentTarget.position - player.position;
        direction.y = 0f;

        float distance = direction.magnitude;
        distanceText.text = Mathf.Round(distance) + "m";

        Vector3 playerForward = player.forward;
        playerForward.y = 0f;

        float signedAngle = Vector3.SignedAngle(playerForward, direction, Vector3.up);

        transform.localRotation = Quaternion.Euler(0f, 0f, -signedAngle + 180f);
    }
}
