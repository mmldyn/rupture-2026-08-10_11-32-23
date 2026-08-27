using UnityEngine;

public class SkyboxRotator : MonoBehaviour
{
    [Header("Pengaturan Awan (Skybox)")]
    [Tooltip("Kecepatan pergerakan awan. Angka kecil agar natural.")]
    public float rotationSpeed = 1.2f;

    void Update()
    {
        // Memutar Skybox secara perlahan setiap frame
        RenderSettings.skybox.SetFloat("_Rotation", Time.time * rotationSpeed);
    }
}