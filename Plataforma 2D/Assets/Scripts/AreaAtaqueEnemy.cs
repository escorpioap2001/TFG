using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;

public class AreaAtaqueEnemy : MonoBehaviour
{
    public UnityEvent unityEvent;
    public GameObject enemy;
    public bool empujar = true;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            if(empujar)
            {
                if(other.transform.position.x > enemy.transform.position.x)
                {
                    other.GetComponent<PlayerControler>().RecibirDaño(new Vector2(-1,0));
                }
                else
                {
                    other.GetComponent<PlayerControler>().RecibirDaño(new Vector2(1,0));
                }
            }
            else
            {
                other.GetComponent<PlayerControler>().RecibirDaño(new Vector2(0,0));
            }
        }

        if(other.name == "MurosDeStuneo")
        {
            unityEvent.Invoke();
        }
    }
}
