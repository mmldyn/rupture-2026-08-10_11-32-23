using System.Collections.Generic;
using UnityEngine;

public class ManajerRetakDinamis : MonoBehaviour
{
    [Header("Deteksi Permukaan Otomatis")]
    [Tooltip("Semua GameObject dinding/lantai yang boleh retak harus diberi Tag ini (buat Tag baru lewat Inspector > Tag dropdown > Add Tag)")]
    public string tagPermukaan = "Dinding";

    [Tooltip("Opsional: assign manual Collider di sini kalau tidak mau pakai sistem Tag")]
    public Collider[] permukaanManual;

    [Header("Intensitas Real-Time")]
    [Tooltip("Nilai magnitude maksimum di skala Unity (samakan dengan Clamp di ConvertRichterToUnity EarthquakeSimulator, default 0.6)")]
    public float magnitudeMaksimalReferensi = 0.6f;
    [Tooltip("Berapa retak baru per detik saat guncangan berada di puncak intensitas (1.0)")]
    public float retakPerDetikPadaPuncak = 1.2f;
    [Tooltip("Batas jumlah retak aktif sekaligus demi performa")]
    public int maksimalRetakAktif = 60;

    public Color warnaRetak = new Color(0.05f, 0.05f, 0.05f, 0.95f);

    private List<Collider> daftarPermukaan = new List<Collider>();
    private List<RetakProsedural> retakAktif = new List<RetakProsedural>();
    private float akumulatorWaktu = 0f;

    void Awake()
    {
        if (permukaanManual != null && permukaanManual.Length > 0)
        {
            daftarPermukaan.AddRange(permukaanManual);
        }
        else
        {
            GameObject[] objekBertag = GameObject.FindGameObjectsWithTag(tagPermukaan);
            foreach (GameObject obj in objekBertag)
            {
                Collider col = obj.GetComponent<Collider>();
                if (col != null) daftarPermukaan.Add(col);
            }
        }

        if (daftarPermukaan.Count == 0)
        {
            Debug.LogWarning($"<color=yellow>[ManajerRetakDinamis]</color> Tidak ada permukaan ditemukan dengan Tag '{tagPermukaan}'. Retak dinamis tidak akan muncul sampai ada objek yang diberi Tag ini (atau isi 'Permukaan Manual').");
        }
    }


    public void PerbaruiRetak(float magnitudeSaatIni, float deltaTime)
    {
        if (daftarPermukaan.Count == 0) return;

        float intensitas = Mathf.Clamp01(magnitudeSaatIni / magnitudeMaksimalReferensi);
        if (intensitas <= 0.01f) return;

        akumulatorWaktu += deltaTime * intensitas * retakPerDetikPadaPuncak;

        while (akumulatorWaktu >= 1f)
        {
            akumulatorWaktu -= 1f;
            CobaSpawnRetak(intensitas);
        }
    }

    private void CobaSpawnRetak(float intensitas)
    {
        retakAktif.RemoveAll(r => r == null);
        if (retakAktif.Count >= maksimalRetakAktif) return;

        Collider dinding = daftarPermukaan[Random.Range(0, daftarPermukaan.Count)];

        // Cari titik acak di sekitar permukaan, lalu "tarik" ke titik terdekat DI permukaan itu
        Vector3 titikAcakDiLuar = dinding.bounds.center
            + Random.insideUnitSphere * (dinding.bounds.extents.magnitude + 0.5f);
        Vector3 titikPermukaan = dinding.ClosestPoint(titikAcakDiLuar);
        Vector3 arahKeLuar = titikAcakDiLuar - titikPermukaan;

        if (arahKeLuar.sqrMagnitude < 0.0001f) arahKeLuar = Vector3.up;
        arahKeLuar.Normalize();

        Vector3 normal = arahKeLuar;
        Vector3 originRay = titikPermukaan + arahKeLuar * 0.05f;

        // Raycast pendek untuk dapat normal permukaan yang akurat
        RaycastHit hit;
        if (dinding.Raycast(new Ray(originRay, -arahKeLuar), out hit, 0.2f))
        {
            titikPermukaan = hit.point;
            normal = hit.normal;
        }

        Vector3 arahRambat = Vector3.Cross(normal, Random.insideUnitSphere).normalized;
        if (arahRambat.sqrMagnitude < 0.0001f) arahRambat = Vector3.up;

        RetakProsedural retakBaru = RetakProsedural.Buat(
            titikPermukaan, normal, arahRambat, intensitas, warnaRetak, dinding.transform
        );
        retakAktif.Add(retakBaru);
    }

    public void HapusSemuaRetak()
    {
        foreach (RetakProsedural retak in retakAktif)
        {
            if (retak != null) Destroy(retak.gameObject);
        }
        retakAktif.Clear();
    }
}