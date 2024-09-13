using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisparoProyectiles : MonoBehaviour
{
    [SerializeField] Transform  puntoDisparo;
    [SerializeField] GameObject  proyectil;
    public int numProyectiles = 5;
    public float tiempoVidaProyectil = 2;
    public float ancho=70;
    public float angulo=30;

    // Update is called once per frame
    public void Disparar()
    {
        //angulo=-ancho/2;
        float parte=ancho/(numProyectiles);
        float radioexp=18 - 5*2;
        for(int i=0;i<numProyectiles;i++)
        {
            GameObject go = Instantiate(proyectil, puntoDisparo.position, Quaternion.identity);
            Destroy(go, tiempoVidaProyectil);
            if(transform.parent.transform.localScale.x >= 0)
                go.GetComponent<Rigidbody2D>().velocity = new Vector2(Mathf.Cos(angulo * Mathf.Deg2Rad) , Mathf.Sin(angulo * Mathf.Deg2Rad)) * radioexp;
            else
                go.GetComponent<Rigidbody2D>().velocity = new Vector2(-Mathf.Cos(angulo * Mathf.Deg2Rad) , Mathf.Sin(angulo * Mathf.Deg2Rad)) * radioexp;
            go.transform.parent = null;
            angulo=(angulo+parte)%(ancho/2);
            radioexp+= 2;
        }
    }
}
