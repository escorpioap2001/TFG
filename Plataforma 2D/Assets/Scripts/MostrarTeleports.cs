using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MostrarTeleports : MonoBehaviour
{
    public List<GameObject> teleports = new List<GameObject>();

    private void Awake()
    {
        if(PlayerPrefs.HasKey("nivelMax"))
        {
            int niveles = PlayerPrefs.GetInt("nivelMax") - 1;
            if(niveles >= 0)
            {
                for(int i=0; i<= niveles; i++)
                {
                    teleports[i].SetActive(true);
                }
            }
        }
    }
}
