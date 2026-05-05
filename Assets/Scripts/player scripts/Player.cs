using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using System.Reflection;
using System.Collections;

public class Player : MonoBehaviour
{
    public Rigidbody body;
    public WheelCollider frontRightWheel, frontLeftWheel, rearRightWheel, rearLeftWheel;
    public float driveSpeed, steerSpeed, speedLimit, brakeSpeed;
    public Animator animator;
    public HeadbuttHitbox headbuttHitbox;
    public float headbuttHitTime = 0.35f;
    public float headbuttActiveTime = 0.15f;
    public float headbuttCooldown = 0.8f;
    public float resetLiftHeight = 1f;
    public float resetCooldown = 2f;
    private int health;
    public TextMeshProUGUI healthText;

    InputAction moveAction;
    InputAction brakeInput;
    InputAction toggleCursorAction;
    InputAction attackInput;
    InputAction resetPositionInput;

    float steerInput, driveInput;

    private bool isCursorLocked = true;
    private bool isAttacking;
    private float nextResetTime;

    private void Start()
    {
        gameObject.tag = "player";

        moveAction = InputSystem.actions.FindAction("Move");
        brakeInput = InputSystem.actions.FindAction("Crouch");

        toggleCursorAction = InputSystem.actions.FindAction("ToggleCursor");
        attackInput = new InputAction("Headbutt", binding: "<Mouse>/leftButton");
        attackInput.Enable();
        resetPositionInput = new InputAction("ResetPosition", binding: "<Keyboard>/r");
        resetPositionInput.Enable();

        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }

        if (headbuttHitbox == null)
        {
            headbuttHitbox = GetComponentInChildren<HeadbuttHitbox>(true);
        }

        if (headbuttHitbox != null)
        {
            headbuttHitbox.DisableHitbox();
        }

        health = 10;
        setHealthText();

        LockCursor();
        TuneThirdPersonCamera();
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

        if (attackInput != null && attackInput.triggered)
        {
            StartHeadbutt();
        }

        if (resetPositionInput != null && resetPositionInput.triggered)
        {
            ResetPosition();
        }

        Vector2 moveInput = moveAction.ReadValue<Vector2>();
        steerInput = moveInput.x;
        driveInput = moveInput.y;
        
    }

    private void OnDestroy()
    {
        if (attackInput != null)
        {
            attackInput.Disable();
            attackInput.Dispose();
        }

        if (resetPositionInput != null)
        {
            resetPositionInput.Disable();
            resetPositionInput.Dispose();
        }
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

    private void ResetPosition()
    {
        if (Time.time < nextResetTime)
        {
            return;
        }

        nextResetTime = Time.time + resetCooldown;

        if (body != null)
        {
            body.linearVelocity = Vector3.zero;
            body.angularVelocity = Vector3.zero;
        }

        Vector3 position = transform.position;
        position.y += resetLiftHeight;
        transform.position = position;

        float currentYaw = transform.eulerAngles.y;
        transform.rotation = Quaternion.Euler(0f, currentYaw, 0f);
    }

    private void StartHeadbutt()
    {
        if (isAttacking)
        {
            return;
        }

        StartCoroutine(HeadbuttRoutine());
    }

    private IEnumerator HeadbuttRoutine()
    {
        isAttacking = true;

        if (animator != null)
        {
            animator.SetTrigger("Headbutt");
        }

        yield return new WaitForSeconds(headbuttHitTime);

        if (headbuttHitbox != null)
        {
            headbuttHitbox.EnableHitbox();
        }

        yield return new WaitForSeconds(headbuttActiveTime);

        if (headbuttHitbox != null)
        {
            headbuttHitbox.DisableHitbox();
        }

        yield return new WaitForSeconds(Mathf.Max(0f, headbuttCooldown - headbuttHitTime - headbuttActiveTime));

        isAttacking = false;
    }

    private void OnCollisionEnter(Collision collision){
        if(collision.gameObject.CompareTag("enemy")){
            EnemyMovement enemy = collision.gameObject.GetComponentInParent<EnemyMovement>();
            if (enemy != null && enemy.IsStunned)
            {
                return;
            }

            health--;
            setHealthText();
        }

        if(collision.gameObject.CompareTag("Vehicle")){
            health-=3;
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

    private void TuneThirdPersonCamera()
    {
        GameObject cameraObject = GameObject.Find("ThirdPersonCamera");
        if (cameraObject == null)
        {
            return;
        }

        foreach (MonoBehaviour component in cameraObject.GetComponents<MonoBehaviour>())
        {
            if (component == null)
            {
                continue;
            }

            string typeName = component.GetType().Name;
            if (typeName == "CinemachineOrbitalFollow")
            {
                SetMember(component, "TargetOffset", new Vector3(0f, 0.9f, 0f));
                SetMember(component, "Radius", 6f);
                SetNestedMember(component, "Orbits.Top.Radius", 6f);
                SetNestedMember(component, "Orbits.Center.Radius", 6f);
                SetNestedMember(component, "Orbits.Bottom.Radius", 5.5f);
            }
            else if (typeName == "CinemachineRotationComposer")
            {
                SetMember(component, "TargetOffset", new Vector3(0f, 0.75f, 0f));
            }
            else if (typeName == "CinemachineDeoccluder")
            {
                component.enabled = true;
                SetMember(component, "IgnoreTag", "player");
            }
        }
    }

    private static void SetMember(object target, string memberName, object value)
    {
        BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
        FieldInfo field = target.GetType().GetField(memberName, flags);
        if (field != null)
        {
            field.SetValue(target, value);
            return;
        }

        PropertyInfo property = target.GetType().GetProperty(memberName, flags);
        if (property != null && property.CanWrite)
        {
            property.SetValue(target, value, null);
        }
    }

    private static void SetNestedMember(object target, string memberPath, object value)
    {
        string[] memberNames = memberPath.Split('.');
        object current = target;
        MemberAccess[] chain = new MemberAccess[memberNames.Length - 1];

        for (int i = 0; i < memberNames.Length - 1; i++)
        {
            MemberInfo member = FindMember(current.GetType(), memberNames[i]);
            if (member == null)
            {
                return;
            }

            chain[i] = new MemberAccess(current, member);
            current = GetMemberValue(current, member);
            if (current == null)
            {
                return;
            }
        }

        MemberInfo leafMember = FindMember(current.GetType(), memberNames[memberNames.Length - 1]);
        if (leafMember == null)
        {
            return;
        }

        SetMemberValue(current, leafMember, value);

        for (int i = chain.Length - 1; i >= 0; i--)
        {
            SetMemberValue(chain[i].Target, chain[i].Member, current);
            current = chain[i].Target;
        }
    }

    private static MemberInfo FindMember(System.Type type, string memberName)
    {
        BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
        FieldInfo field = type.GetField(memberName, flags);
        if (field != null)
        {
            return field;
        }

        return type.GetProperty(memberName, flags);
    }

    private static object GetMemberValue(object target, MemberInfo member)
    {
        FieldInfo field = member as FieldInfo;
        if (field != null)
        {
            return field.GetValue(target);
        }

        PropertyInfo property = member as PropertyInfo;
        return property != null ? property.GetValue(target, null) : null;
    }

    private static void SetMemberValue(object target, MemberInfo member, object value)
    {
        FieldInfo field = member as FieldInfo;
        if (field != null)
        {
            field.SetValue(target, value);
            return;
        }

        PropertyInfo property = member as PropertyInfo;
        if (property != null && property.CanWrite)
        {
            property.SetValue(target, value, null);
        }
    }

    private struct MemberAccess
    {
        public readonly object Target;
        public readonly MemberInfo Member;

        public MemberAccess(object target, MemberInfo member)
        {
            Target = target;
            Member = member;
        }
    }
}
