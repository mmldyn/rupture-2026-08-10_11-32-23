using UnityEngine;
using UnityEngine.AI;
public class DynamicEvacuationPath : MonoBehaviour
{
    [Header("Referensi Objek")]
    public Transform player;
    [Tooltip("Masukkan semua kemungkinan Titik Kumpul (bisa lebih dari 1)")]
    public Transform[] titikKumpulArray; 
    public LineRenderer pathRenderer;

    private NavMeshPath navPath;

    void Start()
    {
        navPath = new NavMeshPath();
        
        // Memastikan garis tidak muncul sebelum dibutuhkan
        if (pathRenderer != null)
        {
            pathRenderer.positionCount = 0;
        }
    }

    void Update()
    {
        if (player == null || titikKumpulArray.Length == 0 || pathRenderer == null) return;

        // 1. Cari Titik Kumpul Paling Dekat
        Transform targetTerdekat = DapatkanTitikTerdekat();

        // 2. Kalkulasi Jalur Cerdas (Menghindari Bangunan/Tembok)
        NavMesh.CalculatePath(player.position, targetTerdekat.position, NavMesh.AllAreas, navPath);

        // 3. Gambar Garis di Lantai
        if (navPath.corners.Length > 0)
        {
            pathRenderer.positionCount = navPath.corners.Length;
            
            for (int i = 0; i < navPath.corners.Length; i++)
            {
                // Ambil titik belokan dari NavMesh
                Vector3 pointPos = navPath.corners[i];
                
                // Angkat garis sedikit (0.1f) ke atas lantai agar tidak tertutup tanah
                pointPos.y += 0.1f; 
                
                pathRenderer.SetPosition(i, pointPos);
            }
        }
    }

    // Fungsi untuk mencari titik kumpul terdekat dari posisi player saat ini
    Transform DapatkanTitikTerdekat()
    {
        Transform terdekat = titikKumpulArray[0];
        float jarakTerdekat = Vector3.Distance(player.position, terdekat.position);

        foreach (Transform titik in titikKumpulArray)
        {
            float jarak = Vector3.Distance(player.position, titik.position);
            if (jarak < jarakTerdekat)
            {
                jarakTerdekat = jarak;
                terdekat = titik;
            }
        }
        return terdekat;
    }
}