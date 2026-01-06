using UnityEngine;
using UnityEngine.SceneManagement; // Sahne işlemleri için şart

public class OyunYonetimi : MonoBehaviour
{
    [Header("UI Ayarları")]
    public GameObject durdurmaPaneli; // Inspector'a sürükleyeceğin Panel

    private bool oyunDurduMu = false;

    void Start()
    {
        // Oyun her başladığında zamanın akmasını ve panelin kapalı olmasını garantiye alıyoruz
        Time.timeScale = 1f;
        durdurmaPaneli.SetActive(false);
    }

    void Update()
    {
        // ESC tuşuna basıldığında
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (oyunDurduMu)
            {
                DevamEt(); // Zaten durmuşsa devam ettir
            }
            else
            {
                Durdur(); // Çalışıyorsa durdur
            }
        }
    }

    // --- Fonksiyonlar (Butonlara Bağlanacaklar) ---

    // 1. ESC'ye basınca çalışır (Otomatik)
    public void Durdur()
    {
        durdurmaPaneli.SetActive(true); // Paneli AÇ
        Time.timeScale = 0f; // Zamanı DURDUR
        oyunDurduMu = true;
        
        // Menüde rahat tıklamak için mouse'u serbest bırak
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // 2. "DEVAM ET" Butonu için
    public void DevamEt()
    {
        durdurmaPaneli.SetActive(false); // Paneli KAPAT
        Time.timeScale = 1f; // Zamanı DEVAM ETTİR
        oyunDurduMu = false;
    }

    // 3. "YENİDEN BAŞLAT" Butonu için
    public void YenidenBaslat()
    {
        Time.timeScale = 1f; // Önce zamanı normal hıza alıyoruz (yoksa oyun donuk başlar)
        
        // Şu anki sahneyi (SampleScene) baştan yükler
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // 4. "ÇIKIŞ YAP" Butonu için
    public void Cikis()
    {
        Debug.Log("Oyundan çıkılıyor...");

        #if UNITY_EDITOR
            // Eğer Unity Editöründeysek, Play modunu durdur:
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            // Eğer gerçek uygulamadaysak (.exe), oyunu kapat:
            Application.Quit();
        #endif
    }
}