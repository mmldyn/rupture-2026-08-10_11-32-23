using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class SaklarVR : MonoBehaviour
{
    [Header("Pengaturan Debug")]
    [Tooltip("Centang ini jika ingin memaksa main di PC meskipun SteamVR menyala")]
    public bool paksaModePC = false;

    [Header("Referensi Karakter")]
    public GameObject tubuhVR;     // Masukkan XR Origin (XR Rig) ke sini
    public GameObject tubuhPC;     // Masukkan Player_Dummy_Rig ke sini

    [Header("Referensi Antarmuka (UI)")]
    public GameObject canvasVR;    // UI khusus VR
    public GameObject canvasPC;    // UI khusus PC (UI_Manager_PC)

    void Start()
    {
        // Jalankan timer untuk menunggu sistem VR siap
        StartCoroutine(CekHeadsetVR());
    }

    private IEnumerator CekHeadsetVR()
    {
        if (paksaModePC)
        {
            AktifkanModePC();
            yield break;
        }

        float timeout = 5f; // beri waktu lebih lama untuk ALVR/SteamVR handshake
        float elapsed = 0f;
        List<InputDevice> perangkatVR = new List<InputDevice>();

        while (elapsed < timeout)
        {
            InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.HeadMounted, perangkatVR);
            if (perangkatVR.Count > 0)
            {
                Debug.Log("<color=green>[Saklar VR]</color> Quest Terdeteksi! Masuk Mode VR.");
                AktifkanModeVR();
                yield break;
            }
            elapsed += 0.5f;
            yield return new WaitForSeconds(0.5f);
        }

        Debug.Log("<color=yellow>[Saklar VR]</color> Headset tidak terdeteksi setelah timeout. Masuk Mode PC.");
        AktifkanModePC();
    }

    private void AktifkanModeVR()
    {
        // Matikan PC dulu
        if (tubuhPC != null) tubuhPC.SetActive(false);
        if (canvasPC != null) canvasPC.SetActive(false);

        // Nyalakan VR
        if (tubuhVR != null) tubuhVR.SetActive(true);
        if (canvasVR != null) canvasVR.SetActive(true);
    }

    private void AktifkanModePC()
    {
        // Matikan VR dulu
        if (tubuhVR != null) tubuhVR.SetActive(false);
        if (canvasVR != null) canvasVR.SetActive(false);

        // Nyalakan PC
        if (tubuhPC != null) tubuhPC.SetActive(true);
        if (canvasPC != null) canvasPC.SetActive(true);
    }
}