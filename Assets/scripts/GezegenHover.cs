using UnityEngine;

// Bu script her bir gezegen objesinin (Sprite) üzerine takılmalıdır.
// Gezegenin üzerinde mutlaka Collider2D (örneğin CircleCollider2D) olmalıdır.
public class GezegenHover : MonoBehaviour
{
    [Tooltip("Gezegenin adını küçük harflerle yazın (örn: mars, jüpiter, güneş)")]
    public string gezegenAdi;

    [Tooltip("Fareyle üzerine gelince gezegenin ne kadar büyüyeceği (örn: 1.1 = %10 büyüme)")]
    public float buyumeOrani = 1.1f;

    private Vector3 orijinalBoyut;

    void Start()
    {
        // Gezegenin oyuna başladığı orijinal boyutunu hafızaya alıyoruz
        orijinalBoyut = transform.localScale;
    }

    void OnMouseEnter()
    {
        // Gezegeni biraz büyüt
        transform.localScale = orijinalBoyut * buyumeOrani;
    }

    void OnMouseExit()
    {
        // Gezegeni eski (orijinal) boyutuna geri döndür
        transform.localScale = orijinalBoyut;
    }
}
