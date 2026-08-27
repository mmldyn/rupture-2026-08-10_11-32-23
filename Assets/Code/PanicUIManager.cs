using UnityEngine;
using System.Collections;
using UnityEngine.UI; // Wajib jika nanti ingin mengubah teks/gambar via script

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

    private Coroutine panikCoroutine;

    void Start()
    {
        // 1. Bersihkan sisa pop-up panik
        foreach (Transform child in popupContainer)
        {
            if (child.gameObject != hudEvakuasi) // Jangan hancurkan HUD Evakuasi!
                Destroy(child.gameObject);
        }

        // 2. Pastikan HUD Evakuasi tersembunyi total di awal game
        if (hudEvakuasi != null)
        {
            CanvasGroup hudCg = hudEvakuasi.GetComponent<CanvasGroup>();
            if (hudCg == null) hudCg = hudEvakuasi.AddComponent<CanvasGroup>();
            
            hudCg.alpha = 0f;
            hudEvakuasi.transform.localScale = Vector3.zero;
            hudEvakuasi.SetActive(false);
        }
    }

    // --- DIPANGGIL SAAT GEMPA MULAI ---
    public void MulaiEfekPanik()
    {
        // Jika HUD sedang aktif (dari gempa sebelumnya), sembunyikan dulu
        if (hudEvakuasi != null && hudEvakuasi.activeSelf)
        {
            StartCoroutine(AnimasiKeluarHUD());
        }

        if (panikCoroutine != null) StopCoroutine(panikCoroutine);
        panikCoroutine = StartCoroutine(MunculkanPopupSesuaiReferensi());
    }

    // --- DIPANGGIL SAAT GEMPA SELESAI ---
    public void HentikanEfekPanik()
    {
        if (panikCoroutine != null) StopCoroutine(panikCoroutine);
        
        // 1. Hilangkan pop-up panik merah
        StartCoroutine(AnimasiKeluarSemuaPopUp());

        // 2. MUNCULKAN HUD EVAKUASI DI KIRI ATAS SECARA MULUS!
        if (hudEvakuasi != null)
        {
            hudEvakuasi.SetActive(true);
            StartCoroutine(AnimasiMasukHUD());
        }
    }

    // --- MESIN ANIMASI POP-UP PANIK ---

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

            float xPos = Random.Range(batasSebaranX.x, batasSebaranX.y);
            float yPos = Random.Range(batasSebaranY.x, batasSebaranY.y);

            if (xPos > -areaTengahKosong.x && xPos < areaTengahKosong.x && 
                yPos > -areaTengahKosong.y && yPos < areaTengahKosong.y)
            {
                yPos = (Random.value > 0.5f) ? Random.Range(areaTengahKosong.y, batasSebaranY.y) : Random.Range(batasSebaranY.x, -areaTengahKosong.y);
            }

            spawnedUI.transform.localPosition = new Vector3(xPos, yPos, Random.Range(batasZLayer.x, batasZLayer.y));
            spawnedUI.transform.localRotation = Quaternion.Euler(0, 0, Random.Range(-15f, 15f));
            
            StartCoroutine(AnimasiMasuk(spawnedUI, Random.Range(0.6f, 1.1f)));
            yield return new WaitForSeconds(0.15f);
        }

        yield return new WaitForSeconds(0.3f);
        
        GameObject mainPopup = Instantiate(mainWarningPrefab, popupContainer);
        mainPopup.transform.localScale = Vector3.zero;
        CanvasGroup mainCg = mainPopup.GetComponent<CanvasGroup>();
        if (mainCg == null) mainCg = mainPopup.AddComponent<CanvasGroup>();
        mainCg.alpha = 0f;

        mainPopup.transform.localPosition = new Vector3(0, 0, -0.15f); 
        StartCoroutine(AnimasiMasuk(mainPopup, 1.5f));
    }

    // --- MESIN ANIMASI HUD EVAKUASI ---

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
        Debug.Log("<color=cyan>[HUD]</color> Evakuasi dimunculkan di kiri atas.");
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
                if (popup != null && popup.gameObject != hudEvakuasi) // Abaikan HUD
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
    }
}