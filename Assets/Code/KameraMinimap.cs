using UnityEngine;

public class KameraMinimap : MonoBehaviour
{
    [Header("Target & Posisi")]
    public Transform player; // Tarik objek Player VR Anda ke sini
    public float ketinggianPeta = 50f; // Seberapa tinggi kamera dari player

    void LateUpdate()
    {
        if (player != null)
        {
            // 1. Kamera mengikuti posisi X dan Z player
            Vector3 posisiBaru = player.position;
            posisiBaru.y = player.position.y + ketinggianPeta;
            transform.position = posisiBaru;
        }
    }
}