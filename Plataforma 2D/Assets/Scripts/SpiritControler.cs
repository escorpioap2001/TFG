using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class SpiritControler : MonoBehaviour
{
    private CinemachineVirtualCamera cm;
    private SpriteRenderer sp;
    private PlayerControler player;
    private Rigidbody2D rb;
    private bool aplicarFuerza;
    private Animator anim;

    public float velocidadDeMovimiento = 3;
    public float  radioDeteccion = 15.0f;
    public LayerMask layerJugador; 

    public Vector2 posiciónCabeza;

    public int vidas = 3;
    public string nombre;

    private AudioSource audioSource;
    private bool detectadoJugador = false;

    private EspiaFunction ef;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        cm = GameObject.FindGameObjectWithTag("VirtualCamera").GetComponent<CinemachineVirtualCamera>();
        sp = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControler>();
        audioSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        gameObject.name = nombre;
        ef =  GameObject.Find("Espia").GetComponent<EspiaFunction>();
    }

    //Funcion para visualizar el radio de detección
    private void OnDrawGizmosSelected() 
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radioDeteccion);
        Gizmos.color = Color.green;
        Gizmos.DrawCube((Vector2)transform.position + posiciónCabeza, new Vector2(1.0f,0.5f) * 0.7f);
    }

    void Update()
    {
        Vector2 direccion = player.transform.position - transform.position;
        float distancia = Vector2.Distance(transform.position, player.transform.position);

        if(distancia <= radioDeteccion)
        {
            if(!anim.GetBool("die"))
            {
                rb.velocity = direccion.normalized * velocidadDeMovimiento;
                CambiarVista(direccion.normalized.x);
                if(!detectadoJugador)
                {
                    audioSource.Play();
                    detectadoJugador = true;
                }
            }
        }
        else
        {
            rb.velocity = Vector2.zero;
            detectadoJugador = false;
        }
    }

    private void CambiarVista(float dirX)
    {
        if(dirX < 0 && transform.localScale.x > 0)
        {
            transform.localScale = new Vector3(-transform.localScale.x,transform.localScale.y,transform.localScale.z);
        }
        else if(dirX > 0 && transform.localScale.x < 0)
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x),transform.localScale.y,transform.localScale.z);
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            if(transform.position.y + posiciónCabeza.y < player.transform.position.y - 0.7f)
            {
                player.GetComponent<Rigidbody2D>().velocity = Vector2.up * player.fuerzaDeSalto;
                StartCoroutine(AgitarCamara(0.1f));
                vidas = 0;
                Morir();
            }
            else
            {
                player.RecibirDaño((transform.position -player.transform.position).normalized);
            }
        }
    }

    private void FixedUpdate() 
    {
        if(aplicarFuerza)
        {
            rb.AddForce((transform.position - player.transform.position).normalized * 100, ForceMode2D.Impulse);
            aplicarFuerza = false;
        }
    }

    public void RecibirDaño()
    {
        if(vidas > 1)
        {
            StartCoroutine(AgitarCamara(0.1f));
            StartCoroutine(EfectoDaño());
            aplicarFuerza = true;
        }
        else
        {
            aplicarFuerza = true;
            Morir();
        }
        vidas--;
    }

    public IEnumerator AgitarCamara(float tiempo)
    {
        CinemachineBasicMultiChannelPerlin cinemachineBMCP = cm.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        cinemachineBMCP.m_AmplitudeGain = 4;
        yield return new WaitForSeconds(tiempo);
        cinemachineBMCP.m_AmplitudeGain = 0;
    }
    
    public IEnumerator EfectoDaño()
    {
        sp.color = Color.red;
        yield return new WaitForSeconds(0.2f);
        sp.color = Color.white;
    }

    private void Morir()
    {
        ef.sumarInformacion("muerte");
        anim.SetBool("die",true);
        rb.gravityScale = 3;
        rb.mass = 5;
        Physics2D.IgnoreCollision(GetComponent<Collider2D>(),player.GetComponent<Collider2D>(),true);
        Destroy(gameObject, 2.0f);
    }
}

