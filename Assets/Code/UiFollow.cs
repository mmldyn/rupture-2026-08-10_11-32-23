using UnityEngine;

/// <summary>
/// Membuat Canvas World Space (UI_Manager_VR) selalu berada pada jarak tetap
/// di depan kamera pemain, dan selalu menghadap ke pemain (billboard).
/// Pasang script ini pada GameObject Canvas itu sendiri.
/// </summary>
public class WorldSpaceUIFollow : MonoBehaviour
{
    [Header("Target")]
    [Tooltip("Kamera pemain di VR (Main Camera milik XR Origin, BUKAN Player_Dummy_Rig)")]
    public Transform target;

    [Header("Jarak & Posisi")]
    [Tooltip("Jarak UI dari mata pemain, dalam meter")]
    public float distance = 1.8f;

    [Tooltip("Offset ketinggian tambahan relatif terhadap kamera (0 = sejajar mata)")]
    public float heightOffset = -0.1f;

    [Header("Perilaku Rotasi")]
    [Tooltip("Kalau dicentang, UI hanya berotasi horizontal (yaw) mengikuti pemain, tidak ikut saat pemain menunduk/mendongak. Direkomendasikan untuk kenyamanan VR.")]
    public bool lockPitchRoll = true;

    [Header("Kehalusan Gerakan")]
    [Tooltip("Semakin besar nilainya, semakin cepat UI menyusul posisi target. Set tinggi (misal 15-20) supaya terasa responsif tapi tidak kaku.")]
    public float followSpeed = 12f;

    void LateUpdate()
    {
        if (target == null) return;

        // Hitung posisi target: sejajar arah pandang kamera pada jarak tetap
        Vector3 desiredPosition = target.position + target.forward * distance;
        desiredPosition.y += heightOffset;

        // Gerakkan Canvas secara halus menuju posisi target (bukan snap instan)
        transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * followSpeed);

        // Hitung rotasi agar Canvas menghadap ke kamera
        Vector3 directionToTarget = transform.position - target.position;

        if (lockPitchRoll)
        {
            // Kunci hanya rotasi horizontal, supaya UI tetap tegak vertikal
            directionToTarget.y = 0f;
        }

        if (directionToTarget.sqrMagnitude > 0.0001f)
        {
            Quaternion desiredRotation = Quaternion.LookRotation(directionToTarget);
            transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, Time.deltaTime * followSpeed);
        }
    }
}