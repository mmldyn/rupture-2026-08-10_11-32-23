using UnityEngine;
using System.Collections; // Wajib untuk fitur Coroutine (timer)

public class EarthquakeUIManager : MonoBehaviour
{
    [Header("Slot Asset Pop-Up")]
    [Tooltip("Masukkan objek/grup UI peringatan gempa ke sini")]
    public GameObject popupGempaMulai;
    
    [Tooltip("Masukkan objek/grup UI arahan evakuasi ke sini")]
    public GameObject popupGempaBerhenti;

    [Header("Pengaturan Waktu")]
    [Tooltip("Berapa detik pop-up evakuasi tampil sebelum hilang sendiri?")]
    public float durasiTampilEvakuasi = 5f;

    void Start()
    {
        // Pastikan layar bersih dari pop-up saat game baru dimulai
        SembunyikanSemua();
    }

    // Panggil fungsi ini tepat saat guncangan gempa dimulai
    public void TampilkanPeringatanGempa()
    {
        SembunyikanSemua(); // Bersihkan layar dulu
        if (popupGempaMulai != null) 
            popupGempaMulai.SetActive(true);
            
        Debug.Log("<color=red>[UI Instruktur]</color> Pop-up Peringatan (Drop & Cover) Muncul!");
    }

    // Panggil fungsi ini tepat saat guncangan gempa selesai
    public void TampilkanArahanEvakuasi()
    {
        SembunyikanSemua();
        if (popupGempaBerhenti != null)
        {
            popupGempaBerhenti.SetActive(true);
            // Jalankan timer untuk menyembunyikan pesan ini otomatis
            StartCoroutine(TungguDanSembunyikan(popupGempaBerhenti, durasiTampilEvakuasi));
        }
        
        Debug.Log("<color=green>[UI Instruktur]</color> Pop-up Arahan Evakuasi Muncul!");
    }

    public void SembunyikanSemua()
    {
        if (popupGempaMulai != null) popupGempaMulai.SetActive(false);
        if (popupGempaBerhenti != null) popupGempaBerhenti.SetActive(false);
    }

    // Timer rahasia di belakang layar
    private IEnumerator TungguDanSembunyikan(GameObject popup, float waktu)
    {
        yield return new WaitForSeconds(waktu);
        popup.SetActive(false);
    }

    // --- FITUR TESTING SEMENTARA (Hapus saat disambungkan ke sistem gempa asli) ---
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) TampilkanPeringatanGempa();   // Tekan Angka 1 di keyboard
        if (Input.GetKeyDown(KeyCode.Alpha2)) TampilkanArahanEvakuasi();    // Tekan Angka 2 di keyboard
    }
}