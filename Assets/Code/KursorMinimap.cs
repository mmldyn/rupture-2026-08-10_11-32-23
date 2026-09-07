using UnityEngine;

public class KursorMinimap : MonoBehaviour
{
    [Header("Referensi")]
    [Tooltip("DEPRECATED - dibiarkan untuk kompatibilitas lama, tidak dipakai lagi kalau Player VR/PC di bawah sudah diisi")]
    public Transform player;

    [Tooltip("Masukkan Main Camera Player VR (XR Origin) ke sini")]
    public Transform playerVR;

    [Tooltip("Masukkan Main Camera Player PC (Player_Dummy_Rig) ke sini")]
    public Transform playerPC;

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
        // SETELAH pergerakan kamera VR/PC selesai di-render (mencegah visual yang bergetar/jitter).

        Transform playerAktif = TentukanPlayerAktif();
        if (playerAktif == null || kursorUI == null) return;

        // Ambil arah tengokan kepala/kamera Player (sumbu Y) - otomatis dari
        // rig mana pun yang sedang aktif (VR atau PC)
        float arahPandang = playerAktif.eulerAngles.y;

        // Putar ikon panah di UI (sumbu Z). 
        // Kita gunakan minus (-) karena arah rotasi 2D dan 3D di Unity berlawanan.
        kursorUI.localEulerAngles = new Vector3(0f, 0f, -arahPandang + kalibrasiPanah);
    }

    /// <summary>
    /// Pilih otomatis Transform player mana yang sedang aktif di Hierarchy
    /// (VR atau PC), sama seperti pola yang dipakai NavMeshGPS/KameraMinimap.
    /// </summary>
    private Transform TentukanPlayerAktif()
    {
        if (playerVR != null && playerVR.gameObject.activeInHierarchy)
            return playerVR;

        if (playerPC != null && playerPC.gameObject.activeInHierarchy)
            return playerPC;

        // Fallback ke field lama kalau VR/PC belum diisi
        if (player != null)
            return player;

        return null;
    }
}