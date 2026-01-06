using UnityEngine;
using System.Collections;

public class YereDusenAlet : MonoBehaviour
{
    private Vector3 baslangicPozisyonu;
    private Quaternion baslangicRotasyonu;
    private Rigidbody rb;

    void Start()
    {
        // Oyun başlarken alet nerede duruyorsa orayı kaydet
        baslangicPozisyonu = transform.position;
        baslangicRotasyonu = transform.rotation;
        rb = GetComponent<Rigidbody>();
    }

    void OnCollisionEnter(Collision collision)
    {
        // 1. ÖNCE GÜVENLİK KONTROLÜ: Çarptığım şey Masa mı?
        // Eğer Masaya çarptıysam HİÇBİR ŞEY YAPMA (Resetleme iptal)
        if (collision.gameObject.CompareTag("masa")) 
        {
            return; // Fonksiyondan çık, aşağıya inme
        }

        // 2. ZEMİN KONTROLÜ: Çarptığım şey Zemin mi?
        if (collision.gameObject.CompareTag("Zemin") || collision.gameObject.name.Contains("Floor"))
        {
            StartCoroutine(MasayaGeriDon());
        }
    }

    IEnumerator MasayaGeriDon()
    {
        // 2 saniye yerde kalsın (düştüğü görünsün)
        yield return new WaitForSeconds(2.0f);

        // Aletin hızını sıfırla (durdur)
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero; 
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true; // Titremesin diye dondur
        }

        // Işınla
        transform.position = baslangicPozisyonu;
        transform.rotation = baslangicRotasyonu;

        // Işınlandıktan sonra kısa bir bekleme
        yield return new WaitForSeconds(0.1f);
        
        // Tekrar alınabilir hale getir (Fiziği aç)
        if (rb != null) rb.isKinematic = false;
    }
}