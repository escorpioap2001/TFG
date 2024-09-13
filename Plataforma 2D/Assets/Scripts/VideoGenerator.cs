using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class VideoGenerator : MonoBehaviour
{
    [SerializeField] GameObject FirstPanel;
    public int asesino = 0;
    public UnityEngine.Video.VideoClip videoAsesino;
    public int explorador = 0;
    public UnityEngine.Video.VideoClip videoExplorador;
    public int trofeos = 1;
    public UnityEngine.Video.VideoClip videoTrofeos;
    public int social = 0;
    public UnityEngine.Video.VideoClip videoSocial;

    private UnityEngine.Video.VideoPlayer vp;
    // Start is called before the first frame update
    void Start()
    {
        int trofeo_ganado = PlayerPrefs.GetInt("trofeos_actual");

        vp = FirstPanel.GetComponent<UnityEngine.Video.VideoPlayer>();
        vp.SetDirectAudioVolume(0, PlayerPrefs.GetFloat("volume",1));
        if(trofeo_ganado == 1)
        {
            vp.clip = videoAsesino;
        }
        else if(trofeo_ganado == 3)
        {
            vp.clip = videoExplorador;
        }else if(trofeo_ganado == 0)
        {
            vp.clip = videoTrofeos;
        }else if(trofeo_ganado == 2)
        {
            vp.clip = videoSocial;
        }

        StartCoroutine(CambioAEscenaInicio());
    }

    IEnumerator CambioAEscenaInicio()
    {
        vp.Play();
        vp.SetDirectAudioVolume(1,PlayerPrefs.GetFloat("volume"));
        yield return new WaitForSeconds((float) vp.length);
        SceneManager.LoadScene("Game");  
    }
}
