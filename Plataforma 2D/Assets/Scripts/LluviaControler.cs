using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LluviaControler : MonoBehaviour
{
    private Transform p1;
    private Transform p2;
    private int gotasSpawneadas = 0;

    public GameObject gota;
    public int numGotas;
    public int MaxDelayGotas = 2;
    public float tiempoEntreGotas = 3;
    public float tiempVidaGota = 3;
    public bool pausado = false;

    // Start is called before the first frame update
    void Start()
    {
        p1 = transform.GetChild(0).transform;
        p2 = transform.GetChild(1).transform;
    }

    void Update()
    {
        if(gotasSpawneadas <= numGotas && !pausado)
        {
            StartCoroutine("CrearUnaGota");
        }
    }

    private IEnumerator CrearUnaGota()
    {
        gotasSpawneadas++;
        yield return new WaitForSeconds(Random.Range(1,MaxDelayGotas));
        Destroy(GameObject.Instantiate(gota,new Vector3(Random.Range(p1.position.x,p2.position.x),(p1.position.y + p2.position.y)/2,0),Quaternion.identity),tiempVidaGota);
        yield return new WaitForSeconds(tiempoEntreGotas);
        gotasSpawneadas--;
    }

    public void PausarLLuvia()
    {
        pausado = true;
    }

    public void DespausarLLuvia()
    {
        pausado = false;
    }
}
