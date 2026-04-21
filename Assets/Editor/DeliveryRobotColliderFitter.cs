using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

[InitializeOnLoad]
public static class DeliveryRobotColliderFitter
{
    private const string MenuPath = "Tools/Fit Delivery Robot Colliders";

    static DeliveryRobotColliderFitter()
    {
        EditorApplication.delayCall += FitIfSceneIsOpen;
    }

    [MenuItem(MenuPath)]
    public static void FitSelectedOrSceneRobot()
    {
        GameObject root = Selection.activeGameObject;

        if (root == null || root.GetComponentInChildren<Player>(true) == null)
        {
            root = GameObject.Find("Robotcar_RotationFix");
        }

        if (root == null)
        {
            Debug.LogWarning("Delivery robot collider fit skipped: could not find Robotcar_RotationFix.");
            return;
        }

        Player player = root.GetComponentInChildren<Player>(true);
        if (player == null || player.body == null)
        {
            Debug.LogWarning("Delivery robot collider fit skipped: could not find the Player component/body Rigidbody.");
            return;
        }

        FitBodyCollider(player.body);
        FitWheelCollider(player.frontRightWheel);
        FitWheelCollider(player.frontLeftWheel);
        FitWheelCollider(player.rearRightWheel);
        FitWheelCollider(player.rearLeftWheel);

        EditorUtility.SetDirty(player.body.gameObject);
        EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        Debug.Log("Delivery robot colliders fitted.");
    }

    private static void FitIfSceneIsOpen()
    {
        if (Application.isPlaying)
        {
            return;
        }

        if (GameObject.Find("Robotcar_RotationFix") != null)
        {
            FitSelectedOrSceneRobot();
        }
    }

    private static void FitBodyCollider(Rigidbody body)
    {
        BoxCollider boxCollider = body.GetComponent<BoxCollider>();
        if (boxCollider == null)
        {
            boxCollider = body.gameObject.AddComponent<BoxCollider>();
        }

        boxCollider.size = new Vector3(1.32f, 0.78f, 1.06f);
        boxCollider.center = new Vector3(-0.18f, 0.43f, 0f);
    }

    private static void FitWheelCollider(WheelCollider wheelCollider)
    {
        if (wheelCollider == null)
        {
            return;
        }

        wheelCollider.center = Vector3.zero;
        wheelCollider.radius = 0.24f;
    }
}
