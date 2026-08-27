using UnityEngine;

public class KursorMinimap : MonoBehaviour
{
    [Header("Referensi")]
    [Tooltip("Masukkan objek Main Camera (Mata Player VR) ke sini")]
    public Transform player; 
    
    [Header("Pengaturan Kalibrasi")]
    [Tooltip("Isi dengan 90, -90, atau 180 jika arah panah tidak lurus dengan pandangan mata")]
    public float kalibrasiPanah = 0f;

    private RectTransform kursorUI;

    void Start()
    {
        // Mengambil komponen UI bawaan Unity dari ikon kursor ini
        kursorUI = GetComponent<RectTransform>();
    }

    void LateUpdate()
    {
        // Kita menggunakan LateUpdate agar rotasi kursor UI dieksekusi 
        // SETELAH pergerakan kamera VR selesai di-render (mencegah visual yang bergetar/jitter).
        
        if (player != null && kursorUI != null)
        {
            // Ambil arah tengokan kepala/kamera Player (sumbu Y)
            float arahPandang = player.eulerAngles.y;
            
            // Putar ikon panah di UI (sumbu Z). 
            // Kita gunakan minus (-) karena arah rotasi 2D dan 3D di Unity berlawanan.
            kursorUI.localEulerAngles = new Vector3(0f, 0f, -arahPandang + kalibrasiPanah);
        }
    }
}