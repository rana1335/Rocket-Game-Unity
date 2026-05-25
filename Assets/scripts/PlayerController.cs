using UnityEngine;
using UnityEngine.UI; // Arayüz (UI) kodlarını kullanabilmek için eklendi

public class PlayerController : MonoBehaviour
{
    [Header("Hareket Ayarları")]
    public float walkSpeed = 5f;
    public float runSpeed = 8f;
    public float jumpForce = 7f;

    [Header("Yeniden Doğma (Respawn) Ayarları")]
    public Transform respawnPoint;    // Karakterin yeniden doğacağı nokta (Boş bırakılırsa başlangıç noktasına döner)
    public float fallThreshold = -10f;// Karakterin hangi Y değerinin altına düştüğünde yeniden doğacağı

    [Header("Zemin Kontrolü")]
    public Transform groundCheck;     // Karakterin ayak hizasına koyacağımız boş obje
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;     // Zeminlerin hangi katmanda olduğunu seçeceğiz

    [Header("Arayüz (UI) Ayarları")]
    public Text infoText;             // Ekrana mesaj yazdırmak için UI Text objesi

    [Header("Bileşenler")]
    private Rigidbody2D rb;
    private Animator anim; 
    private SpriteRenderer spriteRenderer;

    private float horizontalInput;
    private bool isGrounded;
    private bool isRunning;
    private bool canMove = true;
    private bool isNearRocket = false;
    private Vector3 startPosition;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>(); 
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        // Oyun başladığında karakterin ilk konumunu kaydet
        startPosition = transform.position;
    }

    void Update()
    {
        if (!canMove) return;

        // Düşme Kontrolü
        if (transform.position.y < fallThreshold)
        {
            Respawn();
        }

        // 1. Zemin Kontrolü (Zıplama için yerde miyiz?)
        if (groundCheck != null)
        {
            isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        }
        else
        {
            // Eğer Unity'den GroundCheck objesini atamayı unutursanız şimdilik geçici olarak hep yerde saysın
            isGrounded = true; 
        }

        // 2. Girdileri Al (Sağ/Sol ve Koşma Tuşu)
        horizontalInput = Input.GetAxisRaw("Horizontal");
        isRunning = Input.GetKey(KeyCode.LeftShift); // Sol Shift'e basılı tutuluyorsa koşar

        // 3. Zıplama (W tuşu)
        if (Input.GetKeyDown(KeyCode.W) && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }

        // 4. --- ANIMASYON PARAMETRELERİ ---
        if (anim != null)
        {
            // Hareket şiddeti: Duruyorsa 0, Yürüyorsa 1, Koşuyorsa 2 olacak şekilde bir değer oluşturuyoruz
            float moveIntensity = Mathf.Abs(horizontalInput) * (isRunning ? 2f : 1f);
            
            anim.SetFloat("Speed", moveIntensity);
            
            // Eğer yerde değilsek zıplıyoruzdur (veya düşüyoruzdur)
            anim.SetBool("isJumping", !isGrounded);
        }

        // 5. Yön Dönme
        if (horizontalInput > 0)
        {
            spriteRenderer.flipX = false;
        }
        else if (horizontalInput < 0)
        {
            spriteRenderer.flipX = true;
        }

        // 6. Rokete Binme
        if (isNearRocket && Input.GetKeyDown(KeyCode.E))
        {
            EnterRocket();
        }
    }

    void FixedUpdate()
    {
        if (!canMove) return;

        // Mevcut hıza göre fiziksel hareketi uygula (Yürüme veya Koşma)
        float currentSpeed = isRunning ? runSpeed : walkSpeed;
        rb.linearVelocity = new Vector2(horizontalInput * currentSpeed, rb.linearVelocity.y);
    }

    private bool isBoarding = false;
    public bool RoketteMi { get { return isBoarding; } } // Roket kontrolünden ulaşabilmek için dışarıya açıldı

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Rocket") && !isBoarding)
        {
            isNearRocket = true;
            if (infoText != null) infoText.text = "Roketin yanındasın! Binmek için E tuşuna bas.";
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // Eğer rokete biniyorsak yazıyı temizleme!
        if (collision.CompareTag("Rocket") && !isBoarding)
        {
            isNearRocket = false;
            if (infoText != null) infoText.text = ""; // Uzaklaşınca ekrandaki yazıyı temizle
        }
    }

    private void Respawn()
    {
        // Eğer özel bir doğma noktası (respawnPoint) atandıysa oraya, atanmadıysa ilk başladığı yere gönder
        if (respawnPoint != null)
        {
            transform.position = respawnPoint.position;
        }
        else
        {
            transform.position = startPosition;
        }
        
        // Düşerken birikmiş olan hızı (velocity) ve dönüşü (rotation) sıfırla
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
        transform.rotation = Quaternion.identity; // Karakteri düzelt (dik konuma getir)
    }

    private void EnterRocket()
    {
        isBoarding = true; // Rokete binme işleminin başladığını belirtiyoruz
        
        if (infoText != null) infoText.text = "Rokete binildi! Hedef seçimi bekleniyor...";
        canMove = false; 
        rb.linearVelocity = Vector2.zero; 
        
        // Karakteri ekrandan tamamen gizle (Sanki roketin içine girmiş gibi)
        gameObject.SetActive(false);
    }

    // Zemin kontrol çemberini Unity editöründe kırmızı çizgi olarak görmek için
    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}
