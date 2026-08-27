using UnityEngine;

public class DoorController : MonoBehaviour
{
    [Header("Status Pintu")]
    public bool isOpen = false;

    [Header("Pengaturan Putaran")]
    [Tooltip("Derajat saat pintu terbuka penuh")]
    public float openAngle = 90f; 
    
    [Tooltip("Derajat saat pintu tertutup")]
    public float closeAngle = 0f; 
    
    [Tooltip("Kecepatan pintu mengayun")]
    public float smoothSpeed = 5f; 

    private Quaternion targetRotation;

    void Start()
    {
        // Menyimpan rotasi saat game baru mulai
        targetRotation = transform.localRotation;
    }

    void Update()
    {
        // Menentukan ke arah mana pintu harus berputar berdasarkan status 'isOpen'
        float currentYAngle = isOpen ? openAngle : closeAngle;
        targetRotation = Quaternion.Euler(0, currentYAngle, 0);

        // Memutar pintu secara halus (Smooth) setiap frame
        transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, Time.deltaTime * smoothSpeed);
    }

    // Fungsi ini akan dipanggil (di-trigger) oleh sistem klik kita
    public void ToggleDoor()
    {
        isOpen = !isOpen; // Jika tertutup jadi terbuka, jika terbuka jadi tertutup
        Debug.Log($"<color=yellow>[Pintu]</color> Status pintu: {(isOpen ? "Terbuka" : "Tertutup")}");
    }
}