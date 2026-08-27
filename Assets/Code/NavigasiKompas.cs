using UnityEngine;
using UnityEngine.UI;

public class KompasNavigasi : MonoBehaviour
{
    [Header("Referensi Objek")]
    [Tooltip("Masukkan objek Kamera VR (Mata Player) ke sini")]
    public Transform mataPlayer;
    [Tooltip("Masukkan UI RawImage Pita_Kompas ke sini")]
    public RawImage pitaKompas;

    [Header("Pengaturan Tampilan")]
    [Tooltip("Semakin kecil angka, semakin renggang (zoom-in). Coba 0.25f atau 0.3f")]
    public float lebarPandangan = 0.25f; 
    
    [Tooltip("Geser angka ini (0 sampai 1) jika Utara di kompas tidak pas dengan Utara di dunia 3D")]
    public float kalibrasiOffset = 0f;

    void Update()
    {
        if (mataPlayer != null && pitaKompas != null)
        {
            // 1. Ambil arah rotasi kepala Player (sumbu Y)
            float rotasiY = mataPlayer.eulerAngles.y;

            // 2. Ubah skala rotasi 360 derajat menjadi desimal 0 sampai 1
            float rasioRotasi = (rotasiY / 360f) + kalibrasiOffset;

            // 3. Gulung tekstur tanpa menggeser objek UI-nya
            // uvRect = (Posisi X, Posisi Y, Lebar/Zoom, Tinggi)
            pitaKompas.uvRect = new Rect(rasioRotasi, 0f, lebarPandangan, 1f);
        }
    }
}
