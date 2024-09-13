using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plataforma : MonoBehaviour
{
    private bool aplicarFuerza;
    //private bool detectaJugador;
    private PlayerControler player;
    public float fuerzaSalto = 25;
    public BoxCollider2D plataformaCollider;
    public BoxCollider2D plataformaTrigger;

    public bool daSalto;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControler>();
    }

    private void Start() 
    {
        if(!daSalto)
            Physics2D.IgnoreCollision(plataformaCollider,plataformaTrigger,true);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            if(!daSalto)
                Physics2D.IgnoreCollision(plataformaCollider,player.GetComponent<CapsuleCollider2D>(), true);
        }
        
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            if(!daSalto)
                Physics2D.IgnoreCollision(plataformaCollider,player.GetComponent<CapsuleCollider2D>(), false);
        }
        
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            //detectaJugador = true;
            if(daSalto)
            {
                aplicarFuerza = true;
            }
        }
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            //detectaJugador = false;
        }
    }

    private void FixedUpdate() 
    {
        if(aplicarFuerza)
        {
            player.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            player.GetComponent<Rigidbody2D>().AddForce(Vector2.up * fuerzaSalto, ForceMode2D.Impulse);
            aplicarFuerza = false;
        }
    }

    private void Update()
    {
        if(daSalto)
        {
            if(transform.position.y < player.transform.position.y - 0.8f)
            {
                plataformaCollider.isTrigger = false;
            }
            else
            {
                plataformaCollider.isTrigger = true;
            }
        }
    }
}
