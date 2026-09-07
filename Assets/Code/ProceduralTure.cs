using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RetakProsedural : MonoBehaviour
{
    public static RetakProsedural Buat(Vector3 posisi, Vector3 normal, Vector3 arahRambat,
        float skalaKekuatan, Color warna, Transform permukaanInduk = null)
    {
        GameObject objekBaru = new GameObject("RetakDinamis");
        if (permukaanInduk != null) objekBaru.transform.SetParent(permukaanInduk, true);
        objekBaru.transform.position = posisi;

        RetakProsedural retak = objekBaru.AddComponent<RetakProsedural>();
        retak.MulaiTumbuh(posisi, normal, arahRambat, skalaKekuatan, warna);
        return retak;
    }

    private void MulaiTumbuh(Vector3 posisi, Vector3 normal, Vector3 arahRambat, float skalaKekuatan, Color warna)
    {
        skalaKekuatan = Mathf.Clamp01(skalaKekuatan);

        // Semakin kuat guncangan saat retak ini muncul, semakin panjang & kompleks retaknya
        int jumlahSegmen = Mathf.RoundToInt(Mathf.Lerp(3, 14, skalaKekuatan));
        float panjangTotal = Mathf.Lerp(0.15f, 0.9f, skalaKekuatan);
        float variasiSudut = 40f;
        float lebarGaris = Mathf.Lerp(0.004f, 0.012f, skalaKekuatan);
        float durasiMerambat = Mathf.Lerp(0.4f, 1.4f, skalaKekuatan);

        LineRenderer lr = gameObject.AddComponent<LineRenderer>();
        lr.material = new Material(Shader.Find("Sprites/Default"));
        lr.startColor = warna;
        lr.endColor = warna;
        lr.startWidth = lebarGaris;
        lr.endWidth = lebarGaris;
        lr.useWorldSpace = true;
        lr.numCapVertices = 2;
        lr.positionCount = 0;

        StartCoroutine(Merambat(lr, posisi, normal, arahRambat, jumlahSegmen, panjangTotal, variasiSudut, durasiMerambat));
    }

    private IEnumerator Merambat(LineRenderer lr, Vector3 posisiAwal, Vector3 normal, Vector3 arahRambat,
        int jumlahSegmen, float panjangTotal, float variasiSudut, float durasiTotal)
    {
        List<Vector3> titik = new List<Vector3> { posisiAwal };
        float panjangPerSegmen = panjangTotal / jumlahSegmen;
        float jedaPerSegmen = durasiTotal / jumlahSegmen;
        Vector3 arah = arahRambat;

        for (int i = 0; i < jumlahSegmen; i++)
        {
            float sudutAcak = Random.Range(-variasiSudut, variasiSudut);
            arah = (Quaternion.AngleAxis(sudutAcak, normal) * arah).normalized;

            Vector3 titikBaru = titik[titik.Count - 1] + arah * panjangPerSegmen + normal * 0.004f;
            titik.Add(titikBaru);

            lr.positionCount = titik.Count;
            lr.SetPositions(titik.ToArray());

            yield return new WaitForSeconds(jedaPerSegmen);
        }
    }
}