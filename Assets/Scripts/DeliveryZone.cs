using UnityEngine;

public class DeliveryZone : MonoBehaviour
{
    public int deliveredCount = 0; // count of delivered packages

    private void OnTriggerEnter(Collider other)
    {
        PlayerInteraction player = other.GetComponent<PlayerInteraction>();

        if (player != null && player.HasPackage())
        {
            GameObject package = player.GetCurrentPackage(); // get the current package from the player

            Destroy(package); // destroy the package object to simulate delivery

            player.RemoveCurrentPackage(); // clear the player's current package reference

            deliveredCount++; // increment the delivered count

            Debug.Log("Package delivered! Total delivered: " + deliveredCount);
        }
    }
}
