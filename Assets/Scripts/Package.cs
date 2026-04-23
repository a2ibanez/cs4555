using UnityEngine;

public class Package : MonoBehaviour
{
    public void OnPickedUp()
    {
        // animation here
        GetComponent<Collider>().enabled = false;
    }
}
