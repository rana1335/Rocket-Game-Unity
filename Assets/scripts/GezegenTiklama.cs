using UnityEngine;

// Bu script her bir gezegen objesine eklenmelidir
[RequireComponent(typeof(Collider2D))]
public class GezegenTiklama : MonoBehaviour
{
    private RoketKontrol roketKontrol;

    void Start()
    {
        // Sahnedeki roketin üzerindeki RoketKontrol scriptini otomatik bulur
        roketKontrol = FindObjectOfType<RoketKontrol>();

        if (roketKontrol == null)
        {
            Debug.LogError("Sahnede RoketKontrol scripti bulunamadı! Roketin üzerine eklediğinizden emin olun.");
        }
    }

    // Fareyle bu gezegene (üzerindeki Collider'a) tıklandığında çalışır
    void OnMouseDown()
    {
        if (roketKontrol != null)
        {
            GezegenHover hoverBilgisi = GetComponent<GezegenHover>();
            string ad = hoverBilgisi != null ? hoverBilgisi.gezegenAdi : "";

            // Rokete, bu gezegenin (transform) konumuna doğru uçmasını söyle ve adını gönder
            roketKontrol.UcusBaslat(transform, ad);
        }
    }
}
