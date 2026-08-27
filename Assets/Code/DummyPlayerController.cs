using UnityEngine;

[RequireComponent(typeof(Rigidbody))] 
public class DummyPlayerController : MonoBehaviour
{
    [Header("Referensi Objek")]
    public Transform playerCamera;
    public CapsuleCollider playerCollider;
    private Rigidbody rb;

    [Header("Pengaturan Kamera")]
    public float mouseSensitivity = 2f;
    private float xRotation = 0f;

    [Header("Pengaturan Kecepatan")]
    public float walkSpeed = 4f;
    public float sprintSpeed = 7f;
    public float crouchSpeed = 2f;
    public float jumpForce = 5f;

    [Header("Pengaturan Jongkok")]
    public float standingHeight = 2f;
    public float crouchHeight = 1f;
    public float standingCamY = 1.6f;
    public float crouchCamY = 0.8f;

    private float currentSpeed;
    private bool isGrounded;
    
    // Variabel untuk menyimpan status terakhir agar Console tidak spam
    private string lastLogState = "";

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (playerCollider == null) playerCollider = GetComponent<CapsuleCollider>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // --- MELIHAT ---
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        playerCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);

        // --- PIJAKAN ---
        isGrounded = Physics.Raycast(transform.position, Vector3.down, (playerCollider.height / 2) + 0.1f);

        // --- LONCAT ---
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            LogState("Loncat!"); // Log khusus saat menekan spasi
        }

        // --- JONGKOK & LARI ---
        float moveX = Input.GetAxis("Horizontal"); // A/D
        float moveZ = Input.GetAxis("Vertical");   // W/S

        string currentPosture = "Diam";
        
        if (Input.GetKey(KeyCode.LeftControl))
        {
            currentPosture = "Jongkok";
            currentSpeed = crouchSpeed;
            playerCollider.height = Mathf.Lerp(playerCollider.height, crouchHeight, Time.deltaTime * 10f);
            
            Vector3 targetCamPos = new Vector3(playerCamera.localPosition.x, crouchCamY, playerCamera.localPosition.z);
            playerCamera.localPosition = Vector3.Lerp(playerCamera.localPosition, targetCamPos, Time.deltaTime * 10f);
        }
        else
        {
            playerCollider.height = Mathf.Lerp(playerCollider.height, standingHeight, Time.deltaTime * 10f);
            
            Vector3 targetCamPos = new Vector3(playerCamera.localPosition.x, standingCamY, playerCamera.localPosition.z);
            playerCamera.localPosition = Vector3.Lerp(playerCamera.localPosition, targetCamPos, Time.deltaTime * 10f);

            if (Input.GetKey(KeyCode.LeftShift))
            {
                currentPosture = "Lari";
                currentSpeed = sprintSpeed;
            }
            else
            {
                currentPosture = "Jalan";
                currentSpeed = walkSpeed;
            }
        }

        // --- LOGGING STATUS ---
        //  W,A,S,D
        string currentDirection = "";
        if (moveZ > 0) currentDirection += "Maju ";
        else if (moveZ < 0) currentDirection += "Mundur ";

        if (moveX > 0) currentDirection += "Kanan";
        else if (moveX < 0) currentDirection += "Kiri";

        currentDirection = currentDirection.Trim(); // Menghapus spasi kosong berlebih

        string logMessage;
        if (moveX == 0 && moveZ == 0) // Jika tidak ada tombol WASD yang ditekan
        {
            logMessage = Input.GetKey(KeyCode.LeftControl) ? "Status: Jongkok (Diam)" : "Status: Diam";
        }
        else
        {
            logMessage = $"Status: {currentPosture} ke arah {currentDirection}";
        }

        // Mengecek apakah status saat ini berbeda dengan frame sebelumnya
        if (logMessage != lastLogState)
        {
            LogState(logMessage);       // Cetak ke Console
            lastLogState = logMessage;  // Perbarui status memori
        }
    }

    void FixedUpdate()
    {
        // --- BERJALAN (PHYSICS) ---
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        Vector3 moveDirection = (transform.right * moveX + transform.forward * moveZ).normalized;
        Vector3 targetVelocity = moveDirection * currentSpeed;
        rb.velocity = new Vector3(targetVelocity.x, rb.velocity.y, targetVelocity.z);
    }

    // Fungsi khusus agar tampilan log di Console lebih rapi
    private void LogState(string message)
    {
        Debug.Log($"<color=cyan>[Player Logger]</color> {message}");
    }
}