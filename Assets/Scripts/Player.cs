using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class Player : MonoBehaviour
{
    public Rigidbody body;
    public WheelCollider frontRightWheel, frontLeftWheel, rearRightWheel, rearLeftWheel;
    public float driveSpeed, steerSpeed, speedLimit, brakeSpeed;
    private int health;
    public TextMeshProUGUI healthText;

    InputAction moveAction;
    InputAction brakeInput;
    InputAction toggleCursorAction;

    float steerInput, driveInput;

    private bool isCursorLocked = true;

    private void Start()
    {
        moveAction = InputSystem.actions.FindAction("Move");
        brakeInput = InputSystem.actions.FindAction("Crouch");

        toggleCursorAction = InputSystem.actions.FindAction("ToggleCursor");

        health = 10;
        setHealthText();

        LockCursor();
    }

    void Update()
    {
        if (toggleCursorAction != null && toggleCursorAction.triggered)
        {
            if (isCursorLocked) 
            {
                UnlockCursor();
            } else
            {
                LockCursor();
            }
        }

        if (!isCursorLocked) return;

        Vector2 moveInput = moveAction.ReadValue<Vector2>();
        steerInput = moveInput.x;
        driveInput = moveInput.y;
        
    }

    void FixedUpdate()
    {
        if (!isCursorLocked)
        {
            rearRightWheel.motorTorque = 0;
            rearLeftWheel.motorTorque = 0;
            rearRightWheel.brakeTorque = brakeSpeed;
            rearLeftWheel.brakeTorque = brakeSpeed;
            return;
        }

        drive();
        brake();
        steerAngle();
    }

    private void drive(){

        // Debug.Log(body.linearVelocity.magnitude);
        
        float currentSpeed = body.linearVelocity.magnitude;
        float engine = driveInput * driveSpeed;

        if(currentSpeed > speedLimit){
            rearRightWheel.motorTorque = 0;
            rearLeftWheel.motorTorque = 0;
        } else{
            rearRightWheel.motorTorque = engine;
            rearLeftWheel.motorTorque = engine;
        }
    }

    private void brake(){

        if(brakeInput.IsPressed()){
            rearRightWheel.brakeTorque = brakeSpeed;
            rearLeftWheel.brakeTorque = brakeSpeed;
        } else {
            rearRightWheel.brakeTorque = 0;
            rearLeftWheel.brakeTorque = 0;
        }
    }

    private void steerAngle(){

        frontRightWheel.steerAngle = steerSpeed * steerInput;
        frontLeftWheel.steerAngle = steerSpeed * steerInput;
    }

    private void OnCollisionEnter(Collision collision){
        if(collision.gameObject.CompareTag("enemy")){
            health--;
            setHealthText();
        }

        if(collision.gameObject.CompareTag("Vehicle")){
            health-=5;
            setHealthText();
        }
        
    }

    private void setHealthText(){
        healthText.text = "Health: " + health.ToString();
        if(health <= 0){
            GameManager.instance.GameOver();
            Destroy(gameObject);
        }
    }

    private void LockCursor() {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        isCursorLocked = true;
    }

    private void UnlockCursor() {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        isCursorLocked = false;
    }
}
