using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Pengaturan Nyawa")]
    public int maxHealth = 100;
    public int currentHealth;

    [Header("Status Perlindungan")]
    [Tooltip("Jika true, pemain tidak akan menerima damage dari reruntuhan (diatur otomatis oleh script SafeZone)")]
    public bool isProtected = false; 

    [Header("Pengaturan Benturan (Physics Damage)")]
    [Tooltip("Kecepatan tabrakan minimal agar pemain menerima damage (mencegah damage saat sekadar bersentuhan)")]
    public float minimumImpactForce = 3f;
    
    [Tooltip("Faktor pengali damage. Semakin besar, semakin sakit.")]
    public float damageMultiplier = 2f;

    private bool isDead = false;

    void Start()
    {
        // Mengisi nyawa penuh saat game dimulai
        currentHealth = maxHealth;
        isProtected = false; // Memastikan perlindungan mati saat baru mulai
        Debug.Log($"<color=cyan>[Sistem Health]</color> Pemain siap. Nyawa: {currentHealth}/{maxHealth}");
    }

    // Fungsi bawaan Unity yang terpanggil OTOMATIS setiap kali kapsul pemain tertabrak sesuatu
    void OnCollisionEnter(Collision collision)
    {
        if (isDead) return; // Jika sudah mati, abaikan tabrakan selanjutnya

        // Mengecek apakah pemain sedang berada di Zona Aman (di bawah meja & jongkok)
        if (isProtected)
        {
            // Jika pemain terlindungi, abaikan damage dari benda yang jatuh menimpa meja
            return; 
        }

        // Memastikan benda yang menabrak memiliki Rigidbody (seperti kubus reruntuhan)
        if (collision.rigidbody != null)
        {
            // Menghitung seberapa keras benturannya (kecepatan relatif antara pemain dan benda)
            float impactForce = collision.relativeVelocity.magnitude;

            // Jika benturannya cukup keras (bukan sekadar tersenggol)
            if (impactForce >= minimumImpactForce)
            {
                // Rumus Damage Realistis = Kekuatan Benturan x Massa Benda x Pengali
                float calculatedDamage = impactForce * collision.rigidbody.mass * damageMultiplier;
                
                // Membulatkan desimal menjadi angka bulat (int)
                int finalDamage = Mathf.RoundToInt(calculatedDamage);

                // Terapkan damage ke pemain jika angkanya lebih dari 0
                if (finalDamage > 0)
                {
                    TakeDamage(finalDamage, collision.gameObject.name);
                }
            }
        }
    }

    // Fungsi untuk mengurangi nyawa
    public void TakeDamage(int damageAmount, string sourceName)
    {
        currentHealth -= damageAmount;
        
        // Memastikan nyawa tidak pernah minus (berhenti di 0)
        currentHealth = Mathf.Max(currentHealth, 0);

        Debug.Log($"<color=orange>[Terluka!]</color> Tertimpa <b>{sourceName}</b>. Damage: {damageAmount}. Nyawa sisa: {currentHealth}");

        // Cek apakah pemain mati
        if (currentHealth == 0 && !isDead)
        {
            Die();
        }
    }

    private void Die()
    {
        isDead = true;
        Debug.Log("<color=red>[GAME OVER]</color> Pemain tertimpa reruntuhan fatal. Simulasi Gagal.");
        
        // Nanti di sini kita bisa hubungkan dengan layar hitam (Fade to Black) untuk VR
        // atau menampilkan UI menu "Ulangi Simulasi".
    }
}