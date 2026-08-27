using UnityEngine;

public class SafeZone : MonoBehaviour
{
    private PlayerHealth playerHealth;

    // Fungsi ini terpanggil terus-menerus SELAMA ada objek di dalam kotak sensor
    void OnTriggerStay(Collider other)
    {
        // Mengecek apakah yang masuk adalah tubuh pemain (berdasarkan Tag)
        if (other.CompareTag("Player"))
        {
            // Mengambil sistem nyawa pemain
            if (playerHealth == null)
                playerHealth = other.GetComponent<PlayerHealth>();

            // Deteksi apakah menekan tombol Jongkok (Left Control)
            bool isCrouching = Input.GetKey(KeyCode.LeftControl);

            // Logika Keselamatan: Harus di dalam zona DAN sedang jongkok
            if (isCrouching)
            {
                playerHealth.isProtected = true;
                Debug.Log("<color=green>[ZONA AMAN]</color> Pemain berlindung dengan benar (Drop & Cover)!");
            }
            else
            {
                // Jika berdiri di bawah meja, tetap kena damage (karena kepala mentok meja/benda)
                playerHealth.isProtected = false;
                Debug.Log("<color=yellow>[BAHAYA]</color> Pemain di area aman tapi TIDAK JONGKOK!");
            }
        }
    }

    // Fungsi ini terpanggil KETIKA pemain KELUAR dari kotak sensor
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (playerHealth != null)
            {
                playerHealth.isProtected = false; // Matikan perlindungan
            }
            Debug.Log("<color=red>[KELUAR ZONA]</color> Pemain meninggalkan zona aman. Rentan reruntuhan!");
        }
    }
}