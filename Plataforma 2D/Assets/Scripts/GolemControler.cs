using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class GolemControler : MonoBehaviour
{
    private PlayerControler player;
    private Rigidbody2D rb;
    private SpriteRenderer sp;
    private Animator anim;
    private CinemachineVirtualCamera cm;
    private bool aplicarFuerza;
    
    public float distanciaDeteccionJugador;
    public float distanciaDeteccionMazo;
    public float velocidadMovimiento;
    public int vidas= 3;
    public bool atacando;

    private AudioSource audioSource;
    private EspiaFunction ef;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControler>();
        rb = GetComponent<Rigidbody2D>();
        sp = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        cm = GameObject.FindGameObjectWithTag("VirtualCamera").GetComponent<CinemachineVirtualCamera>();
        audioSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        gameObject.name = "Golem";
        ef =  GameObject.Find("Espia").GetComponent<EspiaFunction>();
    }

    void Update()
    {
        Vector2 direccion = (player.transform.position - transform.position).normalized * distanciaDeteccionMazo;
        Debug.DrawRay(transform.position, direccion, Color.red);
        if(!anim.GetBool("morir"))
        {
            float distanciaActual = Vector2.Distance(transform.position,player.transform.position);
            if(distanciaActual <= distanciaDeteccionMazo)
            {
                rb.velocity = new Vector2(0,(rb.velocity * new Vector2(1,1)).y);
                anim.SetBool("caminando",false);

                Vector2 direccionNormalizada = direccion.normalized;
                CambiarVista(direccion.normalized.x);
                if(!atacando)
                {
                    transform.GetChild(0).GetComponent<AreaAtaqueEnemy>().enemy = this.gameObject;
                    StartCoroutine(AtacarMaza(direccion));
                }
            }
            else if(!atacando)
            {
                if(distanciaActual <= distanciaDeteccionJugador)
                {
                    Vector2 movimiento = new Vector2(direccion.x,0);
                    movimiento = movimiento.normalized;
                    rb.velocity = (movimiento * velocidadMovimiento) + new Vector2(0,rb.velocity.y);
                    anim.SetBool("atacando",false);
                    anim.SetBool("caminando",true);
                    CambiarVista(movimiento.x);
                }
                else
                {
                    anim.SetBool("caminando",false);
                    anim.SetBool("atacando",false);
                }

            }
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

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, distanciaDeteccionJugador);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, distanciaDeteccionMazo);
    }

    private IEnumerator AtacarMaza(Vector2 direccionFlecha)
    {
        atacando = true;
        anim.SetBool("atacando",true);
        yield return new WaitForSeconds(1f);
        anim.SetBool("atacando",false);
        atacando = false;
    }

    public void RecibirDaño()
    {
        if(vidas > 1)
        {
            StartCoroutine(EfectoDaño());
            aplicarFuerza = true;
            StartCoroutine(AgitarCamara(0.1f));
        }
        else
        {
            aplicarFuerza = true;
            velocidadMovimiento = 0;
            rb.velocity = Vector2.zero;
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

    private void OnCollisionEnter2D(Collision2D other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            player.RecibirDaño((transform.position -player.transform.position).normalized);
        }
    }

    private void FixedUpdate() 
    {
        if(aplicarFuerza)
        {
            rb.AddForce((transform.position - player.transform.position).normalized * 4, ForceMode2D.Impulse);
            aplicarFuerza = false;
        }
    }

    private void Morir()
    {
        audioSource.Play();
        ef.sumarInformacion("muerte");
        gameObject.tag = "Untagged";
        atacando = true;
        anim.SetBool("morir",true);
        Physics2D.IgnoreCollision(GetComponent<Collider2D>(),player.GetComponent<Collider2D>(),true);
        Destroy(gameObject, 2.0f);
    }
}
