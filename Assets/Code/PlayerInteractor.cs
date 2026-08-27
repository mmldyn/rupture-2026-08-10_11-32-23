using UnityEngine;

public class PlayerInteractor : MonoBehaviour
{
    [Header("Pengaturan Interaksi")]
    [Tooltip("Jarak maksimal tangan/mata bisa menjangkau benda (dalam meter)")]
    public float interactRange = 3f;
    
    [Tooltip("Masukkan Main Camera dari Player ke sini")]
    public Camera playerCamera;

    void Update()
    {
        // Membuat garis laser imajiner lurus ke depan dari tengah kamera
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        RaycastHit hit;

        // Mengecek apakah laser menabrak sesuatu dalam jarak interactRange
        if (Physics.Raycast(ray, out hit, interactRange))
        {
            // Mengecek apakah benda yang ditabrak laser memiliki script "InteractableObject"
            InteractableObject interactable = hit.collider.GetComponentInParent<InteractableObject>();

            if (interactable != null)
            {
                // Nanti di sini kita bisa hubungkan untuk memunculkan teks UI (Misal: "Tekan E untuk Ambil Tas")
                Debug.DrawLine(ray.origin, hit.point, Color.green); // Bantuan visual (Garis hijau) di jendela Scene

                // Jika tombol Klik Kiri Mouse atau huruf 'E' ditekan
                if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.E))
                {
                    interactable.Interact();
                }
            }
            else
            {
                Debug.DrawLine(ray.origin, hit.point, Color.red); // Garis merah jika benda tidak bisa diinteraksi
            }
        }
    }
}