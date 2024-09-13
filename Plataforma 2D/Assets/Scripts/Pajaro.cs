using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Pajaro : MonoBehaviour
{
    private bool enSuelo = true;
    private Animator anim;
    private Rigidbody2D rb;
    private PlayerControler player;
    private Coroutine picotear;

    public float velocidadVuelo;
    [SerializeField] float distanciaDeteccionJugador;
    public Vector2  abajo;
    public float radioDeColision;
    public float tiempoEntrePicotazos;
    public LayerMask layerPiso;

    [Header("Audio")]
    private AudioSource audioSource;
    public bool jugadordetectado = false;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControler>();
        Physics2D.IgnoreCollision(GetComponent<Collider2D>(),player.GetComponent<Collider2D>(),true);
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 direccion = (transform.position - player.transform.position).normalized * distanciaDeteccionJugador;
        enSuelo = Physics2D.OverlapCircle((Vector2)transform.position + abajo, radioDeColision, layerPiso);
        float distanciaActual = Vector2.Distance(transform.position,player.transform.position);
        if(distanciaActual <= distanciaDeteccionJugador)
        {
            if(picotear != null)
            {
                StopCoroutine(picotear);
                picotear = null;
            }
            if(!jugadordetectado)
            {
                audioSource.PlayOneShot(audioSource.clip);
                jugadordetectado = true;
            }
            
            Vector2 movimiento = new Vector2(direccion.x,2*Mathf.Abs(direccion.x));
            movimiento = movimiento.normalized;
            rb.velocity = (movimiento * velocidadVuelo);
            anim.SetBool("comer",false);
            anim.SetBool("volar",true);
            CambiarVista(movimiento.x);
        }
        else if(enSuelo)
        {
            anim.SetBool("volar",false);
            if(picotear == null)
            {
                picotear = StartCoroutine("Picotear");
            }
        }
        else
        { 
            jugadordetectado = false;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, distanciaDeteccionJugador);
    }

    IEnumerator Picotear()
    {
        anim.SetBool("comer",true);
        yield return new WaitForSeconds((int) Random.Range(1,3));
        anim.SetBool("comer",false);
        yield return new WaitForSeconds(Random.Range(1,5));
        picotear = null;
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

}
