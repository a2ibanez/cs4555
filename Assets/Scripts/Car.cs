using UnityEngine;
using UnityEngine.InputSystem;

public class Car : MonoBehaviour
{
    public Rigidbody body;
    public WheelCollider frontRightWheel, frontLeftWheel, rearRightWheel, rearLeftWheel;
    public float driveSpeed, steerSpeed, speedLimit, brakeSpeed;

    InputAction moveAction;
    InputAction brakeInput;
    float steerInput, driveInput;

    private void Start()
    {
        moveAction = InputSystem.actions.FindAction("Move");
        brakeInput = InputSystem.actions.FindAction("Crouch");
    }

    void Update()
    {
        Vector2 moveInput = moveAction.ReadValue<Vector2>();
        steerInput = moveInput.x;
        driveInput = moveInput.y;
        
    }

    void FixedUpdate()
    {
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
}
