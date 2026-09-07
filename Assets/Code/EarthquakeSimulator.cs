using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

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
    public GameObject gpsLineObject; 

    [Header("Database Gempa Dunia Nyata")]
    public List<RealEarthquakeData> earthquakeDatabase;

    [Header("Referensi UI & Audio")]
    public PanicUIManager panicUI;
    public StatusGempaHUD statusHUD; 
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

    [Header("Haptic Feedback (VR)")]
    [Tooltip("Kosongkan kalau sedang di mode PC - haptic otomatis diabaikan kalau null")]
    public XRBaseController leftController;
    public XRBaseController rightController;
    [Range(0f, 1f)] public float kekuatanHapticMaksimal = 0.8f;
    [Tooltip("SR referensi minimal - di SR ini haptic terasa paling lemah (tapi tetap terasa)")]
    public float srReferensiMin = 3f;
    [Tooltip("SR referensi maksimal - di SR ini (atau lebih) haptic full kekuatan maksimal")]
    public float srReferensiMaks = 9f;
    [Tooltip("Kekuatan haptic minimum walau SR sangat kecil, supaya tetap terasa ada getaran (0-1)")]
    [Range(0f, 1f)] public float kekuatanHapticMinimal = 0.15f;

    [Header("Retak Dinamis")]
    [Tooltip("Manajer retak untuk permukaan dinding (Tag: Dinding). Kosongkan untuk auto-cari saat gempa dimulai.")]
    public ManajerRetakDinamis manajerRetakDinding;
    [Tooltip("Manajer retak untuk permukaan lantai (Tag: Lantai). Kosongkan untuk auto-cari saat gempa dimulai.")]
    public ManajerRetakDinamis manajerRetakLantai;

    private Vector3 originalLocalPos;
    public bool isQuaking = false;
    private Coroutine gempaCoroutineAktif;

    void Start()
    {
        if (cameraOffset != null) originalLocalPos = cameraOffset.localPosition;

        if (earthquakeDatabase.Count == 0)
        {
            earthquakeDatabase.Add(new RealEarthquakeData());
        }

        if (earthquakeAudioSource != null)
        {
            earthquakeAudioSource.volume = 0f;
            earthquakeAudioSource.loop = true; 
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R) && !isQuaking)
        {
            MulaiGempaAcak();
        }
    }

    /// <summary>
    /// Method publik: pilih data gempa acak dari database, lalu jalankan simulasi.
    /// Bisa dipanggil dari mana saja - keyboard, tombol VR, UI, dsb.
    /// </summary>
    public void MulaiGempaAcak()
    {
        if (isQuaking) return;

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

        gempaCoroutineAktif = StartCoroutine(SimulateEarthquake(finalDataToPlay));
    }

    /// <summary>
    /// Hentikan simulasi gempa yang sedang berjalan lebih awal secara paksa.
    /// Berguna untuk testing/instruktur - tidak dipakai dalam alur normal.
    /// </summary>
    public void HentikanPaksa()
    {
        if (!isQuaking) return;

        if (gempaCoroutineAktif != null)
        {
            StopCoroutine(gempaCoroutineAktif);
            gempaCoroutineAktif = null;
        }

        // Bersihkan semua efek seperti kalau gempa selesai normal
        if (cameraOffset != null) cameraOffset.localPosition = originalLocalPos;
        isQuaking = false;

        if (earthquakeAudioSource != null)
        {
            earthquakeAudioSource.Stop();
            earthquakeAudioSource.volume = 0f;
        }

        PanicUIManager uiAktif = FindObjectOfType<PanicUIManager>();
        StatusGempaHUD statusAktif = FindObjectOfType<StatusGempaHUD>();
        if (statusAktif != null) statusAktif.ResetStatus();
        if (uiAktif != null) uiAktif.HentikanEfekPanik();

        foreach (ParticleSystem debu in kumpulanEfekDebu)
        {
            if (debu != null) debu.Stop();
        }

        Debug.Log("<color=orange>[Sistem Bencana]</color> Simulasi dihentikan paksa.");
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

        PanicUIManager uiAktif = FindObjectOfType<PanicUIManager>();
        StatusGempaHUD statusAktif = FindObjectOfType<StatusGempaHUD>();

        if (uiAktif != null) uiAktif.MulaiPeringatanAwal(); 

        bool isPanicUITriggered = false;

        if (statusAktif != null) statusAktif.TampilkanStatus(activeData.richterScale);

        foreach (ParticleSystem debu in kumpulanEfekDebu)
        {
            if (debu != null) debu.Play();
        }

        if (earthquakeAudioSource != null)
        {
            earthquakeAudioSource.volume = 0f;
            earthquakeAudioSource.Play();
        }

        Rigidbody[] allPhysicalObjects = FindObjectsOfType<Rigidbody>();

        if (manajerRetakDinding == null || manajerRetakLantai == null)
        {
            ManajerRetakDinamis[] semuaManajer = FindObjectsOfType<ManajerRetakDinamis>();
            foreach (ManajerRetakDinamis m in semuaManajer)
            {
                if (manajerRetakDinding == null && m.tagPermukaan == "Dinding") manajerRetakDinding = m;
                if (manajerRetakLantai == null && m.tagPermukaan == "Lantai") manajerRetakLantai = m;
            }
        }

        while (elapsed < activeData.duration)
        {
            float currentMagnitude = activeData.unityMagnitude;
            float audioLerpProgress = 1f; 

            if (elapsed < activeData.fadeInTime)
            {
                float t = elapsed / activeData.fadeInTime;
                currentMagnitude = Mathf.Lerp(0f, activeData.unityMagnitude, t);
                audioLerpProgress = t; 
            }
            else 
            {
                if (!isPanicUITriggered)
                {
                    if (uiAktif != null) uiAktif.MulaiEfekPanik(); 
                    isPanicUITriggered = true;
                    Debug.Log("<color=orange>[Sistem Bencana]</color> Guncangan Puncak! UI Panik Penuh Aktif.");
                }

                if (elapsed > (activeData.duration - activeData.fadeOutTime))
                {
                    float fadeOutElapsed = elapsed - (activeData.duration - activeData.fadeOutTime);
                    float t = fadeOutElapsed / activeData.fadeOutTime;
                    currentMagnitude = Mathf.Lerp(activeData.unityMagnitude, 0f, t);
                    audioLerpProgress = 1f - t; 
                }
            }

            if (earthquakeAudioSource != null)
                earthquakeAudioSource.volume = audioLerpProgress * maxAudioVolume;

            float x = originalLocalPos.x + Random.Range(-1f, 1f) * currentMagnitude;
            float y = originalLocalPos.y + Random.Range(-1f, 1f) * currentMagnitude;
            cameraOffset.localPosition = new Vector3(x, y, originalLocalPos.z);

            // --- HAPTIC FEEDBACK + RETAK DINAMIS (dinding & lantai) sesuai kekuatan guncangan saat ini ---
            KirimHapticGempa(currentMagnitude, activeData.richterScale);
            if (manajerRetakDinding != null) manajerRetakDinding.PerbaruiRetak(currentMagnitude, Time.deltaTime);
            if (manajerRetakLantai != null) manajerRetakLantai.PerbaruiRetak(currentMagnitude, Time.deltaTime);

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
            float sisaWaktu = activeData.duration - elapsed;
            
            if (statusAktif != null) statusAktif.UpdateWaktuCountdown(sisaWaktu);

            yield return null;
        }

        cameraOffset.localPosition = originalLocalPos;
        isQuaking = false;

        if (earthquakeAudioSource != null)
        {
            earthquakeAudioSource.Stop();
            earthquakeAudioSource.volume = 0f;
        }
        
        Debug.Log($"<color=green>[Sistem Bencana]</color> Simulasi selesai.");

        if (statusAktif != null) statusAktif.ResetStatus();
        if (uiAktif != null) uiAktif.HentikanEfekPanik();

        foreach (ParticleSystem debu in kumpulanEfekDebu)
        {
            if (debu != null) debu.Stop();
        }

        if (gpsLineObject != null) gpsLineObject.SetActive(true); 
    }


    private void KirimHapticGempa(float magnitudeSaatIni, float srGempa)
    {
        // Bentuk/pola naik-turun sesuai fase fade-in -> puncak -> fade-out saat ini
        float bentukGuncangan = Mathf.Clamp01(magnitudeSaatIni / 0.6f);

        // Kekuatan dasar eksplisit berdasarkan SR gempa (0 = SR minimal, 1 = SR maksimal referensi)
        float posisiSR = Mathf.InverseLerp(srReferensiMin, srReferensiMaks, srGempa);
        float kekuatanDasar = Mathf.Lerp(kekuatanHapticMinimal, 1f, posisiSR);

        float amplitude = bentukGuncangan * kekuatanDasar * kekuatanHapticMaksimal;
        float durasi = Time.deltaTime;

        if (leftController != null) leftController.SendHapticImpulse(amplitude, durasi);
        if (rightController != null) rightController.SendHapticImpulse(amplitude, durasi);
    }
}