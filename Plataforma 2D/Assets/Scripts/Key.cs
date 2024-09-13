using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Key : MonoBehaviour
{
    private Animator anim;
    [SerializeField] UnityEvent evento;
    public GameObject efectoLlave;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }
     private void OnTriggerEnter2D(Collider2D other)
    {
       if(other.CompareTag("Player"))
       {
            evento.Invoke();
            GetComponent<AudioSource>().Play();
            GetComponent<BoxCollider2D>().enabled = false;
            GetComponent<SpriteRenderer>().enabled = false;
            Destroy(Instantiate(efectoLlave, transform.position,Quaternion.identity),0.65f);
            Destroy(gameObject,2);
       }
    }
}
