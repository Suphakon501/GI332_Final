using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode; // 1. เพิ่ม namespace นี้

[RequireComponent(typeof(Rigidbody))]
public class SubmarineController : NetworkBehaviour // 2. เปลี่ยนเป็น NetworkBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 15f;
    [SerializeField] private float strafeSpeed = 10f;
    [SerializeField] private float verticalSpeed = 12f;

    [Header("Mouse Look")]
    [SerializeField] private float mouseSensitivity = 0.09f;
    [SerializeField] private float smoothRotation = 15f;

    private Rigidbody rb;
    private PlayerInputActions inputActions;

    private Vector2 moveInput;
    private Vector2 lookInput;
    private float upDownInput;

    private float rotationX;
    private float rotationY;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        inputActions = new PlayerInputActions();

        rb.useGravity = false;
        rb.drag = 1.5f;
        rb.angularDrag = 2.5f;
    }

    private void OnEnable()
    {
        // 3. สำคัญมาก: ถ้าไม่ใช่เจ้าของ (IsOwner) ไม่ต้องเปิดใช้งาน Input
        // เพื่อไม่ให้เครื่องเราไปรับ Input แล้วสั่งตัวละครคนอื่น
    }

    // ใช้ OnNetworkSpawn แทน OnEnable สำหรับการ Setup ของ Netcode
    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            inputActions.Enable();

            inputActions.Player.Move.performed += OnMove;
            inputActions.Player.Move.canceled += OnMove;

            inputActions.Player.Look.performed += OnLook;
            inputActions.Player.Look.canceled += OnLook;

            inputActions.Player.UpDown.performed += OnUpDown;
            inputActions.Player.UpDown.canceled += OnUpDown;

            LockCursor();
        }
    }

    public override void OnNetworkDespawn()
    {
        if (IsOwner)
        {
            inputActions.Disable();
        }
    }

    private void OnMove(InputAction.CallbackContext ctx) => moveInput = ctx.ReadValue<Vector2>();
    private void OnLook(InputAction.CallbackContext ctx) => lookInput = ctx.ReadValue<Vector2>();
    private void OnUpDown(InputAction.CallbackContext ctx) => upDownInput = ctx.ReadValue<float>();

    private void Update()
    {
        // 4. ถ้าไม่ใช่เจ้าของ ไม่ต้องคำนวณ Rotation หรือ Input ในเครื่องนี้
        if (!IsOwner) return;

        rotationY += lookInput.x * mouseSensitivity;
        rotationX -= lookInput.y * mouseSensitivity;
        rotationX = Mathf.Clamp(rotationX, -70f, 70f);

        Quaternion targetRotation = Quaternion.Euler(rotationX, rotationY, 0f);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            Time.deltaTime * smoothRotation
        );
    }

    private void FixedUpdate()
    {
        // 5. ถ้าไม่ใช่เจ้าของ ไม่ต้องใส่แรง (Force) ให้ตัวละคร
        if (!IsOwner) return;

        Vector3 moveDir =
            transform.forward * moveInput.y * moveSpeed +
            transform.right * moveInput.x * strafeSpeed +
            transform.up * upDownInput * verticalSpeed;

        rb.AddForce(moveDir, ForceMode.Acceleration);
    }

    private void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}