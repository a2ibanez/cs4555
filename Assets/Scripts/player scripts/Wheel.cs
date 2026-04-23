using UnityEngine;

public class Wheel : MonoBehaviour
{
    public WheelCollider wheelCollider;
    public Transform wheelMesh;
    public bool wheelTurn;

    private Quaternion startingLocalRotation;

    private void Start()
    {
        if (wheelMesh != null)
        {
            startingLocalRotation = wheelMesh.localRotation;
        }
    }

    private void LateUpdate()
    {
        if (wheelMesh == null)
        {
            return;
        }

        if (wheelMesh.name == "Wheel_FL" || wheelMesh.name == "Wheel_FR")
        {
            float steerAngle = wheelCollider != null ? wheelCollider.steerAngle : 0f;
            wheelMesh.localRotation = Quaternion.Euler(0f, 90f + steerAngle, 0f);
            return;
        }

        wheelMesh.localRotation = startingLocalRotation;
    }
}
