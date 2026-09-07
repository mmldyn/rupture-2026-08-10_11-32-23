using UnityEngine;
using UnityEngine.InputSystem; // Wajib ditambahkan untuk membaca tombol VR (Input System)

/// <summary>
/// Sekarang murni berfungsi sebagai layer INPUT/KEYBIND untuk memicu sistem
/// gempa - baik dari keyboard (testing PC) maupun tombol controller VR.
///
/// Semua tampilan UI (popup peringatan, efek panik, arahan evakuasi) SUDAH
/// otomatis ditangani PanicUIManager lewat pemanggilan langsung dari
/// EarthquakeSimulator sendiri (MulaiPeringatanAwal, MulaiEfekPanik,
/// HentikanEfekPanik) - script ini TIDAK lagi mengurus GameObject popup
/// secara manual seperti versi sebelumnya.
/// </summary>
public class EarthquakeUIManager : MonoBehaviour
{
    [Header("Referensi Sistem Gempa Sungguhan")]
    [Tooltip("Masukkan GameObject yang punya komponen EarthquakeSimulator (misal: GameManager)")]
    public EarthquakeSimulator earthquakeSimulator;

    [Header("Tombol Mulai Gempa")]
    [Tooltip("Keyboard: Angka 1 atau R. VR: assign action tombol di sini (misal XRI RightHand Interaction/Activate)")]
    public InputActionReference tombolMulaiGempaVR;

    [Header("Tombol Hentikan Paksa (opsional - berguna untuk testing/instruktur)")]
    [Tooltip("Keyboard: Angka 2. VR: assign action tombol B di sini. Menghentikan gempa lebih awal secara paksa.")]
    public InputActionReference tombolHentikanPaksaVR;

    void Start()
    {
        if (earthquakeSimulator == null)
        {
            earthquakeSimulator = FindObjectOfType<EarthquakeSimulator>();
            if (earthquakeSimulator == null)
                Debug.LogWarning("[EarthquakeUIManager] EarthquakeSimulator tidak ditemukan di scene! Tombol tidak akan memicu apapun.");
        }
    }

    void Update()
    {
        // --- TOMBOL MULAI GEMPA (Keyboard 1/R, atau tombol VR yang di-assign) ---
        bool vrMulaiDitekan = tombolMulaiGempaVR != null && tombolMulaiGempaVR.action != null
                              && tombolMulaiGempaVR.action.WasPressedThisFrame();

        if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.R) || vrMulaiDitekan)
        {
            if (earthquakeSimulator != null)
            {
                earthquakeSimulator.MulaiGempaAcak();
                Debug.Log("<color=cyan>[Input Gempa]</color> Gempa dipicu lewat tombol.");
            }
        }

        // --- TOMBOL HENTIKAN PAKSA (Keyboard 2, atau tombol VR B yang sudah Anda assign) ---
        bool vrHentikanDitekan = tombolHentikanPaksaVR != null && tombolHentikanPaksaVR.action != null
                                 && tombolHentikanPaksaVR.action.WasPressedThisFrame();

        if (Input.GetKeyDown(KeyCode.Alpha2) || vrHentikanDitekan)
        {
            if (earthquakeSimulator != null)
            {
                earthquakeSimulator.HentikanPaksa();
                Debug.Log("<color=cyan>[Input Gempa]</color> Gempa dihentikan paksa lewat tombol.");
            }
        }
    }
}