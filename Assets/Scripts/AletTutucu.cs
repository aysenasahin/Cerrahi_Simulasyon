using System.Collections;
using UnityEngine;

public class AletTutucu : MonoBehaviour
{
    [Header("Gerekli Bağlantılar")]
    public Transform elModeli; // Buraya 'El_Noktasi' atanacak
    public LayerMask aletKatmani; 
    public Camera anaKamera;

    [Header("Mesafe Ayarları")]
    public float uzanmaMenzili = 2.0f; // İdeal gerçekçi menzil
    public float almaPayi = 0.05f;
    public float birakmaPayi = 0.2f; 
    public float animasyonSuresi = 0.6f;

    [Header("Görsel Efektler")]
    public Material parlamaMateryali; 
    public Material secimMateryali;   

    [Header("Fırlatma (F Tuşu)")]
    public float firlatmaGucu = 2.5f; 

    public GameObject eldekiNesne; 
    private GameObject suanBakilanNesne;
    private Material orijinalMat;
    private Material bakilanMat;

    private Vector3 elOrijinalLocalPos;
    private Quaternion elOrijinalLocalRot;
    private bool elHareketEdiyor = false; 

    void Start()
    {
        if (anaKamera == null) anaKamera = Camera.main;

        if (elModeli != null)
        {
            elOrijinalLocalPos = elModeli.localPosition;
            elOrijinalLocalRot = elModeli.localRotation;
        }
    }

    void Update()
    {
        if (elHareketEdiyor) return;

        Debug.DrawRay(anaKamera.transform.position, anaKamera.transform.forward * uzanmaMenzili, Color.green);
        HoverIslemi();

        if (Input.GetMouseButtonDown(0)) SolTikIslemleri();
        if (Input.GetKeyDown(KeyCode.F)) if (eldekiNesne != null) Firlat();
    }

    void HoverIslemi()
    {
        if (eldekiNesne != null) { ParlamaSondur(); return; }

        RaycastHit hit;
        if (Physics.Raycast(anaKamera.ScreenPointToRay(Input.mousePosition), out hit, uzanmaMenzili, aletKatmani))
        {
            GameObject obje = hit.collider.gameObject;
            if (obje == suanBakilanNesne) return;
            if (obje.GetComponent<Rigidbody>() == null) return; 

            ParlamaSondur();
            suanBakilanNesne = obje;
            Renderer r = suanBakilanNesne.GetComponent<Renderer>();
            if (r != null)
            {
                bakilanMat = r.material;
                if (parlamaMateryali != null) r.material = parlamaMateryali;
            }
        }
        else ParlamaSondur();
    }

    void ParlamaSondur()
    {
        if (suanBakilanNesne != null)
        {
            Renderer r = suanBakilanNesne.GetComponent<Renderer>();
            if (r != null && bakilanMat != null) r.material = bakilanMat;
            suanBakilanNesne = null;
        }
    }

    void SolTikIslemleri()
    {
        RaycastHit hit;
        if (Physics.Raycast(anaKamera.ScreenPointToRay(Input.mousePosition), out hit, uzanmaMenzili))
        {
            GameObject vurulan = hit.collider.gameObject;

            if (eldekiNesne != null)
            {
                StartCoroutine(ElUzanipBiraksin(hit.point));
                return;
            }

            if (vurulan.GetComponent<Rigidbody>() != null)
            {
                StartCoroutine(ElUzanipAlsin(vurulan));
            }
        }
    }

    // --- ALMA İŞLEMİ ---
    IEnumerator ElUzanipAlsin(GameObject hedef)
    {
        elHareketEdiyor = true; 
        ParlamaSondur();

        Vector3 baslangicPos = elModeli.position;
        Vector3 aletPos = hedef.transform.position;
        Vector3 yon = (aletPos - baslangicPos).normalized;
        Vector3 hedefNokta = aletPos - (yon * almaPayi);

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / (animasyonSuresi / 2);
            elModeli.position = Vector3.Lerp(baslangicPos, hedefNokta, t);
            elModeli.LookAt(aletPos); 
            yield return null;
        }

        eldekiNesne = hedef;
        Rigidbody rb = eldekiNesne.GetComponent<Rigidbody>();
        if (rb != null) 
        { 
            rb.linearVelocity = Vector3.zero; 
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true; 
        }
        Collider col = eldekiNesne.GetComponent<Collider>();
        if (col != null) col.enabled = false;

        eldekiNesne.transform.SetParent(elModeli);
        
        // --- ÖZEL POZİSYON AYARLAMALARI (Metzenbaum Fix) ---
        string isim = eldekiNesne.name.ToLower();
        
        if (isim.Contains("metzenbaum") || isim.Contains("scissors") || isim.Contains("makas"))
        {
            // 180 derece çevir (Sapı sana gelsin)
            eldekiNesne.transform.localRotation = Quaternion.Euler(0, 180, 0); 
            eldekiNesne.transform.localPosition = new Vector3(-0.05f, 0, 0);
        }
        else
        {
            // Diğerleri (Neşter, Cımbız) düz kalsın
            eldekiNesne.transform.localPosition = Vector3.zero;
            eldekiNesne.transform.localRotation = Quaternion.identity;
        }

        Renderer r = eldekiNesne.GetComponent<Renderer>();
        if (r != null)
        {
            orijinalMat = bakilanMat;
            if (secimMateryali != null) r.material = secimMateryali;
        }

        yield return StartCoroutine(EliGeriGetir());
    }

    // --- BIRAKMA İŞLEMİ (YATAY BIRAKMA) ---
    IEnumerator ElUzanipBiraksin(Vector3 tiklananNokta)
    {
        elHareketEdiyor = true;
        Vector3 baslangicPos = elModeli.position;
        Vector3 hedefNokta = tiklananNokta + (Vector3.up * birakmaPayi);

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / (animasyonSuresi / 2);
            elModeli.position = Vector3.Lerp(baslangicPos, hedefNokta, t);
            
            // Sadece Y ekseninde dön (Yere bakma, yatay kal)
            Vector3 bakisYonu = tiklananNokta - elModeli.position;
            bakisYonu.y = 0; 
            if (bakisYonu != Vector3.zero)
            {
                elModeli.rotation = Quaternion.Slerp(elModeli.rotation, Quaternion.LookRotation(bakisYonu), t);
            }
            yield return null;
        }

        if (eldekiNesne != null)
        {
            SerbestBirak(false); // Masaya bırak (Fırlatma yok)
        }

        yield return StartCoroutine(EliGeriGetir());
    }

    IEnumerator EliGeriGetir()
    {
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / (animasyonSuresi / 2);
            elModeli.localPosition = Vector3.Lerp(elModeli.localPosition, elOrijinalLocalPos, t);
            elModeli.localRotation = Quaternion.Slerp(elModeli.localRotation, elOrijinalLocalRot, t);
            yield return null;
        }
        elModeli.localPosition = elOrijinalLocalPos;
        elModeli.localRotation = elOrijinalLocalRot;
        elHareketEdiyor = false;
    }

    void Firlat()
    {
        SerbestBirak(true); // Fırlat
    }

    // --- ORTAK BIRAKMA VE MOUSE İLE FIRLATMA FONKSİYONU ---
    void SerbestBirak(bool firlatilsinMi)
    {
        if (eldekiNesne == null) return;

        eldekiNesne.transform.SetParent(null);
        Renderer r = eldekiNesne.GetComponent<Renderer>();
        if (r != null && orijinalMat != null) r.material = orijinalMat;

        Collider col = eldekiNesne.GetComponent<Collider>();
        if (col != null) col.enabled = true;

        Rigidbody rb = eldekiNesne.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
            
            // Yere düşerken içinden geçmesin diye Continuous yapıyoruz
            rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

            if (firlatilsinMi)
            {
                // MOUSE HEDEFLEME SİSTEMİ
                // 1. Mouse'un olduğu yere ışın at
                Ray ray = anaKamera.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                Vector3 hedefNokta;

                // 2. Işın bir yere çarptı mı?
                if (Physics.Raycast(ray, out hit, 100f)) 
                {
                    hedefNokta = hit.point;
                }
                else
                {
                    // Boşluğa bakıyorsak uzakta bir nokta seç
                    hedefNokta = ray.GetPoint(10f);
                }

                // 3. O noktaya doğru yön al
                Vector3 firlatmaYonu = (hedefNokta - eldekiNesne.transform.position).normalized;

                // 4. Hafif yukarı kavis vererek fırlat
                rb.AddForce((firlatmaYonu * firlatmaGucu) + (Vector3.up * 0.5f), ForceMode.VelocityChange);
            }
        }
        eldekiNesne = null;
    }
}