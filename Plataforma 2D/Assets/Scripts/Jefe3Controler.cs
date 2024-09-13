using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;
using Cinemachine;
using UnityEngine.UI;

public class Jefe3Controler : MonoBehaviour
{
    private PlayerControler player;
    private Rigidbody2D rb;
    private SpriteRenderer sp;
    private Animator anim;
    private CinemachineVirtualCamera cm;
    private bool aplicarFuerza;
    
    public float distanciaRecoger;
    public float distanciaDisparoCorto;
    public float distanciaCorrerODisparo;
    public float distanciaCorrer;
    public float distanciaDeteccionPincho;
    public float velocidadMovimiento;
    public int vidas= 3;
    public Vector3 offsetPiernas;

    private float vidaTotal;
    public bool atacando;
    public bool disparado = false;
    public float ataque = 0;

    private AudioSource audioSource;
    public UnityEvent unityEvent;

    public UnityEvent initLluviaEvent;
    public UnityEvent endLluviaEvent;
    public UnityEvent pausarLLuviaEvent;

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
        gameObject.name = "Jefe2";
        if(!PlayerPrefs.HasKey("SegundoJefe"))
            PlayerPrefs.SetInt("SegundoJefe",0);
    }

    void Update()
    {
        Vector2 direccion = (player.transform.position - transform.position).normalized * distanciaDeteccionPincho;
        Debug.DrawRay(transform.position, direccion, Color.red);

        if(VidaSlider != null)
        {
            VidaSlider.value = (float) vidas / vidaTotal;
        }

        if(anim != null && !anim.GetBool("die") && !atacando)
        {
            float distanciaActual = Vector2.Distance(transform.position,player.transform.position);
            CambiarVista(direccion.normalized.x);
            float diatnciaPiernas = Vector2.Distance(transform.position + offsetPiernas,player.transform.position);

            if(diatnciaPiernas <= distanciaRecoger)
            {
                ataque = 1;
                rb.velocity = new Vector2(0,(rb.velocity * new Vector2(1,1)).y);
                anim.SetBool("run",false);

                Vector2 direccionNormalizada = direccion.normalized;
                StartCoroutine(AtacarMaza(direccion));
            }
            else
            {
                if(distanciaActual <= distanciaDeteccionPincho)
                {
                    rb.velocity = new Vector2(0,(rb.velocity * new Vector2(1,1)).y);
                    ataque = 2;
                    anim.SetBool("run",false);
                    Vector2 direccionNormalizada = direccion.normalized;
                    StartCoroutine(AtacarMaza(direccion));
                }
                else
                {
                    if(distanciaActual <= distanciaDisparoCorto)
                    {
                        ataque = 0;
                        rb.velocity = new Vector2(0,(rb.velocity * new Vector2(1,1)).y);
                        anim.SetBool("run",false);
                        Vector2 direccionNormalizada = direccion.normalized;
                        StartCoroutine(AtacarMaza(direccion));
                    }
                    else
                    {
                        if(distanciaActual <= distanciaCorrer)
                        {
                            anim.SetBool("atacar",false);
                            Vector2 movimiento = new Vector2(direccion.x,0);
                            movimiento = movimiento.normalized;
                            rb.velocity = (movimiento * velocidadMovimiento) + new Vector2(0,rb.velocity.y);
                            anim.SetFloat("ataque",0);
                            anim.SetBool("run",true);
                        }
                        else
                        {
                            if(distanciaActual <= distanciaCorrerODisparo && VidaSlider.value <= 0.5  &&  !disparado)
                            {
                                anim.SetBool("atacar",true);
                                Vector2 movimiento = new Vector2(direccion.x,0);
                                atacando = true;
                                StartCoroutine("HacerLluvia");
                                anim.SetBool("run",false);
                            }
                            else 
                            {
                                if(distanciaActual <= distanciaCorrerODisparo)
                                {
                                    anim.SetBool("atacar",false);
                                    rb.velocity = Vector3.zero;
                                    anim.SetFloat("ataque",0);
                                    anim.SetBool("run",false);
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    private IEnumerator HacerLluvia()
    {
        disparado=true;
        ataque = 1;
        anim.SetFloat("ataque",ataque);
        initLluviaEvent.Invoke();
        yield return new WaitForSeconds(1.75f);
        anim.SetBool("atacar",false);
        atacando = false;
        yield return new WaitForSeconds(5f);
        pausarLLuviaEvent.Invoke();
        yield return new WaitForSeconds(2f);
        endLluviaEvent.Invoke();
        disparado = false;
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
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position + offsetPiernas, distanciaRecoger);
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, distanciaDeteccionPincho);
        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(transform.position, distanciaDisparoCorto);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, distanciaCorrer);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, distanciaCorrerODisparo);
    }

    private IEnumerator AtacarMaza(Vector2 direccionFlecha)
    {
        atacando = true;
        anim.SetFloat("ataque",ataque);
        anim.SetBool("atacar",true);

        if(ataque == 2)
        {
            yield return new WaitForSeconds(1.9f);
        }
        else
        {
            if(ataque == 0)
            {
                yield return new WaitForSeconds(1.75f);
            }
            else
            {
                yield return new WaitForSeconds(1.75f);
            }
        }
            
        anim.SetBool("atacar",false);
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
            rb.AddForce(new Vector2((transform.position - player.transform.position).normalized.x * 1, 0), ForceMode2D.Impulse);
            aplicarFuerza = false;
        }
    }

    private void Morir()
    {
        //audioSource.Play();
        unityEvent.Invoke();
        endLluviaEvent.Invoke();
        VidaSlider.value = 0;
        VidaSlider = null;
        PlayerPrefs.SetInt("SegundoJefe",1);
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
