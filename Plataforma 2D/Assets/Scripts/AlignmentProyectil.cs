using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlignmentProyectil : MonoBehaviour
{
    private Vector2 direccionProyectil;
    public float angulo;
    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        direccionProyectil = new Vector2(rb.velocity.x,rb.velocity.y).normalized;

        angulo = Mathf.Atan2(direccionProyectil.y, direccionProyectil.x) *  Mathf.Rad2Deg;

        transform.localEulerAngles = new Vector3(transform.localEulerAngles.y,transform.localEulerAngles.x,angulo);
    }
}
