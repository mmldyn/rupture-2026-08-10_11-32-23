using UnityEngine;

public class KameraMinimap : MonoBehaviour
{
    [Header("Target & Posisi")]
    [Tooltip("DEPRECATED - dibiarkan untuk kompatibilitas lama, tidak dipakai lagi kalau Player VR/PC di bawah sudah diisi")]
    public Transform player;

    [Tooltip("Transform pemain versi VR (biasanya Main Camera atau root XR Origin (XR Rig))")]
    public Transform playerVR;

    [Tooltip("Transform pemain versi PC (biasanya Main Camera di Player_Dummy_Rig)")]
    public Transform playerPC;

    public float ketinggianPeta = 50f; // Seberapa tinggi kamera dari player

    void LateUpdate()
    {
        Transform playerAktif = TentukanPlayerAktif();
        if (playerAktif == null) return;

        // Kamera mengikuti posisi X dan Z player
        Vector3 posisiBaru = playerAktif.position;
        posisiBaru.y = playerAktif.position.y + ketinggianPeta;
        transform.position = posisiBaru;
    }

    /// <summary>
    /// Pilih otomatis Transform player mana yang sedang aktif di Hierarchy
    /// (VR atau PC), sama seperti pola yang dipakai NavMeshGPS.
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