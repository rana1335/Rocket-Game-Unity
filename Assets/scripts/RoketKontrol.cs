using UnityEngine;

public class RoketKontrol : MonoBehaviour
{
    [Header("Uçuş Ayarları")]
    [Tooltip("Uçuşun toplam süresi (saniye)")]
    public float ucusSuresi = 2f; 
    [Tooltip("Roketin uçuş sırasındaki kavis (parabol) yüksekliği")]
    public float kavisYuksekligi = 5f;
    [Tooltip("Uçuşun gidiş animasyon eğrisi (Zamanlama)")]
    public AnimationCurve ucusEgrisi = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
    [Tooltip("Uçuşun dikey kavis (parabol) animasyon eğrisi")]
    public AnimationCurve kavisEgrisi = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(0.5f, 1f), new Keyframe(1f, 0f));
    [Tooltip("Uçuş sırasında roketin boyutunun (ölçeğinin) nasıl değişeceğini belirleyen eğri")]
    public AnimationCurve boyutEgrisi = AnimationCurve.Linear(0f, 1f, 1f, 0.2f); // Başlangıçta 1 (tam boyut), sonda 0.2 (küçülmüş)

    [Header("Bayrak Ayarları")]
    [Tooltip("Gezegene inince oluşturulacak bayrak Prefab'ı")]
    public GameObject bayrakPrefab; 
    [Tooltip("Bayrağın gezegen merkezinden ne kadar yukarıda/farklı bir konumda çıkacağını ayarlayan değer")]
    public Vector3 bayrakOffset = new Vector3(0f, 1f, 0f); 

    [Header("UI Ayarları")]
    [Tooltip("Sadece gezegene inince ortaya çıkacak olan Geri Dönüş butonu")]
    public GameObject geriDonButonu;
    
    private Transform hedefGezegen;
    private bool ucusBasladi = false;
    private PlayerController playerController;
    private Vector3 baslangicPozisyonu;
    private Vector3 baslangicOlcegi;
    private Vector3 anaUsPozisyonu; // Oyun ilk başladığında roketin bulunduğu ana üs konumu
    private GameObject aktifBayrak; // Gezegende oluşturulan güncel bayrak referansı
    private float gecenSure = 0f;
    private calculate hesaplamaSistemi;

    void Start()
    {
        playerController = FindObjectOfType<PlayerController>();
        baslangicOlcegi = transform.localScale; // Roketin orijinal boyutunu kaydet
        anaUsPozisyonu = transform.position; // Roketin ilk doğduğu (ana üs) konumunu kaydet
        hesaplamaSistemi = FindObjectOfType<calculate>();

        // Oyun başında butonu gizle
        if (geriDonButonu != null)
        {
            geriDonButonu.SetActive(false);
        }
    }

    void Update()
    {
        if (ucusBasladi && hedefGezegen != null)
        {
            gecenSure += Time.deltaTime;
            float t = Mathf.Clamp01(gecenSure / ucusSuresi);

            // Eğrilere göre ilerleme durumunu hesapla
            float ilerlemeT = ucusEgrisi.Evaluate(t);
            float dikeyT = kavisEgrisi.Evaluate(t);

            // Başlangıç noktasından hedefe doğru doğrusal pozisyon hesaplaması
            Vector3 dogrusalPozisyon = Vector3.Lerp(baslangicPozisyonu, hedefGezegen.position, ilerlemeT);

            // Parabolik hareketi (y ekseninde kavis) pozisyona ekle
            dogrusalPozisyon.y += dikeyT * kavisYuksekligi;

            transform.position = dogrusalPozisyon;

            // Boyutu eğriye göre güncelle
            float olcekT = boyutEgrisi.Evaluate(t);
            transform.localScale = baslangicOlcegi * olcekT;

            // Uçuş süresi dolduğunda hedefe varılmış sayılır
            if (t >= 1f)
            {
                HedefeVarildi();
            }
        }
    }

    public void UcusBaslat(Transform gezegen, string gezegenAdi = "")
    {
        // Adam (Player) rokete bindi mi kontrolü
        if (playerController != null && !playerController.RoketteMi)
        {
            Debug.Log("Karakter rokete binmeden uçuş başlatılamaz!");
            return; // Eğer binmediyse uçuşu başlatmadan fonksiyondan çık
        }

        // Uçuş başladıysa bilgileri ekranda göster
        if (hesaplamaSistemi != null && !string.IsNullOrEmpty(gezegenAdi))
        {
            hesaplamaSistemi.BilgileriGoster(gezegenAdi);
        }

        hedefGezegen = gezegen;
        baslangicPozisyonu = transform.position; // Uçuşun başlayacağı konumu kaydet
        gecenSure = 0f; // Zamanlayıcıyı sıfırla
        ucusBasladi = true;
    }

    private void HedefeVarildi()
    {
        ucusBasladi = false;

        // 1. Roketi gizle (kaybolsun)
        gameObject.SetActive(false);

        // 2. Bayrak prefab'ını gezegenin üzerinde oluştur
        if (bayrakPrefab != null)
        {
            Vector3 bayrakPozisyonu = hedefGezegen.position + bayrakOffset;
            GameObject yeniBayrak = Instantiate(bayrakPrefab, bayrakPozisyonu, Quaternion.identity);
            aktifBayrak = yeniBayrak; // Geri dönerken silebilmek için referansını sakla
            
            // 3. Bayrak yükselme animasyonunu tetikle
            // Animatör direk objede veya altındaki bayrak/direk objesinde olabilir
            Animator animator = yeniBayrak.GetComponentInChildren<Animator>();
            if (animator != null)
            {
                animator.SetTrigger("BayrakDik");
            }
            else
            {
                Debug.LogWarning("Oluşturulan bayrak prefab'ında Animator bulunamadı! Lütfen Prefab'a Animator ekleyin.");
            }
        }
        else
        {
            Debug.LogError("RoketKontrol'de Bayrak Prefab'ı atanmamış! Lütfen Unity arayüzünden atamasını yapın.");
        }

        // 4. Geri dönüş butonunu görünür yap
        if (geriDonButonu != null)
        {
            geriDonButonu.SetActive(true);
        }
    }

    // Butona tıklandığında çalışacak olan Geri Dönüş metodu
    public void GeriDon()
    {
        // 1. Eğer bir gezegende bayrak dikilmişse, o bayrağı yok et
        if (aktifBayrak != null)
        {
            Destroy(aktifBayrak);
        }

        // 2. Roketi tekrar aktif (görünür) hale getir
        gameObject.SetActive(true);

        // 3. Roketin pozisyonunu ve boyutunu ilk başlangıç (ana üs) değerlerine döndür
        transform.position = anaUsPozisyonu;
        transform.localScale = baslangicOlcegi;
        
        // Opsiyonel: Uçuş durumlarını sıfırla
        ucusBasladi = false;
        hedefGezegen = null;

        // 4. Butonu tekrar gizle
        if (geriDonButonu != null)
        {
            geriDonButonu.SetActive(false);
        }

        // 5. Ekrandaki yazıları (gezegen yakıt vs) temizle
        if (hesaplamaSistemi != null)
        {
            hesaplamaSistemi.BilgileriTemizle();
        }
    }
}
