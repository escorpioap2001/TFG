using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;
using Cinemachine;
using UnityEngine.UI;


public class Jefe2AtaqController : MonoBehaviour
{
    private PlayerControler player;
    private Rigidbody2D rb;
    private SpriteRenderer sp;
    private Animator anim;
    private CinemachineVirtualCamera cm;
    private bool aplicarFuerza;
    
    public float distanciaDeteccionJugador;
    public float distanciaCorreraJugador;
    public float distanciaDeteccionMazo;
    public float velocidadMovimiento;
    public float velocidadEnvestida;
    public int vidas= 3;
    private float vidaTotal;
    public bool atacando;
    public int ataque = 0;

    private AudioSource audioSource;
    public UnityEvent unityEvent;

    [SerializeField] Slider VidaSlider;
    
    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControler>();
        rb = GetComponent<Rigidbody2D>();
        sp = transform.GetChild(0).GetComponent<SpriteRenderer>();
        anim = transform.GetChild(0).GetComponent<Animator>();
        cm = GameObject.FindGameObjectWithTag("VirtualCamera").GetComponent<CinemachineVirtualCamera>();
        audioSource = GetComponent<AudioSource>();
        vidaTotal = vidas;
    }

    void Start()
    {
        gameObject.name = "Jefe1";
        if(!PlayerPrefs.HasKey("PrimerJefe"))
            PlayerPrefs.SetInt("PrimerJefe",0);
    }

    void Update()
    {
        Vector2 direccion = (player.transform.position - transform.position).normalized * distanciaDeteccionMazo;
        Debug.DrawRay(transform.position, direccion, Color.red);

        if(VidaSlider != null)
        {
            VidaSlider.value = (float) vidas / vidaTotal;
        }

        if(anim != null && !anim.GetBool("die") && !atacando)
        {
            float distanciaActual = Vector2.Distance(transform.position,player.transform.position);
            if(distanciaActual <= distanciaDeteccionMazo)
            {
                //Debug.Log("Realizado ataque");
                ataque = (int) Random.Range(1.0f,2.99f);
                rb.velocity = new Vector2(0,(rb.velocity * new Vector2(1,1)).y);
                anim.SetBool("run",false);

                Vector2 direccionNormalizada = direccion.normalized;
                CambiarVista(direccion.normalized.x);
                if(!atacando)
                {
                    StartCoroutine(AtacarMaza(direccion));
                }
            }
            else
            {
                if(distanciaActual <= distanciaCorreraJugador)
                {
                    Vector2 movimiento = new Vector2(direccion.x,0);
                    movimiento = movimiento.normalized;
                    rb.velocity = (movimiento * velocidadMovimiento) + new Vector2(0,rb.velocity.y);
                    anim.SetInteger("ataque",0);
                    anim.SetBool("run",true);
                    CambiarVista(movimiento.x);
                }
                else
                {
                    if(distanciaActual <= distanciaDeteccionJugador)
                    {
                        Vector2 movimiento = new Vector2(direccion.x,0);
                        rb.velocity = (movimiento * velocidadEnvestida) + new Vector2(0,rb.velocity.y);
                        anim.SetBool("dash", true);
                        anim.SetInteger("ataque",0);
                        anim.SetBool("run",false);
                        CambiarVista(movimiento.x);
                        atacando = true;
                    }
                    else
                    {
                        anim.SetBool("dash", false);
                        anim.SetBool("run",false);
                        anim.SetInteger("ataque",0);
                    }
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
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, distanciaCorreraJugador);
        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(transform.position, distanciaDeteccionJugador);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, distanciaDeteccionMazo);
    }

    private IEnumerator AtacarMaza(Vector2 direccionFlecha)
    {
        atacando = true;
        anim.SetInteger("ataque",ataque);
        if(ataque == 2)
            yield return new WaitForSeconds(1f);
        else
            yield return new WaitForSeconds(2f);
        anim.SetInteger("ataque",0);
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

    public void Chocar()
    {
        rb.velocity = new Vector2(0,0);
        StartCoroutine("EfectoStunned");
    }

    public IEnumerator EfectoStunned()
    {
        anim.SetBool("stunned", true);
        yield return new WaitForSeconds(2.0f);
        anim.SetBool("dash", false);
        anim.SetBool("stunned", false);
        anim.SetBool("run", false);
        atacando = false;
    }

    /*private void OnCollisionEnter2D(Collision2D other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            player.RecibirDaño((transform.position -player.transform.position).normalized);
        }
    }*/

    private void FixedUpdate() 
    {
        if(aplicarFuerza)
        {
            rb.AddForce(new Vector2((transform.position - player.transform.position).normalized.x * 1, 0), ForceMode2D.Impulse);
            aplicarFuerza = false;
        }
    }

    private void Morir()
    {
        //audioSource.Play();
        unityEvent.Invoke();
        VidaSlider.value = 0;
        VidaSlider = null;
        PlayerPrefs.SetInt("PrimerJefe",1);
        gameObject.tag = "Untagged";
        atacando = true;
        anim.SetBool("die",true);
        rb.drag = 10;
        Physics2D.IgnoreCollision(transform.GetChild(0).GetComponent<Collider2D>(),player.GetComponent<Collider2D>(),true);
        Destroy(transform.GetChild(0).gameObject, 2.0f);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player") && vidas > 1)
        {
            VidaSlider.gameObject.SetActive(true);
        }
    }
}
