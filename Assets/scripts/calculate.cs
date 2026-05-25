using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Arayüz elemanları (Text) için gerekli

// Dosya adı calculate.cs olduğu için sınıf adı da calculate olmak zorunda.
public class calculate : MonoBehaviour
{
    [Header("Arayüz (UI) Elemanları")]
    public Text uiHedefText;
    public Text uiMesafeText;
    public Text uiYakitText;
    public Text uiSureText;

    [Header("Roket Özellikleri")]
    public double roketHiziKmh = 100000.0; // Saatte 100.000 km hız
    public double yakitTuketimiKmBasina = 0.05; // Her kilometrede harcanan yakıt (Litre)

    [Header("Sonuçlar (Hesaplandıktan Sonra Görünür)")]
    public double hesaplananSureSaat;
    public double hesaplananSureGun;
    public double hesaplananSureYil;
    public double hesaplananYakitLitre;

    // Gezegenlerin Dünya'ya olan ortalama mesafeleri (Kilometre cinsinden)
    private Dictionary<string, long> mesafeler_km = new Dictionary<string, long>
    {
        { "güneş", 149600000 },
        { "merkür", 91700000 },
        { "venüs", 41400000 },
        { "mars", 78300000 },
        { "jüpiter", 628700000 },
        { "satürn", 1275000000 },
        { "uranüs", 2720000000 },
        { "neptün", 4350000000 }
    };

    void Start()
    {
        // Oyun başladığında ekranda eski yazı kalmasın diye temizliyoruz
        BilgileriTemizle();
    }

    // Gezegenin üzerine fare ile gelindiğinde çağrılacak metod
    public void BilgileriGoster(string hedefGirdisi)
    {
        string hedef = hedefGirdisi.Trim().ToLower();

        if (mesafeler_km.ContainsKey(hedef))
        {
            long mesafe = mesafeler_km[hedef];
            
            // Hesaplamalar
            hesaplananSureSaat = mesafe / roketHiziKmh;
            hesaplananSureGun = hesaplananSureSaat / 24.0;
            hesaplananSureYil = hesaplananSureGun / 365.0;
            hesaplananYakitLitre = mesafe * yakitTuketimiKmBasina;

            string hedefBuyukHarf = char.ToUpper(hedef[0]) + hedef.Substring(1);

            // Arayüzü (UI) Güncelle
            if (uiHedefText != null) uiHedefText.text = $"Hedef: {hedefBuyukHarf}";
            if (uiMesafeText != null) uiMesafeText.text = $"Mesafe: {mesafe:N0} KM";
            if (uiYakitText != null) uiYakitText.text = $"Gerekli Yakıt: {hesaplananYakitLitre:N0} L";
            
            if (uiSureText != null)
            {
                if (hesaplananSureYil >= 1)
                {
                    uiSureText.text = $"Tahmini Süre: {hesaplananSureYil:N2} Yıl";
                }
                else if (hesaplananSureGun >= 1)
                {
                    uiSureText.text = $"Tahmini Süre: {hesaplananSureGun:N0} Gün";
                }
                else
                {
                    uiSureText.text = $"Tahmini Süre: {hesaplananSureSaat:N0} Saat";
                }
            }
        }
        else
        {
            Debug.LogError($"HATA: Geçersiz bir hedef ({hedefGirdisi}) girdiniz. Lütfen listedeki hedeflerden birini seçin.");
        }
    }

    // Fare gezegenden çekildiğinde ekranı temizleyen metod
    public void BilgileriTemizle()
    {
        if (uiHedefText != null) uiHedefText.text = "";
        if (uiMesafeText != null) uiMesafeText.text = "";
        if (uiYakitText != null) uiYakitText.text = "";
        if (uiSureText != null) uiSureText.text = "";
    }
}