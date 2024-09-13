using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class Waypoints : MonoBehaviour
{
    private Vector3 direccion;
    private PlayerControler player;
    private CinemachineVirtualCamera cm;
    private Rigidbody2D rb;
    private SpriteRenderer sp;
    private Animator anim;
    public int indiceActual = 0;
    private bool aplicarFuerza = false;

    public int vidas = 3;
    public Vector2 posicionCabeza;
    public float velocidadDesplazamiento;
    public List<Transform> puntos = new List<Transform>();
    public bool esperando;
    public float tiempoEspera;

    private AudioSource audioSource;
    public AudioClip audioClip;

    private EspiaFunction ef;

    private void Awake()
    {
        cm = GameObject.FindGameObjectWithTag("VirtualCamera").GetComponent<CinemachineVirtualCamera>();
        sp = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControler>();
        audioSource = GetComponent<AudioSource>();
    }

    private void Start() 
    {
        if(gameObject.CompareTag("Enemigo"))
        {
            gameObject.name = "Spider";
            anim = GetComponent<Animator>();
        }
        if(GameObject.Find("Espia")!=null)
            ef =  GameObject.Find("Espia").GetComponent<EspiaFunction>();
        
    }


    private void FixedUpdate()
    {
        MovimientoWaypoints();
        if(gameObject.CompareTag("Enemigo"))
        {
            CambiarEscalaEnemigo();
        }
        else if (gameObject.CompareTag("Prop"))
        {
            CambiarEscalaEnemigo();
        }

        if(aplicarFuerza)
        {
            rb.AddForce((transform.position - player.transform.position).normalized * 5, ForceMode2D.Impulse);
            aplicarFuerza = false;
        }
    }
    
    private void OnCollisionEnter2D(Collision2D other)
    {
        if(other.gameObject.CompareTag("Player") && gameObject.CompareTag("Enemigo"))
        {
            if(player.transform.position.y - 0.7f > transform.position.y + posicionCabeza.y)
            {
                player.GetComponent<Rigidbody2D>().velocity = Vector2.up * player.fuerzaDeSalto;
                StartCoroutine(AgitarCamara(0.1f));  
                vidas=0;
                Morir();
            }
            else
            {
                player.RecibirDaño(-(player.transform.position - transform.position).normalized);
            }
        }else if(other.gameObject.CompareTag("Player") && gameObject.CompareTag("Plataforma"))
        {
            if(player.transform.position.y - 0.7f > transform.position.y)
            {
                player.transform.parent = transform;
            }
        }
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        if(other.gameObject.CompareTag("Player") && gameObject.CompareTag("Plataforma"))
        {
            player.transform.parent = null;
        }
    }

    private void CambiarEscalaEnemigo()
    {
        if(direccion.x < 0 && transform.localScale.x > 0)
        {
            transform.localScale = new Vector3(-transform.localScale.x,transform.localScale.y,transform.localScale.z);
        }
        else if(direccion.x > 0 && transform.localScale.x < 0)
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x),transform.localScale.y,transform.localScale.z);
        }
    }

    private void MovimientoWaypoints()
    {
        direccion = (puntos[indiceActual].position - transform.position).normalized;
        if(!esperando)
        {
            transform.position = (Vector2.MoveTowards(transform.position,puntos[indiceActual].position,velocidadDesplazamiento * Time.deltaTime));
        }

        if(Vector2.Distance(transform.position,puntos[indiceActual].position) <= 0.7f)
        {
            if(!esperando)
                StartCoroutine(Espera());
        }

    }

    private IEnumerator Espera()
    {
        esperando = true;
        yield return new WaitForSeconds(tiempoEspera);
        esperando = false;
        indiceActual++;
        if(indiceActual >= puntos.Count)
            indiceActual = 0;
    }

    public void RecibirDaño()
    {
        if(vidas > 1)
        {
            StartCoroutine(AgitarCamara(0.1f));
            StartCoroutine(EfectoDaño());
            if(transform.parent.name == "Spider")
                aplicarFuerza = true;
        }
        else
        {
            if(transform.parent.name == "Spider")
                aplicarFuerza = true;
            velocidadDesplazamiento = 0;
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

    private void Morir()
    {
        if(GameObject.Find("Espia")!=null)
            ef.sumarInformacion("muerte");
            
        audioSource.clip = audioClip;
        audioSource.loop = false;
        audioSource.Play();
        esperando = true;
        anim.SetBool("die",true);
        gameObject.tag = "Untagged";
        Physics2D.IgnoreCollision(GetComponent<Collider2D>(),player.GetComponent<Collider2D>(),true);
        Destroy(gameObject, 2.0f);
    }
}
