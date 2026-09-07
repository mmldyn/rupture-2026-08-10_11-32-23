using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PanicUIManager : MonoBehaviour
{
    [Header("Bahan Baku Pop-Up Gempa (Prefabs)")]
    public GameObject mainWarningPrefab;       
    public GameObject[] minorWarningPrefabs;   

    [Header("Referensi HUD Evakuasi (Pojok Kiri Atas)")]
    [Tooltip("Masukkan objek HudEvakuasi Anda ke sini")]
    public GameObject hudEvakuasi;

    [Header("Pengaturan Wadah")]
    public Transform popupContainer;          

    [Header("Pengaturan Animasi")]
    public float durasiAnimasi = 0.25f; 

    [Header("Pengaturan Sebaran Pop-Up")]
    public Vector2 batasSebaranX = new Vector2(-550f, 550f);
    public Vector2 batasSebaranY = new Vector2(-250f, 250f);
    public Vector2 areaTengahKosong = new Vector2(300f, 100f);
    public Vector2 batasZLayer = new Vector2(-0.1f, 0.1f);

    [Header("Anti-Tabrakan Antar Pop-Up")]
    [Tooltip("Jarak minimum antar pusat popup (dalam unit RectTransform lokal) supaya tidak saling menimpa")]
    public float jarakMinimumAntarPopup = 220f;
    [Tooltip("Berapa kali sistem coba cari posisi baru sebelum menyerah dan pakai posisi terakhir apa adanya")]
    public int maksimalPercobaanPosisi = 15;

    private System.Collections.Generic.List<Vector2> posisiPopupAktif = new System.Collections.Generic.List<Vector2>();

    [Header("Kelengkungan Panel (posisi & rotasi, BUKAN mesh)")]
    [Tooltip("Centang supaya seluruh popup disusun melengkung mengikuti busur lingkaran. Tiap elemen tetap flat/tidak terdistorsi.")]
    public bool aktifkanLengkungan = true;
    [Tooltip("Radius busur lengkungan. Semakin kecil, semakin melengkung tajam.")]
    public float radiusLengkungan = 900f;

    /// <summary>
    /// Konversi posisi flat (seolah panel masih datar) menjadi posisi + rotasi
    /// yang seolah-olah menempel di permukaan silinder. Elemen itu sendiri
    /// tetap berupa rectangle datar biasa, tidak ada distorsi mesh sama sekali.
    /// </summary>
    private void TerapkanPosisiMelengkung(Transform elemen, float flatX, float flatY, float flatZ)
    {
        if (!aktifkanLengkungan || radiusLengkungan <= 0f)
        {
            elemen.localPosition = new Vector3(flatX, flatY, flatZ);
            return;
        }

        float sudutRadian = flatX / radiusLengkungan;
        float x = radiusLengkungan * Mathf.Sin(sudutRadian);
        float z = radiusLengkungan * (1f - Mathf.Cos(sudutRadian)) + flatZ;
        float sudutDerajat = sudutRadian * Mathf.Rad2Deg;

        elemen.localPosition = new Vector3(x, flatY, z);
        // Gabungkan rotasi mengikuti busur (Y) dengan rotasi tilt acak (Z) yang sudah di-set sebelumnya
        Vector3 rotasiSekarang = elemen.localEulerAngles;
        elemen.localRotation = Quaternion.Euler(rotasiSekarang.x, sudutDerajat, rotasiSekarang.z);
    }

    [Header("UI Peringatan Awal (P-Wave)")]
    [Tooltip("Masukkan Canvas/Grup UI Peringatan Awal (dengan logo segitiga) ke sini")]
    public GameObject earlyWarningUI;

    private Coroutine panikCoroutine;

    void Start()
    {
        foreach (Transform child in popupContainer)
        {
            if (child.gameObject != hudEvakuasi) 
                Destroy(child.gameObject);
        }

        if (earlyWarningUI != null)
        {
            earlyWarningUI.SetActive(false);
        }

        if (hudEvakuasi != null)
        {
            CanvasGroup hudCg = hudEvakuasi.GetComponent<CanvasGroup>();
            if (hudCg == null) hudCg = hudEvakuasi.AddComponent<CanvasGroup>();
            
            hudCg.alpha = 0f;
            hudEvakuasi.transform.localScale = Vector3.zero;
            hudEvakuasi.SetActive(false);
        }
    }

    public void MulaiPeringatanAwal()
    {
        if (earlyWarningUI != null)
        {
            earlyWarningUI.SetActive(true);
            earlyWarningUI.transform.localScale = Vector3.zero;
            
            CanvasGroup cg = earlyWarningUI.GetComponent<CanvasGroup>();
            if (cg == null) cg = earlyWarningUI.AddComponent<CanvasGroup>();
            cg.alpha = 0f;

            StartCoroutine(AnimasiMasuk(earlyWarningUI, 1f)); 
        }
        Debug.Log("<color=yellow>[UI]</color> Peringatan Dini / Tugas Ambil Tas Muncul!");
    }

    // --- DIPANGGIL SAAT GEMPA MULAI (PUNCAK) ---
    public void MulaiEfekPanik()
    {
        // Sembunyikan peringatan awal karena guncangan puncak sudah mulai
        if (earlyWarningUI != null) earlyWarningUI.SetActive(false);
    
        if (hudEvakuasi != null && hudEvakuasi.activeSelf)
        {
            StartCoroutine(AnimasiKeluarHUD());
        }

        posisiPopupAktif.Clear();

        if (panikCoroutine != null) StopCoroutine(panikCoroutine);
        panikCoroutine = StartCoroutine(MunculkanPopupSesuaiReferensi());
    }

    // --- DIPANGGIL SAAT GEMPA SELESAI ---
    public void HentikanEfekPanik()
    {
        if (earlyWarningUI != null) earlyWarningUI.SetActive(false);
        if (panikCoroutine != null) StopCoroutine(panikCoroutine);
        
        StartCoroutine(AnimasiKeluarSemuaPopUp());

        if (hudEvakuasi != null)
        {
            hudEvakuasi.SetActive(true);
            StartCoroutine(AnimasiMasukHUD());
        }
    }

    private Vector2 CariPosisiBebasBentrok()
    {
        Vector2 posisi = Vector2.zero;

        for (int percobaan = 0; percobaan < maksimalPercobaanPosisi; percobaan++)
        {
            float xPos = Random.Range(batasSebaranX.x, batasSebaranX.y);
            float yPos = Random.Range(batasSebaranY.x, batasSebaranY.y);

            // Hindari area tengah (supaya tidak menutupi fokus utama pemain)
            if (xPos > -areaTengahKosong.x && xPos < areaTengahKosong.x &&
                yPos > -areaTengahKosong.y && yPos < areaTengahKosong.y)
            {
                yPos = (Random.value > 0.5f)
                    ? Random.Range(areaTengahKosong.y, batasSebaranY.y)
                    : Random.Range(batasSebaranY.x, -areaTengahKosong.y);
            }

            posisi = new Vector2(xPos, yPos);

            // Cek jarak terhadap semua popup yang sudah aktif
            bool bentrok = false;
            foreach (Vector2 posisiLain in posisiPopupAktif)
            {
                if (Vector2.Distance(posisi, posisiLain) < jarakMinimumAntarPopup)
                {
                    bentrok = true;
                    break;
                }
            }

            if (!bentrok)
            {
                // Posisi aman ditemukan, langsung pakai
                break;
            }
            // Kalau bentrok, loop akan coba posisi acak baru lagi (sampai batas percobaan)
        }

        posisiPopupAktif.Add(posisi);
        return posisi;
    }

    private IEnumerator MunculkanPopupSesuaiReferensi()
    {
        for (int i = 0; i < 8; i++)
        {
            GameObject prefabAcak = minorWarningPrefabs[Random.Range(0, minorWarningPrefabs.Length)];
            GameObject spawnedUI = Instantiate(prefabAcak, popupContainer);

            spawnedUI.transform.localScale = Vector3.zero;
            CanvasGroup cg = spawnedUI.GetComponent<CanvasGroup>();
            if (cg == null) cg = spawnedUI.AddComponent<CanvasGroup>();
            cg.alpha = 0f;

            Vector2 posisiAman = CariPosisiBebasBentrok();

            // Set rotasi tilt acak dulu (sumbu Z), baru TerapkanPosisiMelengkung akan
            // menggabungkannya dengan rotasi busur (sumbu Y) tanpa saling menimpa
            spawnedUI.transform.localRotation = Quaternion.Euler(0, 0, Random.Range(-15f, 15f));
            TerapkanPosisiMelengkung(spawnedUI.transform, posisiAman.x, posisiAman.y, Random.Range(batasZLayer.x, batasZLayer.y));
            
            StartCoroutine(AnimasiMasuk(spawnedUI, Random.Range(0.6f, 1.1f)));
            yield return new WaitForSeconds(0.15f);
        }

        yield return new WaitForSeconds(0.3f);
        
        GameObject mainPopup = Instantiate(mainWarningPrefab, popupContainer);
        mainPopup.transform.localScale = Vector3.zero;
        CanvasGroup mainCg = mainPopup.GetComponent<CanvasGroup>();
        if (mainCg == null) mainCg = mainPopup.AddComponent<CanvasGroup>();
        mainCg.alpha = 0f;

        TerapkanPosisiMelengkung(mainPopup.transform, 0f, 0f, -0.15f);
        StartCoroutine(AnimasiMasuk(mainPopup, 1.5f));
    }

    private IEnumerator AnimasiMasukHUD()
    {
        CanvasGroup cg = hudEvakuasi.GetComponent<CanvasGroup>();
        Vector3 awalScale = Vector3.zero;
        Vector3 akhirScale = Vector3.one; 
        
        float waktu = 0f;
        while (waktu < 1f)
        {
            waktu += Time.deltaTime / durasiAnimasi;
            float t = Mathf.SmoothStep(0f, 1f, waktu); 

            hudEvakuasi.transform.localScale = Vector3.Lerp(awalScale, akhirScale, t);
            cg.alpha = Mathf.Lerp(0f, 1f, t); 
            
            yield return null; 
        }
        
        hudEvakuasi.transform.localScale = akhirScale;
        cg.alpha = 1f;
    }

    private IEnumerator AnimasiKeluarHUD()
    {
        CanvasGroup cg = hudEvakuasi.GetComponent<CanvasGroup>();
        float waktu = 0f;
        while (waktu < 1f)
        {
            waktu += Time.deltaTime / durasiAnimasi;
            float t = Mathf.SmoothStep(0f, 1f, waktu);

            cg.alpha = Mathf.Lerp(1f, 0f, t); 
            hudEvakuasi.transform.localScale = Vector3.Lerp(hudEvakuasi.transform.localScale, Vector3.zero, t);
            yield return null;
        }
        hudEvakuasi.SetActive(false);
    }

    private IEnumerator AnimasiMasuk(GameObject popup, float targetScale)
    {
        CanvasGroup cg = popup.GetComponent<CanvasGroup>();
        float waktu = 0f;
        while (waktu < 1f)
        {
            waktu += Time.deltaTime / durasiAnimasi;
            float t = Mathf.SmoothStep(0f, 1f, waktu); 
            popup.transform.localScale = Vector3.Lerp(Vector3.zero, new Vector3(targetScale, targetScale, targetScale), t);
            cg.alpha = Mathf.Lerp(0f, 1f, t); 
            yield return null; 
        }
        popup.transform.localScale = new Vector3(targetScale, targetScale, targetScale);
        cg.alpha = 1f;
    }

    private IEnumerator AnimasiKeluarSemuaPopUp()
    {
        Transform[] semuaPopup = new Transform[popupContainer.childCount];
        for (int i = 0; i < popupContainer.childCount; i++)
        {
            semuaPopup[i] = popupContainer.GetChild(i);
        }

        float waktu = 0f;
        while (waktu < 1f)
        {
            waktu += Time.deltaTime / durasiAnimasi;
            float t = Mathf.SmoothStep(0f, 1f, waktu);

            foreach (Transform popup in semuaPopup)
            {
                if (popup != null && popup.gameObject != hudEvakuasi) 
                {
                    CanvasGroup cg = popup.GetComponent<CanvasGroup>();
                    if (cg != null) cg.alpha = Mathf.Lerp(1f, 0f, t); 
                    popup.localScale = Vector3.Lerp(popup.localScale, Vector3.zero, t);
                }
            }
            yield return null;
        }

        foreach (Transform popup in semuaPopup)
        {
            if (popup != null && popup.gameObject != hudEvakuasi) 
            {
                Destroy(popup.gameObject);
            }
        }

        posisiPopupAktif.Clear();
    }
}