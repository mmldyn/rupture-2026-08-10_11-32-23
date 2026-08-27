using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class DebuBenturan : MonoBehaviour
{
    [Header("Referensi Debu")]
    [Tooltip("Masukkan Prefab P_Debu_Gempa ke sini")]
    public GameObject prefabDebu; 
    
    [Header("Sensitivitas Benturan")]
    public float minimalKecepatan = 0.5f; 

    private void OnCollisionEnter(Collision collision)
    {
        // SOLUSI BUG: Abaikan semua benturan selama 2 detik pertama saat game baru mulai (fase settling)
        if (Time.timeSinceLevelLoad < 2.0f)
        {
            return; 
        }

        // Abaikan jika yang menabrak adalah Player
// Cek seberapa keras benda membentur lantai/objek lain
        if (collision.relativeVelocity.magnitude >= minimalKecepatan)
        {
            if (prefabDebu != null)
            {
                ContactPoint titikSentuh = collision.contacts[0];
                Vector3 posisiAman = titikSentuh.point + (Vector3.up * 0.1f);
                
                // 1. Lahirkan debunya dan simpan ke dalam variabel 'debuBaru'
                GameObject debuBaru = Instantiate(prefabDebu, posisiAman, Quaternion.LookRotation(titikSentuh.normal));
                
                // 2. Ambil komponen partikelnya
                ParticleSystem sistemPartikel = debuBaru.GetComponent<ParticleSystem>();
                if (sistemPartikel != null)
                {
                    // 3. PAKSA NYALA! (Jadi tidak peduli Play On Awake dicentang atau tidak)
                    sistemPartikel.Play();
                }

                // 4. Bersihkan dari memori (Hancurkan objek ini otomatis setelah 5 detik)
                Destroy(debuBaru, 5f);
            }
        }

    }
    
}