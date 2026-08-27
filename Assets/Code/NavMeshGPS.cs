using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

[RequireComponent(typeof(LineRenderer))]
public class NavMeshGPS : MonoBehaviour
{
    [Header("Target Evakuasi")]
    public Transform player;
    public Transform titikKumpul;
    
    [Header("Referensi Sistem")]
    public EarthquakeSimulator earthquakeSimulator;

    [Header("Pengaturan Tampilan")]
    public float tinggiGaris = 0.1f; 
    
    [Tooltip("Jumlah segmen lengkungan. Semakin besar (misal 10-15), garis makin bulat sempurna!")]
    [Range(1, 20)]
    public int tingkatKehalusan = 10; 

    private LineRenderer garisVisual;
    private NavMeshPath jalurKalkulasi;

    void Start()
    {
        garisVisual = GetComponent<LineRenderer>();
        jalurKalkulasi = new NavMeshPath();
        garisVisual.enabled = false; 

        garisVisual.numCornerVertices = 8;
        garisVisual.numCapVertices = 8;
    }

    void Update()
    {
        if (player == null || titikKumpul == null) return;

        if (earthquakeSimulator == null || earthquakeSimulator.isQuaking)
        {
            garisVisual.enabled = false;
            return;
        }

        garisVisual.enabled = true;

        NavMeshHit hitPlayer, hitKumpul;
        bool posisiPlayerValid = NavMesh.SamplePosition(player.position, out hitPlayer, 5.0f, NavMesh.AllAreas);
        bool posisiKumpulValid = NavMesh.SamplePosition(titikKumpul.position, out hitKumpul, 5.0f, NavMesh.AllAreas);

        if (!posisiPlayerValid || !posisiKumpulValid)
        {
            garisVisual.positionCount = 0;
            return;
        }

        NavMesh.CalculatePath(hitPlayer.position, hitKumpul.position, NavMesh.AllAreas, jalurKalkulasi);

        if (jalurKalkulasi.status == NavMeshPathStatus.PathComplete)
        {
            BuatLengkunganSpline();
        }
        else
        {
            garisVisual.positionCount = 0;
        }
    }

    void BuatLengkunganSpline()
    {
        Vector3[] corners = jalurKalkulasi.corners;
        if (corners.Length < 2) return;

        List<Vector3> titikDasar = new List<Vector3>();
        for (int i = 0; i < corners.Length; i++)
        {
            Vector3 t = corners[i];
            t.y += tinggiGaris;
            titikDasar.Add(t);
        }

        List<Vector3> controlPoints = new List<Vector3>();
        controlPoints.Add(titikDasar[0]);
        controlPoints.AddRange(titikDasar);
        controlPoints.Add(titikDasar[titikDasar.Count - 1]);

        List<Vector3> smoothPoints = new List<Vector3>();

        for (int i = 1; i < controlPoints.Count - 2; i++)
        {
            Vector3 p0 = controlPoints[i - 1];
            Vector3 p1 = controlPoints[i];
            Vector3 p2 = controlPoints[i + 1];
            Vector3 p3 = controlPoints[i + 2];

            for (int j = 0; j < tingkatKehalusan; j++)
            {
                float t = j / (float)tingkatKehalusan;
                Vector3 position = KalkulasiCatmullRom(t, p0, p1, p2, p3);

                // PENANGKAL NEMBUS: Cek apakah titik lengkungan keluar jalur
                NavMeshHit hit;
                // Memaksa titik kembali ke atas area biru NavMesh terdekat
                if (NavMesh.SamplePosition(position, out hit, 2.0f, NavMesh.AllAreas))
                {
                    // Ambil koordinat X dan Z dari NavMesh, pertahankan Y (tinggi garis)
                    position = new Vector3(hit.position.x, position.y, hit.position.z);
                }

                smoothPoints.Add(position);
            }
        }
        
        smoothPoints.Add(titikDasar[titikDasar.Count - 1]);

        garisVisual.positionCount = smoothPoints.Count;
        garisVisual.SetPositions(smoothPoints.ToArray());
    }

    Vector3 KalkulasiCatmullRom(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
    {
        Vector3 a = 2f * p1;
        Vector3 b = p2 - p0;
        Vector3 c = 2f * p0 - 5f * p1 + 4f * p2 - p3;
        Vector3 d = -p0 + 3f * p1 - 3f * p2 + p3;
        return 0.5f * (a + (b * t) + (c * t * t) + (d * t * t * t));
    }
}