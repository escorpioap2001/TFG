using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Caida : MonoBehaviour
{
    public GameObject checkpoint;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            other.transform.position = checkpoint.transform.position;
            other.GetComponent<PlayerControler>().RecibirDaño();
        }
        
    }
}
