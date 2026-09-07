using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteractor : MonoBehaviour
{
    [Header("Pengaturan Interaksi")]
    [Tooltip("Jarak maksimal tangan/mata bisa menjangkau benda (dalam meter)")]
    public float interactRange = 3f;

    [Tooltip("Sumber arah ray: Main Camera (PC) atau Right Controller (VR)")]
    public Transform rayOrigin;

    [Header("Input PC (opsional, kosongkan di instance VR)")]
    public bool gunakanInputPC = true;

    [Header("Input VR (opsional, kosongkan di instance PC)")]
    [Tooltip("Contoh: XRI RightHand Interaction/Activate, atau tombol grip/trigger yang Anda pakai")]
    public InputActionReference tombolInteraksiVR;

    void Reset()
    {
        // fallback supaya field lama (playerCamera) tidak hilang total kalau ada referensi lama
        if (rayOrigin == null && GetComponent<Camera>() != null)
            rayOrigin = transform;
    }

    void Update()
    {
        if (rayOrigin == null) return;

        Ray ray = new Ray(rayOrigin.position, rayOrigin.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactRange))
        {
            InteractableObject interactable = hit.collider.GetComponentInParent<InteractableObject>();

            if (interactable != null)
            {
                Debug.DrawLine(ray.origin, hit.point, Color.green);

                bool inputPC = gunakanInputPC && (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.E));
                bool inputVR = tombolInteraksiVR != null && tombolInteraksiVR.action != null
                               && tombolInteraksiVR.action.WasPressedThisFrame();

                if (inputPC || inputVR)
                {
                    interactable.Interact();
                }
            }
            else
            {
                Debug.DrawLine(ray.origin, hit.point, Color.yellow);
            }
        }
    }
}