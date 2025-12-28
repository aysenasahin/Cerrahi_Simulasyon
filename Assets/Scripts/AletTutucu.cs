using UnityEngine;

public class AletTutucu : MonoBehaviour
{
    [Header("Ayarlar")]
    public Transform elNoktasi;
    public LayerMask aletKatmani;

    // MÜHENDİSLİK STANDARDI: Erişim mesafesi (~2 metre)
    public float mesafe = 2.0f;

    [Header("Görsel Efektler")]
    public Material parlamaMateryali; // SARI (Hover/Üzerine gelme)
    public Material secimMateryali;   // MOR (Seçildi)

    // --- DEĞİŞKENLER ---
    private GameObject eldekiNesne;
    private GameObject seciliNesne;
    private GameObject suanBakilanNesne; // Sadece üzerine geldiğimiz (Henüz tıklamadık)

    private Vector3 aletinEskiKonumu;
    private Quaternion aletinEskiRotasyonu;

    private Material nesneninOrijinalMateryali;
    private Material bakilaninOrijinalMateryali; // Sarı yanarken orijinalini saklamak için

    private Camera anaKamera;

    void Start()
    {
        anaKamera = Camera.main;
    }

    void Update()
    {
        // 1. SÜREKLİ TARA (Sarı Parlama İçin)
        NesneTespitiVeHover();

        // 2. TIKLAMA İŞLEMİ
        if (Input.GetMouseButtonDown(0))
        {
            TiklamaIslemleri();
        }
    }

    Ray MouseIşını()
    {
        return anaKamera.ScreenPointToRay(Input.mousePosition);
    }

    // --- SARI PARLAMA (HOVER) SİSTEMİ ---
    void NesneTespitiVeHover()
    {
        // Elim doluyken veya bir şey seçiliyken sarı ışık yakma, kafa karışmasın
        if (eldekiNesne != null)
        {
            SariParlamayiSondur();
            return;
        }

        RaycastHit hit;
        if (Physics.Raycast(MouseIşını(), out hit, mesafe, aletKatmani))
        {
            GameObject tesbitEdilen = hit.transform.gameObject;

            // Eğer zaten MOR yanan (Seçili) nesneye bakıyorsak SARI yakma
            if (tesbitEdilen == seciliNesne)
            {
                SariParlamayiSondur();
                return;
            }

            // Yeni bir nesneye baktık
            if (tesbitEdilen != suanBakilanNesne)
            {
                SariParlamayiSondur(); // Önceki sarıyı söndür

                suanBakilanNesne = tesbitEdilen;

                Renderer ren = suanBakilanNesne.GetComponent<Renderer>();
                if (ren != null)
                {
                    bakilaninOrijinalMateryali = ren.material;
                    if (parlamaMateryali != null) ren.material = parlamaMateryali; // SARI YAP
                }
            }
        }
        else
        {
            // Boşluğa bakıyorsak söndür
            SariParlamayiSondur();
        }
    }

    void SariParlamayiSondur()
    {
        if (suanBakilanNesne != null)
        {
            Renderer ren = suanBakilanNesne.GetComponent<Renderer>();
            // Eğer nesne şu an MOR değilse rengini eski haline getir
            if (ren != null && bakilaninOrijinalMateryali != null && suanBakilanNesne != seciliNesne)
            {
                ren.material = bakilaninOrijinalMateryali;
            }
            suanBakilanNesne = null;
        }
    }

    // --- TIKLAMA VE SEÇME SİSTEMİ ---
    void TiklamaIslemleri()
    {
        RaycastHit hit;
        if (Physics.Raycast(MouseIşını(), out hit, mesafe, aletKatmani))
        {
            GameObject vurulanNesne = hit.transform.gameObject;

            // SENARYO 1: Elimde zaten bir alet var -> DEĞİŞTİR
            if (eldekiNesne != null)
            {
                MasayaGeriBirak(); // Elimdekini yerine koy (Düşmeden)

                // Tıkladığım yeni aleti direkt seç veya al
                if (vurulanNesne != eldekiNesne)
                {
                    NesneyiSec(vurulanNesne); // Yeni aleti MOR yak
                }
                return;
            }

            // SENARYO 2: Elim boş
            if (vurulanNesne == seciliNesne)
            {
                // Zaten MOR olana tıkladım -> Eline AL
                ElineAl();
            }
            else
            {
                // Yeni bir şeye tıkladım -> Sadece SEÇ (Mor Yak)
                SecimiIptalEt();
                NesneyiSec(vurulanNesne);
            }
        }
        else
        {
            // Boşluğa tıkladım
            if (eldekiNesne != null) MasayaGeriBirak(); // Yerine koy
            else SecimiIptalEt(); // Seçimi kaldır
        }
    }

    void NesneyiSec(GameObject nesne)
    {
        // Sarı parlamayı iptal et ki renkler karışmasın
        SariParlamayiSondur();

        seciliNesne = nesne;
        Renderer ren = seciliNesne.GetComponent<Renderer>();
        if (ren != null)
        {
            // Eğer orijinal materyali daha önce kaydetmediysek şimdi kaydet
            // (Sarı yanarken kaydettiysek o sarı kalmış olabilir, dikkat)
            if (nesneninOrijinalMateryali == null)
                nesneninOrijinalMateryali = bakilaninOrijinalMateryali; // Hover'dan geleni al

            // Eğer hover hiç çalışmadıysa direkt al
            if (nesneninOrijinalMateryali == null)
                nesneninOrijinalMateryali = ren.material;

            if (secimMateryali != null) ren.material = secimMateryali; // MOR YAP
        }
    }

    void ElineAl()
    {
        // Mor rengi düzelt (Orijinal haline dön)
        if (seciliNesne != null && nesneninOrijinalMateryali != null)
        {
            seciliNesne.GetComponent<Renderer>().material = nesneninOrijinalMateryali;
        }

        eldekiNesne = seciliNesne;
        seciliNesne = null;
        nesneninOrijinalMateryali = null; // Sıfırla

        // Konumunu kaydet
        aletinEskiKonumu = eldekiNesne.transform.position;
        aletinEskiRotasyonu = eldekiNesne.transform.rotation;

        // Fiziği Kapat
        Rigidbody rb = eldekiNesne.GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = true;
        Collider col = eldekiNesne.GetComponent<Collider>();
        if (col != null) col.enabled = false;

        // Ele Yapıştır
        eldekiNesne.transform.position = elNoktasi.position;
        eldekiNesne.transform.rotation = elNoktasi.rotation;
        eldekiNesne.transform.SetParent(elNoktasi);
    }

    void MasayaGeriBirak()
    {
        if (eldekiNesne == null) return;

        eldekiNesne.transform.SetParent(null);

        // Eski yerine IŞINLA
        eldekiNesne.transform.position = aletinEskiKonumu;
        eldekiNesne.transform.rotation = aletinEskiRotasyonu;

        // --- DÜZELTME: FİZİĞİ AÇMA! (Düşmesin) ---
        Rigidbody rb = eldekiNesne.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true; // BURASI ÖNEMLİ: True kalsın ki masaya çivilensin
            rb.linearVelocity = Vector3.zero;
        }

        // Ama çarpışmayı aç ki tekrar tıklayabilelim
        Collider col = eldekiNesne.GetComponent<Collider>();
        if (col != null) col.enabled = true;

        eldekiNesne = null;
    }

    void SecimiIptalEt()
    {
        if (seciliNesne != null)
        {
            Renderer ren = seciliNesne.GetComponent<Renderer>();
            if (ren != null && nesneninOrijinalMateryali != null)
            {
                ren.material = nesneninOrijinalMateryali;
            }
            seciliNesne = null;
            nesneninOrijinalMateryali = null;
        }
    }
}