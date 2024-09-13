using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public float tiempoInmortalidad = 10;

    private void OnTriggerEnter2D(Collider2D other) 
    {
        if(other.CompareTag("Player"))
        {
            AsignarItem();
        }
    }

    private void AsignarItem()
    {
        if(gameObject.CompareTag("Moneda"))
        {
            GameManager.instance.ActualizarContadorMonedas();
            GameManager.instance.EfectoMoneda(transform);
        }
        else if(gameObject.CompareTag("PowerUp"))
        {
            GameManager.instance.player.DarInmortalidadTemporal(tiempoInmortalidad);
        }
        GetComponent<AudioSource>().Play();
        GetComponent<BoxCollider2D>().enabled = false;
        GetComponent<SpriteRenderer>().enabled = false;
        Destroy(gameObject, 2);

    }
}
