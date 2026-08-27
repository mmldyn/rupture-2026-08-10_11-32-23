using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RealEarthquakeData
{
    public string eventName = "Simulasi Sesar Lembang"; 
    public float duration = 15f;
    [Tooltip("Input nilai Skala Richter asli dari lapangan")]
    public float richterScale = 5.0f; 
    [HideInInspector] 
    public float unityMagnitude; 
    public float fadeInTime = 2f;
    public float fadeOutTime = 3f;
}

public class EarthquakeSimulator : MonoBehaviour
{

    [Header("Efek Partikel Lingkungan")]
    public ParticleSystem[] kumpulanEfekDebu;

    [Header("Referensi GPS Evakuasi")]
    public GameObject gpsLineObject; // <--- TAMBAHKAN INI

    [Header("Database Gempa Dunia Nyata")]
    public List<RealEarthquakeData> earthquakeDatabase;

    [Header("Referensi UI & Audio")]
    public PanicUIManager panicUI;
    public StatusGempaHUD statusHUD; // Referensi HUD Status Gempa
    [Tooltip("Masukkan komponen AudioSource untuk suara gemuruh gempa")]
    public AudioSource earthquakeAudioSource; 
    [Tooltip("Volume maksimal suara saat gempa mencapai puncak (0.0 - 1.0)")]
    public float maxAudioVolume = 1f;         

    [Header("Variasi Dinamis")]
    public bool addRandomVariance = true;
    public float durationVariance = 2.0f;
    public float richterVariance = 0.2f;

    [Header("Pengaturan Fisika Objek")]
    public float objectShakeForce = 30f;

    [Header("Referensi Objek")]
    public Transform cameraOffset; 

    private Vector3 originalLocalPos;
    public bool isQuaking = false;

    void Start()
    {
        if (cameraOffset != null) originalLocalPos = cameraOffset.localPosition;

        if (earthquakeDatabase.Count == 0)
        {
            earthquakeDatabase.Add(new RealEarthquakeData());
        }

        // Pastikan audio tidak menyala otomatis di awal
        if (earthquakeAudioSource != null)
        {
            earthquakeAudioSource.volume = 0f;
            earthquakeAudioSource.loop = true; // Agar suara gemuruhnya berulang jika gempanya panjang
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R) && !isQuaking)
        {
            int randomIndex = Random.Range(0, earthquakeDatabase.Count);
            RealEarthquakeData selectedBaseData = earthquakeDatabase[randomIndex];
            
            RealEarthquakeData finalDataToPlay = new RealEarthquakeData
            {
                eventName = selectedBaseData.eventName,
                duration = selectedBaseData.duration,
                richterScale = selectedBaseData.richterScale, 
                fadeInTime = selectedBaseData.fadeInTime,
                fadeOutTime = selectedBaseData.fadeOutTime
            };

            if (addRandomVariance)
            {
                finalDataToPlay.duration += Random.Range(-durationVariance, durationVariance);
                float minDuration = finalDataToPlay.fadeInTime + finalDataToPlay.fadeOutTime + 1f;
                finalDataToPlay.duration = Mathf.Max(finalDataToPlay.duration, minDuration);
                finalDataToPlay.richterScale += Random.Range(-richterVariance, richterVariance);
            }

            finalDataToPlay.unityMagnitude = ConvertRichterToUnity(finalDataToPlay.richterScale);

            StartCoroutine(SimulateEarthquake(finalDataToPlay));
        }
    }

    private float ConvertRichterToUnity(float sr)
    {
        float converted = (sr - 3f) * 0.075f;
        return Mathf.Clamp(converted, 0.02f, 0.6f);
    }

    IEnumerator SimulateEarthquake(RealEarthquakeData activeData)
    {
        isQuaking = true;
        float elapsed = 0.0f;
        
        Debug.Log($"<color=red>[Sistem Bencana]</color> Simulasi Dimulai: {activeData.richterScale:F1} SR");

        if (panicUI != null) panicUI.MulaiEfekPanik();

        // --- TAMPILKAN STATUS HUD GEMPA ---
        if (statusHUD != null) statusHUD.TampilkanStatus(activeData.richterScale);

        // Menyalakan semua debu yang terdaftar
        foreach (ParticleSystem debu in kumpulanEfekDebu)
        {
            if (debu != null) debu.Play();
        }

        // --- MULAI AUDIO GEMPA ---
        if (earthquakeAudioSource != null)
        {
            earthquakeAudioSource.volume = 0f;
            earthquakeAudioSource.Play();
        }

        Rigidbody[] allPhysicalObjects = FindObjectsOfType<Rigidbody>();

        while (elapsed < activeData.duration)
        {
            float currentMagnitude = activeData.unityMagnitude;
            float audioLerpProgress = 1f; // Rasio untuk volume audio (0 - 1)

            // Kalkulasi Transisi Getaran & Audio
            if (elapsed < activeData.fadeInTime)
            {
                float t = elapsed / activeData.fadeInTime;
                currentMagnitude = Mathf.Lerp(0f, activeData.unityMagnitude, t);
                audioLerpProgress = t; // Suara makin keras
            }
            else if (elapsed > (activeData.duration - activeData.fadeOutTime))
            {
                float fadeOutElapsed = elapsed - (activeData.duration - activeData.fadeOutTime);
                float t = fadeOutElapsed / activeData.fadeOutTime;
                currentMagnitude = Mathf.Lerp(activeData.unityMagnitude, 0f, t);
                audioLerpProgress = 1f - t; // Suara makin pelan mereda
            }

            // Atur Volume Audio sesuai kalkulasi di atas
            if (earthquakeAudioSource != null)
            {
                earthquakeAudioSource.volume = audioLerpProgress * maxAudioVolume;
            }

            // Getarkan Kamera
            float x = originalLocalPos.x + Random.Range(-1f, 1f) * currentMagnitude;
            float y = originalLocalPos.y + Random.Range(-1f, 1f) * currentMagnitude;
            cameraOffset.localPosition = new Vector3(x, y, originalLocalPos.z);

            // Getarkan Objek Fisik
            foreach (Rigidbody rb in allPhysicalObjects)
            {
                if (rb != null && rb.gameObject.name != "Player_Dummy_Rig" && !rb.isKinematic)
                {
                    if (rb.IsSleeping()) rb.WakeUp();
                    Vector3 randomJolt = new Vector3(
                        Random.Range(-1f, 1f), Random.Range(-0.5f, 0.5f), Random.Range(-1f, 1f)
                    );
                    rb.AddForce(randomJolt * currentMagnitude * objectShakeForce * rb.mass, ForceMode.Force);
                }
            }
            elapsed += Time.deltaTime;

            // --- HITUNG MUNDUR (COUNTDOWN) ---
            float sisaWaktu = activeData.duration - elapsed;
            if (statusHUD != null) statusHUD.UpdateWaktuCountdown(sisaWaktu);

            yield return null;
        }

        cameraOffset.localPosition = originalLocalPos;
        isQuaking = false;

        // --- MATIKAN AUDIO GEMPA ---
        if (earthquakeAudioSource != null)
        {
            earthquakeAudioSource.Stop();
            earthquakeAudioSource.volume = 0f;
        }
        
        Debug.Log($"<color=green>[Sistem Bencana]</color> Simulasi selesai.");

        // --- SEMBUNYIKAN STATUS HUD GEMPA ---
        if (statusHUD != null) statusHUD.ResetStatus();

        if (panicUI != null) panicUI.HentikanEfekPanik();

        if (gpsLineObject != null) gpsLineObject.SetActive(true);

        // Mematikan semua debu secara perlahan
        foreach (ParticleSystem debu in kumpulanEfekDebu)
        {
            if (debu != null) debu.Stop();
        }

        // --- SAAT GEMPA SELESAI ---
        if (gpsLineObject != null) 
        {
            gpsLineObject.SetActive(true); // <--- MENYALAKAN GARIS GPS
        }
    }
}