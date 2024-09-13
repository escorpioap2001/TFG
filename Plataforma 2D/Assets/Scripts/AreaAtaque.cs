using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaAtaque : MonoBehaviour
{
    [SerializeField] AudioClip audioClip;
    private AudioSource audioSource;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Enemigo"))
        {
            audioSource = collision.GetComponent<AudioSource>();
            if(collision.name == "Bat")
            {
                collision.GetComponent<BatControl>().RecibirDaño();
            }
            else if(collision.name == "Wraith")
            {
                collision.GetComponent<SpiritControler>().RecibirDaño();
            }
            else if(collision.name == "Squeleton")
            {
                collision.GetComponent<SqueletonControl>().RecibirDaño();
                audioSource.Play();
                return;
            }
            else if(collision.name == "Spider")
            {
                collision.GetComponent<Waypoints>().RecibirDaño();
            }else if(collision.name == "Golem")
            {
                collision.GetComponent<GolemControler>().RecibirDaño();
            }
            else if(collision.name == "SqueletonBoss")
            {
                collision.transform.parent.GetComponent<Jefe2AtaqController>().RecibirDaño();
                return;
            }
            else if(collision.name == "Big_bloated")
            {
                collision.transform.parent.GetComponent<Jefe3Controler>().RecibirDaño();
                return;
            }
            audioSource.PlayOneShot(audioClip); 
            
        }
        else if(collision.CompareTag("Destruible"))
        {
            collision.GetComponent<Animator>().SetBool("destruir",true);
            collision.GetComponent<AudioSource>().Play();
            collision.tag ="Untagged";
        }
    }
}
