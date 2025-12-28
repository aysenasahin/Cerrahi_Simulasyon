using UnityEngine;

public class DoktorHareket : MonoBehaviour
{
    // MÜHENDİSLİK STANDARDI: Güvenli yürüme hızı
    public float hiz = 1.8f; 
    public float mouseHassasiyeti = 2.0f; 
    
    public Transform doktorGovde;

    float xRotasyon = 0f;

    void Start()
    {
        // ARTIK MOUSE'U KİLİTLEMİYORUZ! Serbestçe gezsin.
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true; // Mouse imleci görünsün
    }

    void Update()
    {
        // --- MOUSE İLE BAKIŞ (Sadece SAĞ TIK basılıyken) ---
        if (Input.GetMouseButton(1)) 
        {
            // Sağ tık basılıyken imleci gizle ve kilitle ki rahat dönelim
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            float mouseX = Input.GetAxis("Mouse X") * mouseHassasiyeti;
            float mouseY = Input.GetAxis("Mouse Y") * mouseHassasiyeti;

            xRotasyon -= mouseY;
            xRotasyon = Mathf.Clamp(xRotasyon, -90f, 90f); 

            transform.localRotation = Quaternion.Euler(xRotasyon, 0f, 0f); 
            doktorGovde.Rotate(Vector3.up * mouseX); 
        }
        else
        {
            // Sağ tıkı bıraktığım an mouse geri gelsin
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        // --- KLAVYE İLE YÜRÜME (Her zaman çalışır) ---
        float x = Input.GetAxis("Horizontal"); 
        float z = Input.GetAxis("Vertical");   

        Vector3 hareket = doktorGovde.right * x + doktorGovde.forward * z;
        doktorGovde.Translate(hareket * hiz * Time.deltaTime, Space.World);
    }
}