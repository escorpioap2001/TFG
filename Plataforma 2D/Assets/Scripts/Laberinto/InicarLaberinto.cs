using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InicarLaberinto : MonoBehaviour
{
    public List<GameObject> laberintos;

    void Start()
    {
        int numero = Random.Range(0,laberintos.Count);
        Instantiate(laberintos[numero],this.transform);
        Debug.Log("Se creo el laberinto");
        
    }
}
