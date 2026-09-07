using UnityEngine;

/// <summary>
/// Pasang di objek Pintu yang sudah punya Rigidbody + Hinge Joint (setup fisik),
/// DAMPINGAN dengan komponen InteractableObject (bukan menggantikannya).
///
/// Cara hubungkan: di Inspector komponen InteractableObject pada Pintu, buka
/// bagian "On Interact ()", klik "+", drag GameObject Pintu ini ke slot object,
/// lalu di dropdown function pilih PintuFisik > DorongPintu(). Dengan begitu,
/// setiap kali PlayerInteractor memanggil InteractableObject.Interact(), UnityEvent
/// akan otomatis memanggil DorongPintu() di script ini juga.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(HingeJoint))]
public class PintuFisik : MonoBehaviour
{
    [Tooltip("Seberapa besar dorongan torque saat tombol E/interaksi PC dipakai. Sesuaikan sampai terasa natural.")]
    public float kekuatanDorongan = 8f;

    [Tooltip("true = selalu dorong ke arah yang sama tiap kali dipanggil, false = bergantian buka/tutup")]
    public bool arahTetap = true;

    private Rigidbody rb;
    private bool sedangTerbuka = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    /// <summary>
    /// Dipanggil lewat UnityEvent "On Interact ()" milik InteractableObject di Inspector.
    /// Memberi dorongan fisik nyata ke pintu lewat Hinge Joint yang sudah ada.
    /// </summary>
    public void DorongPintu()
    {
        float arah = arahTetap ? 1f : (sedangTerbuka ? -1f : 1f);
        rb.AddTorque(transform.up * kekuatanDorongan * arah, ForceMode.Impulse);
        sedangTerbuka = !sedangTerbuka;

        Debug.Log("<color=cyan>[Pintu]</color> Didorong lewat interaksi (tombol E / klik).");
    }
}