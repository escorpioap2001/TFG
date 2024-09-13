using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Dialogos : MonoBehaviour
{
    public bool enDialogo;
    public bool detectando;

    public List<string> dialogo = new List<string>();
    public Text textDialogos;
    public PlayerControler playerController;
    public float tiempoEntreTextos;
    public GameObject iconoDialogo;

    public Image imagenCaraDialogo;
    public Sprite imagenCara;

    private Animator anim;
    private AudioSource audioSource;
    private EspiaFunction ef;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }
    private void Start() 
    {
        iconoDialogo.SetActive (false);
        detectando = false;
        if(GameObject.Find("Espia")!=null)
            ef =  GameObject.Find("Espia").GetComponent<EspiaFunction>();
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if(col.gameObject.CompareTag("Player"))
        {
            detectando = true;
            iconoDialogo.SetActive (true);
            if(this.gameObject.name == "Forger")
            {
                anim.SetBool("esperando", true);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        if(col.gameObject.CompareTag("Player"))
        {
            detectando = false;
            iconoDialogo.SetActive (false);
            if(this.gameObject.name == "Forger")
            {
                anim.SetBool("esperando", false);
            }
        }
    }

    private void Update ()
    {
        if (detectando)
        {
            if (Input.GetKeyDown(KeyCode.V) && !enDialogo)
            {
                imagenCaraDialogo.sprite = imagenCara;
                textDialogos.transform.parent.gameObject.SetActive(true);
                enDialogo = true;
                playerController.PararTodo();
                playerController.enabled = false;
                audioSource.PlayOneShot(audioSource.clip);
                if(GameObject.Find("Espia")!=null)
                    ef.sumarInformacion("npc");
                StartCoroutine(Dialogar());
            }
        }
    }
 

    public bool GetEnDialogo()
    {
        return enDialogo;
    }

    private IEnumerator Dialogar()
    {
        for(int i = 0; i < dialogo.Count; i++)
        {   
            if(i!=0)
                audioSource.Play();
                
            char[] textoActual = dialogo[i].ToCharArray();
            for (int j = 0; j < textoActual.Length; j++)
            {
                textDialogos.text += textoActual[j];
                if (Input.GetKeyDown(KeyCode.V))
                {
                    if(i!=0)
                        audioSource.Stop();
                    textDialogos.text = dialogo[i];
                    j = textoActual.Length - 1;
                    yield return null;
                }
                else
                {
                    yield return new WaitForSeconds(tiempoEntreTextos);
                }
                if(j == textoActual.Length - 1 && i!=0)
                    audioSource.Stop();
            }

            while (!Input.GetKeyDown(KeyCode.V))
            {
                yield return null;
            }
            audioSource.Stop();
            textDialogos.text = string.Empty;
            yield return null;
        }
        enDialogo = false;
        textDialogos.transform.parent.gameObject.SetActive(false);
        playerController.enabled = true;
    }
}
