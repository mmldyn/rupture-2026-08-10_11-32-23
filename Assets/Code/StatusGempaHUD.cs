using UnityEngine;
using TMPro;

public class StatusGempaHUD : MonoBehaviour
{
    [Header("Referensi UI")]
    public GameObject panelStatusGempa;
    public TextMeshProUGUI teksRichter;
    public TextMeshProUGUI teksWaktu;

    void Start()
    {
        if (panelStatusGempa != null)
        {
            panelStatusGempa.SetActive(true);
        }

        ResetStatus();
    }

    public void TampilkanStatus(float skalaRichter)
    {
        string angkaSR = skalaRichter.ToString("F1").Replace('.', ',');
        if (teksRichter != null) teksRichter.text = angkaSR + "<size=50%> SR</size>";
    }

    // Menerima sisa waktu (hitung mundur)
    public void UpdateWaktuCountdown(float sisaWaktu)
    {
        if (teksWaktu != null)
        {
            // Menggunakan CeilToInt agar sisa 0.8 detik tetap terbaca 1 detik
            int detik = Mathf.CeilToInt(Mathf.Max(0f, sisaWaktu));
            teksWaktu.text = detik.ToString() + "<size=40%>\nDETIK</size>";
        }
    }

    public void ResetStatus()
    {
        if (teksRichter != null) teksRichter.text = "0,0<size=50%> SR</size>";
        if (teksWaktu != null) teksWaktu.text = "0<size=40%>\nDETIK</size>";
    }
}