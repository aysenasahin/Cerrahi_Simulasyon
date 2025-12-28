using UnityEngine;

public class DoktorHareket : MonoBehaviour
{
    public float hiz = 1.8f;
    public float mouseHassasiyeti = 2.0f;
    public Transform doktorGovde;

    public float yerCekimi = 20f;

    [Header("Görüntü Yumuşatma")]
    public float goruntuYumusatma = 12f; // 8–20 arası ideal

    float xRotasyon = 0f;
    float dususHizi = 0f;

    float mouseX_sm = 0f;
    float mouseY_sm = 0f;

    CharacterController controller;

    void Start()
    {
        controller = GetComponentInParent<CharacterController>();

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void Update()
    {
        // --- MOUSE İLE BAKIŞ (SAĞ TIK) ---
        if (Input.GetMouseButton(1))
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            float mouseX = Input.GetAxis("Mouse X") * mouseHassasiyeti;
            float mouseY = Input.GetAxis("Mouse Y") * mouseHassasiyeti;

            // SADE GÖRÜNTÜ YUMUŞATMA
            mouseX_sm = Mathf.Lerp(mouseX_sm, mouseX, goruntuYumusatma * Time.deltaTime);
            mouseY_sm = Mathf.Lerp(mouseY_sm, mouseY, goruntuYumusatma * Time.deltaTime);

            xRotasyon -= mouseY_sm;
            xRotasyon = Mathf.Clamp(xRotasyon, -90f, 90f);

            transform.localRotation = Quaternion.Euler(xRotasyon, 0f, 0f);
            doktorGovde.Rotate(Vector3.up * mouseX_sm);
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            mouseX_sm = 0f;
            mouseY_sm = 0f;
        }

        // --- YER ÇEKİMİ
        if (controller.isGrounded)
            dususHizi = -2f;
        else
            dususHizi -= yerCekimi * Time.deltaTime;

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 hareket =
            doktorGovde.right * x +
            doktorGovde.forward * z;

        Vector3 hareketVektoru =
            hareket * hiz +
            Vector3.up * dususHizi;

        controller.Move(hareketVektoru * Time.deltaTime);
    }
}
