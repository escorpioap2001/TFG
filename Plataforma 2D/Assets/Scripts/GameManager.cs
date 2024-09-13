using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using Cinemachine;
using UnityEngine.Rendering.PostProcessing;

public class GameManager : MonoBehaviour
{
    private bool ejecutando;
    private int indiceNivelInicio;
    private static bool salioTrofeos;
    private int indicePosSalida = 2;
    private static int numSalidas = 2;

    public static GameManager instance;

    [Header("UI")]
    public GameObject vidasUI;
    public TMP_Text textMonedas;
    public PlayerControler player;
    public int monedas;
    public GameObject efectoMoneda;
    public TMP_Text guardaPartidaTexto;

    [Header("Audio")]
    public Slider AudioSlider;
    public Slider SFXSlider;
    [SerializeField] AudioMixer audioMixer;

    [Header("Brillo")]
    public Slider BrightSlider;
    [SerializeField] PostProcessProfile PostProf;
    public PostProcessLayer layer;
    AutoExposure exposure;

    [Header("Tamaño")]
    [SerializeField] Button buttonScreen;
    public int fullScreen = 0;

    [Header("Daltonismo")]
    [SerializeField] TMP_Dropdown dropDaltonismo;


    [Header("Paneles")]
    public GameObject panelPausa;
    public GameObject panelGameOver;
    public GameObject panelCarga;

    public CinemachineConfiner cinemachineConfiner;
    [Header("Niveles")]
    public bool avanzandoNivel;
    public int nivelActual;
    public List<Transform> posicionesAvance = new List<Transform>();
    public List<Transform> posicionesRetroceder = new List<Transform>();
    public List<Collider2D> areaCamara = new List<Collider2D>();
    public GameObject panelTransicion;

    [Header("Trofeos")]
    public List<GameObject> objetosTrofeos = new List<GameObject>();

    private void Awake()
    {
        instance=this;
    
        if(SceneManager.GetActiveScene().name == "Level1") //cambiar nombre escena niveles
        {
            //Debug.Log("Salio al Level1");
            ComprobarUI();
            CargarPartida();
        }
        else if(SceneManager.GetActiveScene().name == "Game")
        {
            ComprobarUI();
            PosicionInicialJugador(indiceNivelInicio);
        }else if(SceneManager.GetActiveScene().name == "SalaTrofeos")
        {
            ComprobarUI();
            if(PlayerPrefs.HasKey("trofeos"))
            {
                // Busqueda de trofeos como si fueran numeros binarios 1111
                int trofeos = PlayerPrefs.GetInt("trofeos");
                for(int i= 0; trofeos > 0 ;i++)
                {
                    objetosTrofeos[i].SetActive((trofeos & 1) == 1);
                    trofeos >>= 1;
                }
            }
        }else if(SceneManager.GetActiveScene().name == "MainMenu")
        {
            if(PlayerPrefs.HasKey("trofeos"))
            {
                // Busqueda de trofeos como si fueran numeros binarios 1111
                int trofeos = PlayerPrefs.GetInt("trofeos");
                for(int i= objetosTrofeos.Count - 1; trofeos > 0 ;i--)
                {
                    objetosTrofeos[i].SetActive((trofeos & 1) == 1);
                    trofeos >>= 1;
                }
            }
        }
    }

    public void ActivarPanelTransicion()
    {
        panelTransicion.SetActive(true);
        panelTransicion.GetComponent<Animator>().SetTrigger("ocultar");
        
}

    public void ActivarTrofeo(int Trofeo)
    {
        panelTransicion.SetActive(true);
        int numTrofeos = 0;
        int nuevosTrofeos = 0;

        if(PlayerPrefs.HasKey("trofeos"))
        {
            numTrofeos = PlayerPrefs.GetInt("trofeos");
        }
        PlayerPrefs.SetInt("trofeos_actual",Trofeo);
        nuevosTrofeos = numTrofeos;
        numTrofeos>>=Trofeo;
        if((numTrofeos & 1) != 1)
            nuevosTrofeos += (int) Mathf.Pow(2,Trofeo);
        PlayerPrefs.SetInt("trofeos", nuevosTrofeos); 
    }

    private void PosicionInicialJugador(int indiceNivelActual)
    {
        if(salioTrofeos)
        {
            Debug.Log("Entro:");
            player.transform.position = posicionesAvance[indicePosSalida].transform.position;
            if(numSalidas == 1)
                salioTrofeos = false;
            else
                numSalidas--;
        }
        else
        {
            player.transform.position = posicionesAvance[indiceNivelActual].transform.position;
        }
    }

    public void SetIndiceNivelInicio(int indice)
    {
        indiceNivelInicio = indice;
        PlayerPrefs.SetInt("indiceNivelInicio",indiceNivelInicio);
        nivelActual = indice;
        PlayerPrefs.SetInt("nivel",nivelActual);
    }

    public void CambiarPosicionJugador()
    {
        if(avanzandoNivel)
        {
            player.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            if(posicionesAvance.Count > nivelActual + 1)
            {
                cinemachineConfiner.m_BoundingShape2D = areaCamara[nivelActual + 1];
                player.transform.position = posicionesAvance[nivelActual + 1].transform.position;
                player.terminandoMapa = false;
                player.GetComponent<Animator>().SetBool("caminar",false);
                if(PlayerPrefs.GetInt("nivelMax",0) < nivelActual + 1)
                    PlayerPrefs.SetInt("nivelMax",nivelActual + 1);
            }
        }
        else
        {
            if(posicionesRetroceder.Count > nivelActual - 1)
            {
                cinemachineConfiner.m_BoundingShape2D = areaCamara[nivelActual - 1];
                player.transform.position = posicionesRetroceder[nivelActual - 1].transform.position;
                player.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                player.terminandoMapa = false;
                player.GetComponent<Animator>().SetBool("caminar",false);
            }
        }
        player.vidas = PlayerPrefs.GetInt("vidas");
        int vidasADescartar = 3 - player.vidas;
        player.MostrarVidasUI();
        player.ActualizarVidasUI(vidasADescartar);
    }

    public void GuardarPartida()
    {
        float x,y;
        x = player.transform.position.x;
        y = player.transform.position.y;

        int vidas = player.vidas;
        int nivel = nivelActual;

        PlayerPrefs.SetInt("monedas",monedas);
        PlayerPrefs.SetInt("vidas",vidas);
        PlayerPrefs.SetFloat("x",x);
        PlayerPrefs.SetFloat("y",y);
        PlayerPrefs.SetInt("nivel",nivel);
        PlayerPrefs.SetInt("indiceNivelInicio",indiceNivelInicio);
       

        if(!ejecutando)
        {
            StartCoroutine(mostrarTextoGuardado());
        }
    }

    public void GuardarPartidaPasada(int num)
    {
        float x,y;
        x = player.transform.position.x;
        y = player.transform.position.y;

        int nivel = nivelActual;

        if(((nivelActual==3 && num < 0)||(nivelActual==1 && num > 0)||(nivelActual== 4 && num > 0)) && (PlayerPrefs.GetInt("SegundoJefe")!=0 || PlayerPrefs.GetInt("PrimerJefe")!=0))
        {
            num = num * 2;
        }

        PlayerPrefs.SetInt("monedas",monedas);
        PlayerPrefs.SetInt("vidas",3);
        PlayerPrefs.DeleteKey("x");
        PlayerPrefs.DeleteKey("y");
        PlayerPrefs.SetInt("nivel",nivel + num);
        PlayerPrefs.SetInt("indiceNivelInicio",indiceNivelInicio + num);
        
        indiceNivelInicio += num;
        nivel += num;
    }

    private IEnumerator mostrarTextoGuardado()
    {
        ejecutando = true;
        guardaPartidaTexto.gameObject.SetActive(true);
        yield return new WaitForSeconds(1);
        guardaPartidaTexto.gameObject.SetActive(false);
        ejecutando = false;
    }

    public void CargarNivel(string nombreNivel)
    {
        SceneManager.LoadScene(nombreNivel);
    }

    public void CargarPartida()
    {
        
        indiceNivelInicio = PlayerPrefs.GetInt("indiceNivelInicio");
        nivelActual = PlayerPrefs.GetInt("nivel");
        cinemachineConfiner.m_BoundingShape2D = areaCamara[nivelActual];
        if(PlayerPrefs.HasKey("x"))
            player.transform.position = new Vector2(PlayerPrefs.GetFloat("x"),PlayerPrefs.GetFloat("y"));
            
        
        else
            PosicionInicialJugador(indiceNivelInicio);

        monedas = PlayerPrefs.GetInt("monedas");
        textMonedas.text = monedas.ToString();

        if(PlayerPrefs.HasKey("vidas"))
        {
            player.vidas = PlayerPrefs.GetInt("vidas");
            int vidasADescartar = 3 - player.vidas;
            player.MostrarVidasUI();
            player.ActualizarVidasUI(vidasADescartar);
        }
        else
        {
            player.MostrarVidasUI();
            player.ActualizarVidasUI(0);
        }
    }

    public void ActualizarContadorMonedas()
    {
        monedas++;
        textMonedas.text = monedas.ToString();
    }

    public void EfectoMoneda(Transform pos)
    {
        GameObject efecto = GameObject.Instantiate(efectoMoneda,pos.position,Quaternion.identity);
        Destroy(efecto,0.75f);
    }

    public void PausarJuego()
    {
        Time.timeScale = 0;
        panelPausa.SetActive(true);
    }

    public void DespausarJuego()
    {
        Time.timeScale = 1;
        panelPausa.SetActive(false);
    }

    public void VolverAlMenu()
    {
        Time.timeScale = 1;
        PlayerPrefs.SetInt("vidas",3);
        PlayerPrefs.DeleteKey("x");
        PlayerPrefs.DeleteKey("y");
        SceneManager.LoadScene("MainMenu");
    }

    public void CargarSelector()
    {
        Time.timeScale = 1;
        PlayerPrefs.SetInt("vidas",3);
        PlayerPrefs.DeleteKey("x");
        PlayerPrefs.DeleteKey("y");
        SceneManager.LoadScene("Game");
    }

    public void CargarEscena(string escenaCargar)
    {
        StartCoroutine(CargarEscenaCorrutine(escenaCargar));
    }

    public IEnumerator CargarEscenaCorrutine(string escenaCargar)
    {

        if(SceneManager.GetActiveScene().name == "SalaTrofeos")
        {
            salioTrofeos = true;
            numSalidas = 2;
            indiceNivelInicio = 2;
        }

        Time.timeScale = 1;
        SceneManager.LoadScene(escenaCargar);

        panelCarga.SetActive(true);

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(escenaCargar);

        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        PosicionInicialJugador(indiceNivelInicio);
        
    }

    public void GameOver()
    {
        panelGameOver.SetActive(true);
        
    }

    public void ContinuarJuego()
    {
        Time.timeScale = 1;
        player.enabled = true;
        CargarEscena(SceneManager.GetActiveScene().name);
        panelGameOver.SetActive(false);
   
    }

    public void SalirJuego()
    {
        Application.Quit();
    }

    public void CargarEscenaSelector()
    {
        StartCoroutine(CargarEscena());
    }

    private IEnumerator CargarEscena()
    {
        panelCarga.SetActive(true);

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("Game");

        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }

    public void SetBakgroundVolume()
    {
        audioMixer.SetFloat("background",Mathf.Log10(AudioSlider.value)*20);
        PlayerPrefs.SetFloat("volume", AudioSlider.value);
    }

    public void SetSFXVolume()
    {
        audioMixer.SetFloat("sfx",Mathf.Log10(SFXSlider.value)*20);
        PlayerPrefs.SetFloat("volumeSFX", SFXSlider.value);
    }

    public void SetBrightness()
    {
        float value = BrightSlider.value;
        exposure.keyValue.value = value;
        PlayerPrefs.SetFloat("brillo",value);
    }

    public void SetScreenSize()
    {
        fullScreen = (fullScreen + 1) % 2 ;
        PlayerPrefs.SetInt("screen",fullScreen);
        if(fullScreen != 0)
        {
            buttonScreen.image.color = Color.gray;
            Screen.fullScreen = true;
        }
        else
        {
            buttonScreen.image.color = Color.white;
            Screen.fullScreen = false;
        }
    }

    public void CambiarDaltonismo()
    {
        Camera.main.GetComponent<Wilberforce.Colorblind>().Type = dropDaltonismo.value;
        Camera.main.transform.GetChild(0).GetComponent<Wilberforce.Colorblind>().Type = dropDaltonismo.value;
        PlayerPrefs.SetInt("daltonismo", dropDaltonismo.value);
    }


    private void ComprobarUI()
    {
        if(audioMixer != null)
        {
            if(!PlayerPrefs.HasKey("volume"))
            {
                AudioSlider.value = 1;
                audioMixer.SetFloat("background",Mathf.Log10(AudioSlider.value)*20);
                
            }
            else
            {
                AudioSlider.value = PlayerPrefs.GetFloat("volume");
                audioMixer.SetFloat("background",Mathf.Log10(AudioSlider.value)*20);
                //Debug.Log("Volumen general"+PlayerPrefs.GetFloat("volume"));
            }

            if(!PlayerPrefs.HasKey("volumeSFX"))
            {
                SFXSlider.value = 1;
                audioMixer.SetFloat("sfx",Mathf.Log10(SFXSlider.value)*20);
            }
            else
            {
                SFXSlider.value = PlayerPrefs.GetFloat("volumeSFX");
                audioMixer.SetFloat("sfx",Mathf.Log10(SFXSlider.value)*20);
                //Debug.Log("Volumen efectos"+PlayerPrefs.GetFloat("sfx"));
            }
        }

        if(BrightSlider != null)
        {
            PostProf.TryGetSettings(out exposure);
            if (!PlayerPrefs.HasKey("brillo"))
            {
                BrightSlider.value = 1;
                exposure.keyValue.value = BrightSlider.value;
            }
            else
            {
                BrightSlider.value = PlayerPrefs.GetFloat("brillo");
                exposure.keyValue.value = BrightSlider.value;
                //Debug.Log("Brillo"+PlayerPrefs.GetFloat("brillo"));
            }
        }

        if(buttonScreen != null)
        {
            if(PlayerPrefs.GetInt("screen") != 0)
            {
                buttonScreen.image.color = Color.gray;
                Screen.fullScreen = true;
                fullScreen = 1;
            }
            else
            {
                buttonScreen.image.color = Color.white;
                Screen.fullScreen = false;
                fullScreen = 0;
                //Debug.Log("Pantalla completa"+PlayerPrefs.GetFloat("screen"));
            }
        }
        PlayerPrefs.SetInt("daltonismo", 0); 
    }

    
}
