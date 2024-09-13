using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DeteccionFinal : MonoBehaviour
{
    public bool avanzando;
    public UnityEvent unityEvent;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            GameManager.instance.ActivarPanelTransicion();
            GameManager.instance.avanzandoNivel = avanzando;
            unityEvent.Invoke();
            StartCoroutine(EsperarCambioPosicion());
        } 
    }

    private IEnumerator EsperarCambioPosicion()
    {
        yield return new WaitForSeconds(0.2f);
        GameManager.instance.CambiarPosicionJugador();
        if(avanzando)
        {
            if((PlayerPrefs.GetInt("PrimerJefe") != 0 && GameManager.instance.nivelActual == 1)|| (PlayerPrefs.GetInt("SegundoJefe") != 0 && GameManager.instance.nivelActual == 4))
            { 
                GameManager.instance.nivelActual++;
                GameManager.instance.ActivarPanelTransicion();
                GameManager.instance.CambiarPosicionJugador();
                GameManager.instance.nivelActual++;
            }
            else
            {
                GameManager.instance.nivelActual++;
            }
        }
        else
        {
            if((PlayerPrefs.GetInt("PrimerJefe") != 0 && GameManager.instance.nivelActual == 3))
            { 
                GameManager.instance.nivelActual--;
                GameManager.instance.ActivarPanelTransicion();
                GameManager.instance.CambiarPosicionJugador();
                GameManager.instance.nivelActual--;
            }
            else
            {
                GameManager.instance.nivelActual--;
            }
        }
    }
}
