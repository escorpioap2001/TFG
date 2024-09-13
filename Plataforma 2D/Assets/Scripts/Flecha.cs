using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flecha : MonoBehaviour
{
    private Rigidbody2D rb;
    private BoxCollider2D bc;

    public LayerMask layerPiso;
    public LayerMask layerParedes;
    public GameObject esqueleto;
    public Vector2 direccionFlecha;
    public float radioDeColision = 0.25f;
    public bool tocaSuelo;
    public bool tocaPared;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        bc = GetComponent<BoxCollider2D>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            other.GetComponent<PlayerControler>().RecibirDaño(-(other.transform.position - esqueleto.transform.position).normalized);
            Destroy(this.gameObject);
        }
    }

    private void Update() 
    {
        tocaSuelo = Physics2D.OverlapCircle((Vector2)transform.position, radioDeColision, layerPiso);
        tocaPared = Physics2D.OverlapCircle((Vector2)transform.position, radioDeColision, layerParedes);

        if(tocaSuelo || tocaPared)
        {
            rb.bodyType = RigidbodyType2D.Static;
            bc.enabled = false;
            this.enabled = false;
        }

        float angulo = Mathf.Atan2(direccionFlecha.y, direccionFlecha.x) * Mathf.Rad2Deg;

        transform.localEulerAngles = new Vector3(transform.localEulerAngles.y,transform.localEulerAngles.x,angulo);
    }

}
