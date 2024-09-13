using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Interactuable : MonoBehaviour
{
    private bool puedeInteractuar;
    private BoxCollider2D bc;
    private SpriteRenderer sp;
    private GameObject indicadorInteractuable;
    private Animator anim;
    private AudioSource audioSource;

    
    public UnityEvent evento;

    [Header("Palanca")]
    public bool esPalanca;
    public bool palancaAccionada;
    [Header("Cofre")]
    public GameObject[] objetos;
    public bool esCofre;
    [Header("Checkpoint")]
    public bool esCheckpoint;
    [Header("Selector")]
    public bool esSelector;
    [Header("Final")]
    public bool esFinal;

    private void Awake()
    {
        bc = GetComponent<BoxCollider2D>();
        sp = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        if(transform.GetChild(0)!=null)
            indicadorInteractuable = transform.GetChild(0).gameObject;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
       if(other.CompareTag("Player"))
       {
            puedeInteractuar = true;
            indicadorInteractuable.SetActive(true);
            if(esCheckpoint || esSelector ) 
            {
                audioSource.Play();
            }
       }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
       if(other.CompareTag("Player"))
       {
            puedeInteractuar = false;
            indicadorInteractuable.SetActive(false);
       }
    }
    private void Palanca()
    {
        if(esPalanca && !palancaAccionada)
        {
            anim.SetBool("activar",true);
            palancaAccionada = true;
            evento.Invoke();
            indicadorInteractuable.SetActive(false);
            bc.enabled = false;
            this.enabled = false;
            GetComponent<AudioSource>().Play();
        }

    }

    private void Cofre()
    {
        if(esCofre)
        {
            Instantiate(objetos[Random.Range(0,objetos.Length)], transform.position, Quaternion.identity);
            anim.SetBool("abrir",true);
            bc.enabled = false;
        }
    }

    private void Checkpoint()
    {
       if(esCheckpoint) 
       {
            evento.Invoke();
       }
    }

    private void Finalizar()
    {
       if(esFinal) 
       {
            //transform.GetChild(1).GetComponent<Animator>().SetBool("sacar",true);
            PlayerPrefs.DeleteKey("x");
            PlayerPrefs.DeleteKey("y");
            PlayerPrefs.SetInt("nivel",0);
            PlayerPrefs.SetInt("nivelMax",0);
            PlayerPrefs.SetInt("indiceNivelInicio",0);
            PlayerPrefs.DeleteKey("PrimerJefe");
            PlayerPrefs.DeleteKey("SegundoJefe");
            StartCoroutine("CambioFinal");
       }
    }

    private void SeleccionadoNivel()
    {
       if(esSelector) 
       {
            evento.Invoke();
       }
    }
    private void Update()
    {
        if(puedeInteractuar && Input.GetKeyDown(KeyCode.V))
        {
            Cofre();
            Palanca();
            Checkpoint();
            SeleccionadoNivel();
            Finalizar();
        }
    }

    private IEnumerator CambioFinal()
    {
        yield return new WaitForSeconds(0.0f);
        evento.Invoke();
    }
}
