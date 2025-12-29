using System.Collections;
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

    [Header("Eline Alma Animasyonu")]
    [Tooltip("Aletin ele süzülme süresi (saniye).")]
    public float eleGelmeSuresi = 1.0f;

    [Tooltip("Ele gelirken dönüşün ne kadar takip edeceği (0=hiç dönmez, 1=hedef rotasyona döner).")]
    [Range(0f, 1f)]
    public float eleGelmeRotasyonTakibi = 1.0f;

    [Header("Bırakma / Fırlatma")]
    [Tooltip("Alet bırakılınca bakış yönüne uygulanacak hız (VelocityChange).")]
    public float firlatmaHizi = 3.0f;

    [Tooltip("Eğik atış için yukarı doğru ek hız.")]
    public float yukariHiz = 1.2f;

    [Tooltip("Fırlatma sonrası çarpışmaların stabil olması için opsiyonel ek: Continuous.")]
    public bool continuousCollisionOnThrow = true;

    // --- DEĞİŞKENLER ---
    private GameObject eldekiNesne;
    private GameObject seciliNesne;
    private GameObject suanBakilanNesne; // Sadece üzerine geldiğimiz (Henüz tıklamadık)

    private Material nesneninOrijinalMateryali;
    private Material bakilaninOrijinalMateryali; // Sarı yanarken orijinalini saklamak için

    private Camera anaKamera;

    // Eline süzülme sırasında state
    private bool eleGeliyor = false;
    private Coroutine eleGelmeCoroutine;

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
        // Elim doluyken veya ele gelme animasyonu varken sarı ışık yakma
        if (eldekiNesne != null || eleGeliyor)
        {
            SariParlamayiSondur();
            return;
        }

        RaycastHit hit;
        if (Physics.Raycast(MouseIşını(), out hit, mesafe, aletKatmani))
        {
            GameObject tesbitEdilen = hit.collider.gameObject;

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
        // Ele gelme animasyonu varken tıklamaları kilitle (state bozulmasın)
        if (eleGeliyor) return;

        RaycastHit hit;
        if (Physics.Raycast(MouseIşını(), out hit, mesafe, aletKatmani))
        {
            GameObject vurulanNesne = hit.collider.gameObject;

            // SENARYO 1: Elimde zaten bir alet var -> BIRAK (Artık eski yerine ışınlama yok, fırlatma var)
            if (eldekiNesne != null)
            {
                FirlatBirak();

                // İstersen: bıraktıktan sonra tıkladığın nesneyi seç (mor) yap
                // Hemen tekrar almayı engellemek için bu satırları kapalı tutmak daha stabil olabilir.
                SecimiIptalEt();
                NesneyiSec(vurulanNesne);
                return;
            }

            // SENARYO 2: Elim boş
            if (vurulanNesne == seciliNesne)
            {
                // Zaten MOR olana tıkladım -> Eline AL (Artık süzülerek gelecek)
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
            if (eldekiNesne != null) FirlatBirak();
            else SecimiIptalEt();
        }
    }

    void NesneyiSec(GameObject nesne)
    {
        SariParlamayiSondur();

        seciliNesne = nesne;
        Renderer ren = seciliNesne.GetComponent<Renderer>();
        if (ren != null)
        {
            if (nesneninOrijinalMateryali == null)
                nesneninOrijinalMateryali = bakilaninOrijinalMateryali;

            if (nesneninOrijinalMateryali == null)
                nesneninOrijinalMateryali = ren.material;

            if (secimMateryali != null) ren.material = secimMateryali; // MOR
        }
    }

    void ElineAl()
    {
        if (seciliNesne == null || elNoktasi == null) return;

        // Mor rengi düzelt
        if (nesneninOrijinalMateryali != null)
        {
            Renderer rr = seciliNesne.GetComponent<Renderer>();
            if (rr != null) rr.material = nesneninOrijinalMateryali;
        }

        // State
        eldekiNesne = seciliNesne;
        seciliNesne = null;
        nesneninOrijinalMateryali = null;

        // Fizik: ele gelirken çarpışma/itme olmasın
        Rigidbody rb = eldekiNesne.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        Collider col = eldekiNesne.GetComponent<Collider>();
        if (col != null) col.enabled = false;

        // Eğer daha önce coroutine varsa kapat
        if (eleGelmeCoroutine != null) StopCoroutine(eleGelmeCoroutine);

        // Süzülerek ele getir
        eleGelmeCoroutine = StartCoroutine(AletiEleSudzur(eldekiNesne.transform, elNoktasi, eleGelmeSuresi));
    }

    IEnumerator AletiEleSudzur(Transform alet, Transform hedef, float sure)
    {
        eleGeliyor = true;

        // Parent yok; world uzayında süzülme
        alet.SetParent(null);

        Vector3 basPos = alet.position;
        Quaternion basRot = alet.rotation;

        Vector3 hedefPos = hedef.position;
        Quaternion hedefRot = hedef.rotation;

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / Mathf.Max(0.0001f, sure);

            // Yumuşak geçiş (ease-in-out)
            float s = t * t * (3f - 2f * t);

            alet.position = Vector3.Lerp(basPos, hedefPos, s);

            if (eleGelmeRotasyonTakibi > 0f)
            {
                Quaternion araRot = Quaternion.Slerp(basRot, hedefRot, s);
                alet.rotation = Quaternion.Slerp(basRot, araRot, eleGelmeRotasyonTakibi);
            }

            yield return null;
        }

        // Tam hedefe oturt ve parentla
        alet.position = hedef.position;
        alet.rotation = hedef.rotation;
        alet.SetParent(hedef);

        eleGeliyor = false;
        eleGelmeCoroutine = null;
    }

    void FirlatBirak()
    {
        if (eldekiNesne == null) return;

        // Parent'ı çöz
        eldekiNesne.transform.SetParent(null);

        // Çarpışmayı aç
        Collider col = eldekiNesne.GetComponent<Collider>();
        if (col != null) col.enabled = true;

        // Rigidbody zorunlu: fırlatma için
        Rigidbody rb = eldekiNesne.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;

            // Daha stabil
            if (continuousCollisionOnThrow)
                rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            // Bakış yönü + yukarı bileşeni (eğik atış)
            Vector3 yon = Vector3.zero;
            if (anaKamera != null)
                yon = anaKamera.transform.forward;
            else
                yon = transform.forward;

            // Kısa mesafede “fırlatma”
            Vector3 hiz = (yon.normalized * firlatmaHizi) + (Vector3.up * yukariHiz);

            // Hızı direkt ver (kısa, kontrollü atış için daha tutarlı)
            rb.AddForce(hiz, ForceMode.VelocityChange);
        }
        // Rigidbody yoksa: fırlatamaz, ama en azından bırakır
        // (İstersen burada otomatik Rigidbody ekleyebilirsin; genelde editörden eklemek daha doğru.)

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
