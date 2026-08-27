using UnityEngine;

public class TimeOfDayManager : MonoBehaviour
{
    [Header("Referensi Objek")]
    [Tooltip("Tarik Directional Light (Matahari) dari Hierarchy ke sini")]
    public Light sunLight; 

    [Header("Pengaturan Waktu")]
    [Tooltip("Waktu dalam format 24 Jam (Misal 14.5 = Jam 14:30)")]
    [Range(0f, 24f)]
    public float currentTime = 8f; 

    [Header("Sistem Pengacakan (Saat Mulai)")]
    public bool randomizeOnStart = true;
    
    [Tooltip("Batas waktu paling pagi saat diacak")]
    public float minRandomTime = 7.0f; // Jam 7 pagi
    
    [Tooltip("Batas waktu paling sore saat diacak")]
    public float maxRandomTime = 16.0f; // Jam 4 sore

    void Start()
    {
        if (randomizeOnStart)
        {
            // Mengacak nilai dari batas minimum ke maksimum
            currentTime = Random.Range(minRandomTime, maxRandomTime);
            Debug.Log($"<color=yellow>[Sistem Cuaca]</color> Waktu dimulai secara acak pada pukul: <b>{currentTime:F1}</b>");
        }

        // Terapkan rotasi saat game dimulai
        UpdateSunRotation();
    }

    // Fungsi sakti: Meng-update langit secara Real-Time di Editor meskipun game BELUM di Play!
    void OnValidate()
    {
        UpdateSunRotation();
    }

    public void UpdateSunRotation()
    {
        if (sunLight == null) return;

        // RUMUS KONVERSI JAM KE DERAJAT ROTASI:
        // (Jam / 24) * 360 derajat - 90 derajat offset.
        // Minus 90 memastikan jam 06.00 ada di horizon (0 derajat rotasi X)
        float sunRotationX = (currentTime / 24f) * 360f - 90f;
        
        // Aplikasikan rotasi. Sumbu Y (30f) dibuat agar matahari tidak terbit lurus utara-selatan
        sunLight.transform.rotation = Quaternion.Euler(sunRotationX, 30f, 0f);
    }
}