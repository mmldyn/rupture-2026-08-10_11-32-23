using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

/// <summary>
/// Pasang di objek yang sama dengan XR Grab Interactable (misal Pintu).
/// Kalau jarak antara titik genggam (Attach Transform) dan tangan yang
/// menggenggam melebihi batas tertentu, paksa lepas grab-nya secara otomatis
/// - meniru tangan yang "terlepas" dari gagang, sekaligus mencegah Hinge Joint
/// dipaksa tarik terus-menerus sampai fisikanya tidak stabil.
/// </summary>
[RequireComponent(typeof(XRGrabInteractable))]
[RequireComponent(typeof(HingeJoint))]
public class LepasOtomatisJikaJauh : MonoBehaviour
{
    [Tooltip("Jarak maksimal (meter) sebelum tangan otomatis 'terlepas' dari gagang")]
    public float jarakMaksimal = 0.35f;

    [Tooltip("Toleransi derajat dari limit sebelum dianggap 'mentok' (misal 2 = dianggap mentok kalau sudah dalam 2 derajat dari batas)")]
    public float toleransiMentok = 2f;

    private XRGrabInteractable grabInteractable;
    private HingeJoint hinge;

    void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
        hinge = GetComponent<HingeJoint>();
    }

    void Update()
    {
        if (!grabInteractable.isSelected) return;
        if (grabInteractable.interactorsSelecting.Count == 0) return;

        var interactor = grabInteractable.interactorsSelecting[0];

        // Cek 1: apakah pintu sudah mentok di limit rotasinya?
        float sudutSekarang = hinge.angle;
        bool sudahMentok = sudutSekarang <= (hinge.limits.min + toleransiMentok)
                            || sudutSekarang >= (hinge.limits.max - toleransiMentok);

        // Cek 2: apakah tangan sudah terlalu jauh dari titik genggam?
        Transform titikGenggamObjek = grabInteractable.attachTransform != null
            ? grabInteractable.attachTransform
            : grabInteractable.transform;
        Transform titikGenggamTangan = interactor.GetAttachTransform(grabInteractable);
        float jarak = Vector3.Distance(titikGenggamObjek.position, titikGenggamTangan.position);
        bool terlaluJauh = jarak > jarakMaksimal;

        if (sudahMentok || terlaluJauh)
        {
            grabInteractable.interactionManager.CancelInteractableSelection(
                (IXRSelectInteractable)grabInteractable
            );

            string alasan = sudahMentok ? "pintu sudah mentok" : "tangan terlalu jauh";
            Debug.Log($"<color=orange>[Pintu]</color> Tangan otomatis terlepas dari gagang ({alasan}).");
        }
    }
}