using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EspiaFunction : MonoBehaviour
{
    [Header("Niveles finales")]
    public List<GameObject> listaFinales = new List<GameObject>();
    [Header("Niveles a tener en cuenta")]
    public List<GameObject> listaNiveles = new List<GameObject>();
    [Header("Número enemigos")]
    public List<int> nivelEnemigos = new List<int>();
    [Header("Número NPC")]
    public List<int> nivelNPC = new List<int>();
    [Header("Número medidas")]
    public List<float> nivelTam = new List<float>();
    [Header("Tiempo esperado")]
    public float tiempo = 0.0f;

    [Header(" ")]
    [Header(" ")]
    public float tiempoTranscurrido = 0;
    public int nMuertes = 0;
    public float nDistancia = 0;
    public int nCharlas = 0;
    public string nNiveles = "";

    private Rigidbody2D jugador;

    void Start()
    {
        int nivelActual = PlayerPrefs.GetInt("nivel");
        //Debug.Log("Estas en el nivel " + nivelActual);
        jugador = GameObject.Find("Player").GetComponent<Rigidbody2D>();
        nMuertes = PlayerPrefs.GetInt("muertes",0);
        nDistancia = PlayerPrefs.GetFloat("distanciaRecorrida",0);
        nCharlas =  PlayerPrefs.GetInt("charlas");
        nNiveles = PlayerPrefs.GetString("NivelesSuperados","");
        tiempoTranscurrido = PlayerPrefs.GetFloat("tiempoJugado",0.0f);
        for(int i= 0 ; i < listaNiveles.Count;i++)
        {
            if(GetChildWithName(listaNiveles[i],"Enemigos")!=null)
                nivelEnemigos.Add(GetChildWithName(listaNiveles[i],"Enemigos").transform.childCount);
            else    
                nivelEnemigos.Add(0); 

            if(GetChildWithName(listaNiveles[i],"NPC")!=null)
                nivelNPC.Add(GetChildWithName(listaNiveles[i],"NPC").transform.childCount);
            else    
                nivelNPC.Add(0);
            
            Vector3 tamanyo = GetChildWithName(GetChildWithName(listaNiveles[i],"Grid"),"Terreno").GetComponent<Renderer>().bounds.size;
            nivelTam.Add(tamanyo.x * tamanyo.y);
        }
        CalcularFinal();
    }

    private void Update() 
    {
        
        if(PlayerPrefs.GetInt("nivel",0) < 6)
        {
            nDistancia += jugador.velocity.magnitude * Time.deltaTime;
            tiempoTranscurrido += Time.deltaTime;
        }
    }

    public void GuardarDatosEspia() 
    {
        nDistancia += jugador.velocity.magnitude * Time.deltaTime;
        PlayerPrefs.SetInt("muertes",nMuertes);
        PlayerPrefs.SetInt("charlas",nCharlas);
        PlayerPrefs.SetFloat("distanciaRecorrida",nDistancia);
        PlayerPrefs.SetString("NivelesSuperados",nNiveles);
        PlayerPrefs.SetFloat("tiempoJugado",tiempoTranscurrido);
    }

    public void CalcularFinal()
    {
        float finalMuerte = 0;
        float finalLogro = tiempo/tiempoTranscurrido;
        float finalSocial= 0;
        float finalExplorador= nivelTam[0] + nivelTam[1] + nivelTam[2] + nivelTam[3] + nivelTam[4] + nivelTam[5];
        
        for(int i=0;i < nNiveles.Length;i++)
        {
            //Debug.Log("Estamos calculando el nivel " + int.Parse(""+nNiveles[i]));
            //Debug.Log("Estamos calculando el nivel " + nNiveles[i]);
            finalMuerte+=(nivelEnemigos[int.Parse(""+nNiveles[i])]);
            finalSocial+=(nivelNPC[int.Parse(""+nNiveles[i])]);
        }

        finalMuerte = nMuertes / finalMuerte;
        finalSocial = nCharlas / finalSocial;
        finalExplorador= nDistancia / finalExplorador;

        /*Debug.Log("finalLogro ="+ finalLogro);
        Debug.Log("finalMuerte ="+ finalMuerte);
        Debug.Log("finalSocial ="+ finalSocial);
        Debug.Log("finalExplorador =" + finalExplorador);*/

        if((finalMuerte > finalLogro)&&(finalMuerte > finalSocial)&&(finalMuerte > finalExplorador))
            listaFinales[1].SetActive(true);
        else if((finalLogro > finalSocial)&&(finalLogro > finalExplorador))
            listaFinales[0].SetActive(true);
        else if ((finalExplorador > finalSocial))
            listaFinales[3].SetActive(true);
        else
            listaFinales[2].SetActive(true);

    }

    public void LimpiarDatosEspia() 
    {
        PlayerPrefs.DeleteKey("muertes");
        PlayerPrefs.DeleteKey("charlas");
        PlayerPrefs.DeleteKey("distanciaRecorrida");
        PlayerPrefs.DeleteKey("NivelesSuperados");
        PlayerPrefs.DeleteKey("tiempoJugado");
        PlayerPrefs.DeleteKey("nivelMax");
    }

    public void sumarInformacion(string tipo)
    {
        switch(tipo)
        {
            case "muerte":
            nMuertes++;
            break;
            case "npc":
            nCharlas++;
            break;
            case "nivel":
            nNiveles+=PlayerPrefs.GetInt("nivel");
            Debug.Log(PlayerPrefs.GetInt("nivel"));
            break;
        }

        
    }

     GameObject GetChildWithName(GameObject obj, string name) 
     {
        Transform trans = obj.transform;
        Transform childTrans = trans. Find(name);
        if (childTrans != null) 
        {
            return childTrans.gameObject;
        } else 
        {
            return null;
        }
    }

    public void MostrarTiempo (int i) 
    {
        Debug.Log("Nivel "+i+": "+tiempoTranscurrido);
        GuardarDatosEspia();
    }
}
