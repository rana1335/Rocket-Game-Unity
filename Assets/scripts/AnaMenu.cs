using UnityEngine;
using UnityEngine.SceneManagement; 
using System.Collections; // Gecikme (Coroutine) fonksiyonları için gerekli

public class AnaMenu : MonoBehaviour
{
    [Header("Efekt Sesleri")]
    public AudioSource efektKaynagi; // Tıklama sesini çalacak olan obje
    public AudioClip butonSesi;      // Tıklayınca çıkacak ses dosyası

    [Header("Arka Plan Müziği")]
    public AudioSource muzikKaynagi; // Müziği çalacak olan obje
    public AudioClip arkaPlanMuzigi; // Çalacak müzik



    private void Start()
    {
        // Eğer müzik kaynağı ve müzik dosyası atandıysa, müziği döngüye alıp başlatır
        if (muzikKaynagi != null && arkaPlanMuzigi != null)
        {
            muzikKaynagi.clip = arkaPlanMuzigi;
            muzikKaynagi.loop = true; // Şarkı bitince başa sarsın
            muzikKaynagi.Play();
        }
    }

    // "Oyna" butonuna tıklandığında çalışacak fonksiyon
    public void OyunaBasla()
    {
        SesiCal(); // Tıklama sesini çal
        
        // Sesin bitmesini biraz bekleyip sonra sahneyi yükle
        StartCoroutine(SahneYukleGecikmeli("SampleScene", 0.4f));
    }

    // "Çıkış" butonuna tıklandığında çalışacak fonksiyon
    public void OyundanCik()
    {
        SesiCal(); // Tıklama sesini çal
        Debug.Log("Oyundan çıkılıyor..."); 
        
        StartCoroutine(OyundanCikGecikmeli(0.4f));
    }

    private bool sesAcikmi = true; // Sesin açık olup olmadığını takip eden değişken

    // "Ses" butonuna tıklandığında çalışacak fonksiyon
    public void SesiAcKapat()
    {
        SesiCal(); // Tıklama sesini çal
        
        sesAcikmi = !sesAcikmi; // Durumu tersine çevir (açıksa kapat, kapalıysa aç)
        
        // AudioListener.volume, oyundaki TÜM seslerin (arka plan müziği dahil) yüksekliğidir
        if (sesAcikmi)
        {
            AudioListener.volume = 1f; // Sesleri tamamen aç
        }
        else
        {
            AudioListener.volume = 0f; // Sesleri tamamen kapat
        }
    }

    // Sesi çalma fonksiyonu
    private void SesiCal()
    {
        if (efektKaynagi != null && butonSesi != null)
        {
            efektKaynagi.PlayOneShot(butonSesi);
        }
    }

    // Belirli bir süre bekledikten sonra sahneyi yükleyen Coroutine
    private IEnumerator SahneYukleGecikmeli(string sahneAdi, float gecikme)
    {
        yield return new WaitForSeconds(gecikme); // Belirtilen saniye kadar bekle
        SceneManager.LoadScene(sahneAdi); 
    }

    // Belirli bir süre bekledikten sonra oyunu kapatan Coroutine
    private IEnumerator OyundanCikGecikmeli(float gecikme)
    {
        yield return new WaitForSeconds(gecikme); // Belirtilen saniye kadar bekle
        
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit(); 
        #endif
    }
}
