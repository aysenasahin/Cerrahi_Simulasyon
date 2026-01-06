using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections; // Coroutine için gerekli

public class MenuYonetimi : MonoBehaviour
{
    public GameObject yuklemeYazisi; // Inspector'dan o gizlediğin yazıyı buraya atacağız

    public void SimulasyonuBaslat()
    {
        // Butona basılınca bu işlem başlar
        StartCoroutine(SahneYukle());
    }

    IEnumerator SahneYukle()
    {
        // 1. Önce "Yükleniyor" yazısını görünür yap
        if (yuklemeYazisi != null)
        {
            yuklemeYazisi.SetActive(true);
        }

        // >> KRİTİK NOKTA BURASI <<
        // Unity'nin yazıyı ekrana çizebilmesi için çok kısa (0.1 sn) bekle.
        // Bu komut sayesinde yazı ekranda belirir, sonra donma başlar.
        yield return new WaitForSeconds(0.1f);

        // 2. Şimdi sahneyi arka planda yüklemeye başla
        AsyncOperation operasyon = SceneManager.LoadSceneAsync(1);

        // Yükleme bitene kadar bekle
        while (!operasyon.isDone)
        {
            yield return null;
        }
    }

    public void UygulamadanCik()
    {
        Debug.Log("Çıkış yapıldı!");

        #if UNITY_EDITOR
            // Eğer Unity Editöründeysek, Play modunu durdur:
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            // Eğer gerçek uygulamadaysak (.exe), oyunu kapat:
            Application.Quit();
        #endif
    }
}