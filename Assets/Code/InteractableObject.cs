using UnityEngine;
using UnityEngine.Events; // Wajib ditambahkan agar fitur UnityEvent menyala

public class InteractableObject : MonoBehaviour
{
    [Header("Info Objek")]
    public string namaObjek = "Benda Misterius";

    [Header("Aksi Saat Diinteraksi")]
    [Tooltip("Apa yang terjadi saat benda ini diklik? (Bisa diatur langsung dari Inspector)")]
    public UnityEvent onInteract;

    // Fungsi ini dipanggil secara otomatis oleh PlayerInteractor
    public void Interact()
    {
        Debug.Log($"<color=cyan>[Interaksi]</color> Anda berinteraksi dengan: <b>{namaObjek}</b>");
        
        // Mengeksekusi semua perintah yang sudah Anda pasang di panel Inspector
        onInteract.Invoke();
    }
}