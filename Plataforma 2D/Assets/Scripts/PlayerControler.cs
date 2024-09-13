using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class PlayerControler : MonoBehaviour
{
    private GameObject ultimoEnemigo;
    private int direccionXfinal;

    private Rigidbody2D rb;
    private Vector2 direccion;
    private Animator anim;
    private CinemachineVirtualCamera cm;
    private Vector2 direccionMovimiento;
    private bool bloqueado;
    private GrayCamera gc;
    private Vector2 direccionDaño;
    private SpriteRenderer sp;

    //Variables de agacharse
    private float velocidadDeMovimientoAuxiliar;
    private CapsuleCollider2D ccollider;

    [Header("Estadísticas")]
    public float fuerzaDeSalto = 10;
    public float velocidadDeMovimiento = 10;
    public float velocidadDash = 20;
    public float velocidadDeslizar;
    public int vidas = 3;
    public float tiempoInmortalidad = 2;

    [Header("Colisiones")]
    public LayerMask layerPiso;
    public LayerMask layerPared;
    public LayerMask layerPlataforma;
    public Vector2  abajo,izquierda,derecha;
    public float radioDeColision;
    public PhysicsMaterial2D pm2;

    [Header("Booleanos")]
    public bool puedeMover = true;
    public bool enSuelo = true;
    public bool puedeDash;
    public bool haciendoDash=false;
    public bool tocadoPiso;
    public bool haciendoShake;
    public bool estaAtacando;
    public bool enMuro;
    public bool muroDerecho;
    public bool muroIzquierdo;
    public bool agarrarse;
    public bool saltardeMuro;
    public bool esInmortal;
    public bool aplicarFuerza;
    public bool terminandoMapa;
    public bool estaAgachado;

    [Header("Sounds")]
    private AudioSource audioSource;
    public AudioClip walkSound;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>(); 
        anim = GetComponent<Animator>();
        cm = GameObject.FindGameObjectWithTag("VirtualCamera").GetComponent<CinemachineVirtualCamera>();
        gc = Camera.main.GetComponent<GrayCamera>();
        sp = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();

        velocidadDeMovimientoAuxiliar = velocidadDeMovimiento;
        ccollider = GetComponent<CapsuleCollider2D>();
    }

    public void SetBloqueadoTrue()
    {
        bloqueado = true;
    }

    public void Morir()
    {
        if(vidas > 0)
            return;
        
        GameManager.instance.GameOver();
        Time.timeScale = 0;
        this.enabled = false;
    }

    public void RecibirDaño()
    {
        StartCoroutine(ImpactoDaño(Vector2.zero));
    }

    public void RecibirDaño(Vector2 direccionDaño)
    {
        StartCoroutine(ImpactoDaño(direccionDaño));
    }

    private IEnumerator ImpactoDaño(Vector2 dir)
    {
        if(!esInmortal)
        {
            DarInmortalidad();
            vidas--;
            gc.enabled = true;
            float velocidadAuxiliar = velocidadDeMovimiento;
            this.direccionDaño = dir;
            aplicarFuerza = true;
            Time.timeScale = 0.4f;
            FindObjectOfType<RippleEffect>().Emit(Camera.main.WorldToViewportPoint(transform.position));
            StartCoroutine(AgitarCamara());
            yield return new WaitForSeconds(0.2f);
            Time.timeScale = 1f;
            gc.enabled = false;

            ActualizarVidasUI(1);

            velocidadDeMovimiento = velocidadAuxiliar;
            Morir();
        }
    }

    public void MostrarVidasUI()
    {
        for(int i=0; i < GameManager.instance.vidasUI.transform.childCount; i++)
        {
            GameManager.instance.vidasUI.transform.GetChild(i).gameObject.SetActive(true);
        }
    }

    public void ActualizarVidasUI(int vidasADescartar)
    {
        int vidasDescontadas = vidasADescartar;
        for(int i = GameManager.instance.vidasUI.transform.childCount -1; i >= 0; i--)
        {
            if(GameManager.instance.vidasUI.transform.GetChild(i).gameObject.activeInHierarchy && vidasDescontadas != 0)
            {
                GameManager.instance.vidasUI.transform.GetChild(i).gameObject.SetActive(false);
                vidasDescontadas--;
            }
            else
            {
                if(vidasDescontadas == 0)
                    break;
            }
        }
    }

    private void FixedUpdate() {
        if(aplicarFuerza)
        {
            velocidadDeMovimiento=0;
            rb.velocity = Vector2.zero;
            rb.AddForce(-direccionDaño * 10, ForceMode2D.Impulse);
            aplicarFuerza = false;
        }
    }

    public void DarInmortalidad()
    {
        StartCoroutine(Inmortalidad(tiempoInmortalidad));
    }

    public void DarInmortalidadTemporal(float tiempo)
    {
        StartCoroutine(Inmortalidad(tiempo));
    }

    private IEnumerator Inmortalidad(float t)
    {
        esInmortal = true;

        float tiempoTranscurrido = 0;

        while (tiempoTranscurrido < t)
        {
            sp.color = new Color(1,1,1, 0.5f);
            yield return new WaitForSeconds( t/10);
            sp.color = new Color(1,1,1,1);
            yield return new WaitForSeconds( t/10);
            tiempoTranscurrido +=  t/5;
        }

        esInmortal = false;
    }

    public void MovimientoFinalMapa(int direccionX)
    {
        terminandoMapa = true;
        direccionXfinal = direccionX;
        anim.SetBool("caminar",true);
        if(direccionXfinal < 0 && transform.localScale.x > 0)
        {
            direccionMovimiento = DireccionAtaque(Vector2.left,direccion);
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        }
        else if(direccionXfinal > 0 && transform.localScale.x < 0)
        {
            direccionMovimiento = DireccionAtaque(Vector2.right,direccion);
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
    }

    void Update()
    {
        if(!terminandoMapa)
        {
            Movimiento();
            Agarres();
            if(!enSuelo)
            {
                ccollider.sharedMaterial = pm2;
            }
            else
            {
                ccollider.sharedMaterial = null;
            }
        }
        else
        {
            rb.velocity =  new Vector2(direccionXfinal * velocidadDeMovimiento,rb.velocity.y);
        }

        if(!esInmortal && ultimoEnemigo != null)
        {
            Physics2D.IgnoreCollision(ultimoEnemigo.GetComponent<Collider2D>(),GetComponent<Collider2D>(),false);
            ultimoEnemigo = null;
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if(other.gameObject.CompareTag("Enemigo"))
        {
            if(esInmortal)
            {
                ultimoEnemigo = other.gameObject;
                Physics2D.IgnoreCollision(ultimoEnemigo.GetComponent<Collider2D>(),GetComponent<Collider2D>(),true);
            }
        }
    }

    private void Agacharse()
    {
        estaAgachado = true;
        ccollider.offset = new Vector2(0.01213455f,0.0f);
        ccollider.size = new Vector2(1.008659f,1.238659f);
        velocidadDeMovimiento = velocidadDeMovimientoAuxiliar / 3;
        anim.SetBool("agachado",true);
    }

    private void Atacar(Vector2 direccion)
    {
        if(Input.GetKeyDown(KeyCode.Z) && !Input.GetKey(KeyCode.LeftControl))
        {
            if(!estaAtacando && !haciendoDash)
            {
                estaAtacando=true;

                anim.SetFloat("ataqueX",direccion.x);
                anim.SetFloat("ataqueY",direccion.y);
                anim.SetBool("atacar",true);
            }

        }
    }

    public void FinalizarAtaque()
    {
        anim.SetBool("atacar",false);
        estaAtacando = false;
        bloqueado = false;
    }

    private Vector2 DireccionAtaque(Vector2 dir_mov,Vector2 direccion)
    {
        if(rb.velocity.x == 0 && direccion.y != 0)
            return new Vector2(0,direccion.y);
        
        return new Vector2(dir_mov.x,direccion.y);
    }

    private IEnumerator AgitarCamara()
    {
        haciendoShake = true;
        CinemachineBasicMultiChannelPerlin cinemachineBMCP = cm.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        cinemachineBMCP.m_AmplitudeGain = 4;
        yield return new WaitForSeconds(0.3f);
        cinemachineBMCP.m_AmplitudeGain = 0;
        haciendoShake = false;
    }

    private IEnumerator AgitarCamara(float tiempo)
    {
        haciendoShake = true;
        CinemachineBasicMultiChannelPerlin cinemachineBMCP = cm.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        cinemachineBMCP.m_AmplitudeGain = 4;
        yield return new WaitForSeconds(tiempo);
        cinemachineBMCP.m_AmplitudeGain = 0;
        haciendoShake = false;
    }

    private void Dash(float x)
    {
        anim.SetBool("dash",true);
        Camera.main.GetComponent<RippleEffect>().Emit(Camera.main.WorldToViewportPoint(transform.position));
        StartCoroutine(AgitarCamara(0.5f));

        puedeDash=true;
        rb.velocity = Vector2.zero;
        rb.velocity+= new Vector2(x,0).normalized * velocidadDash;
        StartCoroutine(PrepararDash());
    }

    private IEnumerator PrepararDash()
    {
         StartCoroutine(DashSuelo());
        
        rb.gravityScale = 0;
        haciendoDash = true;

        yield return new WaitForSeconds(0.3f);
        rb.gravityScale = 3;
        haciendoDash = false;
        FinalizarDash();
    }

    private IEnumerator DashSuelo()
    {
        yield return new WaitForSeconds(0.15f);
        if(enSuelo)
            puedeDash=false;
    }

    public void FinalizarDash()
    {
        anim.SetBool("dash",false);
    }

    private void TocarPiso()
    {
        puedeDash = false;
        haciendoDash = false;
        anim.SetBool("salto",false);
    }

    private void Movimiento() 
    {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");

        float xr = Input.GetAxisRaw("Horizontal");
        float yr = Input.GetAxisRaw("Vertical");

        direccion = new Vector2(x,y);

        Caminar();
        Atacar(DireccionAtaque(direccionMovimiento, new Vector2(xr,yr)));

        if(Input.GetKey(KeyCode.LeftControl))
        {
            Agacharse();
        }
        else if(estaAgachado)
        {
            estaAgachado = false;
            ccollider.offset = new Vector2(0.01213455f,-0.01820087f);
            ccollider.size = new Vector2(1.008659f,1.495062f);
            velocidadDeMovimiento = velocidadDeMovimientoAuxiliar;
            anim.SetBool("agachado",false);
        }

        if(enSuelo && !haciendoDash)
        {
            if(saltardeMuro)
            {
                saltardeMuro = false;
                rb.velocity = new Vector2(0,rb.velocity.y);
            }
        }

        agarrarse = enMuro && Input.GetKey(KeyCode.C);

        if(agarrarse && !enSuelo)
        {
            
            anim.SetBool("escalar",true);
            
            if(rb.velocity.y <= 0.1)
            {
                anim.SetFloat("vel_escala",0);
            }
            else
            {
                anim.SetFloat("vel_escala",1);
            }
        }
        else
        {
            anim.SetBool("escalar",false);
            anim.SetFloat("vel_escala",0);
        }

        //Cambia la gravedad al escalar y pone bien al jugador de sitio
        if(agarrarse && !haciendoDash)
        {
            rb.gravityScale = 0;
            if(x > 0.2f || x < -0.2f)
            {
                rb.velocity = new Vector2(rb.velocity.x,0);
            }

            float modificadorVelocidad = y > 0 ? 0.5f : 1;
            rb.velocity = new Vector2(rb.velocity.x, y * (velocidadDeMovimiento * modificadorVelocidad));

            if(muroIzquierdo && transform.localScale.x > 0)
            {
                transform.localScale = new Vector3(-transform.localScale.x,transform.localScale.y,transform.localScale.z);
            }
            else if(muroDerecho && transform.localScale.x < 0)
            {
                transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x),transform.localScale.y,transform.localScale.z);
            }
        }
        else
        {
            rb.gravityScale = 3;
        }

        if(enMuro && !enSuelo)
        {
            anim.SetBool("escalar",true);
            if(x != 0 && !agarrarse)
                DeslizarPared();
        }

        MejorarSalto();
        if(Input.GetKeyDown(KeyCode.Space))
        {
            if(enSuelo)
            {
                anim.SetBool("salto",true);
                audioSource.PlayOneShot(walkSound);
                Saltar();
            }
            
            if(enMuro && !enSuelo) 
            {
                anim.SetBool("escalar",true);
                anim.SetBool("salto",true);
                SaltarDesdeMuro();
            }
        }
        
        float vel;
        if(rb.velocity.y > 0)
            vel=1;
        else
            vel=-1;
        
        if(!enSuelo)
        {
            anim.SetFloat("velocidadVertical",vel);
        }
        else
        {
            if(vel == -1)
                FinalizarSalto();
        }

        if(Input.GetKeyDown(KeyCode.X) && !haciendoDash && !puedeDash)
        {
            if(xr !=0)
            {
                Dash(xr);
            }
        }

        if(enSuelo && !tocadoPiso)
        {
            anim.SetBool("escalar",false);

            TocarPiso();
            tocadoPiso=true;
        }

        if(!enSuelo && tocadoPiso)
        {
            tocadoPiso=false;
        }
    }
    private void SaltarDesdeMuro()
    {
        StopCoroutine(DeshabilitarMovimiento(0));
        StartCoroutine(DeshabilitarMovimiento(0.05f));

        Vector2 direccionMuro = muroDerecho ? Vector2.left : Vector2.right;

        if(direccionMuro.x < 0 && transform.localScale.x > 0)
        {
            transform.localScale = new Vector3(-transform.localScale.x,transform.localScale.y,transform.localScale.z);
        }
        else if(direccionMuro.x > 0 && transform.localScale.x < 0)
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x),transform.localScale.y,transform.localScale.z);
        }

        anim.SetBool("salto",true);
        anim.SetBool("escalar",false);
        Saltar((Vector2.up + direccionMuro),true);

        saltardeMuro = true;
    }

    private IEnumerator DeshabilitarMovimiento(float tiempo)
    {
        puedeMover = false;
        yield return new WaitForSeconds(tiempo);
        puedeMover = true;
    }

    private void DeslizarPared()
    {
        if(puedeMover)
        {
            rb.velocity = new Vector2(rb.velocity.x, -velocidadDeslizar);
        }
    }
    
    private void Agarres()
    {
        enSuelo = Physics2D.OverlapCircle((Vector2)transform.position + abajo, radioDeColision, layerPiso) || Physics2D.OverlapCircle((Vector2)transform.position + abajo, radioDeColision, layerPlataforma);
        muroDerecho = Physics2D.OverlapCircle((Vector2)transform.position + derecha,radioDeColision, layerPared);
        muroIzquierdo = Physics2D.OverlapCircle((Vector2)transform.position + izquierda,radioDeColision, layerPared);
        
        enMuro = muroDerecho || muroIzquierdo;

    } 
    
    private void MejorarSalto()
    {
        if(rb.velocity.y < 0)
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (2.5f -1) * Time.deltaTime;
        }
        else if(rb.velocity.y > 0 && !Input.GetKey(KeyCode.Space))
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (2.0f -1) * Time.deltaTime;
        }
    }

    private void Saltar() 
    {
        rb.velocity = new Vector2(rb.velocity.x,rb.velocity.y);
        rb.velocity += Vector2.up * fuerzaDeSalto;
    }

    private void Saltar(Vector2 direccionSaltar, bool muro) 
    {
        rb.velocity = new Vector2(rb.velocity.x,0);
        rb.velocity += direccionSaltar * fuerzaDeSalto;
    }

    private void Caminar() 
    {   
        if(puedeMover && !haciendoDash && !estaAtacando)
        {
            if(saltardeMuro)
            {
                rb.velocity = Vector2.Lerp(rb.velocity, 
                    (new Vector2(direccion.x * velocidadDeMovimiento/2, rb.velocity.y)), Time.deltaTime / 2);
            }
            else
            {
                if(direccion.x != 0 && !agarrarse)
                {
                    if(!enSuelo)
                    {
                        if(estaAgachado)
                            anim.SetBool("caminar",true);
                        else
                            anim.SetBool("salto",true);
                    }
                    else
                    {
                        anim.SetBool("caminar",true);
                    }

                    rb.velocity =  new Vector2(direccion.x * velocidadDeMovimiento,rb.velocity.y);
                    if(direccion.x < 0 && transform.localScale.x > 0)
                    {
                        direccionMovimiento = DireccionAtaque(Vector2.left,direccion);
                        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
                    }
                    else if(direccion.x > 0 && transform.localScale.x < 0)
                    {
                        direccionMovimiento = DireccionAtaque(Vector2.right,direccion);
                        transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
                    }
                }
                else
                {
                    if(direccion.y > 0 && direccion.x == 0)
                        direccionMovimiento = DireccionAtaque(direccion,Vector2.up);
                    anim.SetBool("caminar",false);
                }

            }
           
            
        }
        else
        {
            if(bloqueado)
            {
                FinalizarAtaque();
            }
        }
    }

    public void FinalizarSalto()
    {
        anim.SetBool("salto",false);
        anim.SetBool("caer",false);
    }

    public void PararTodo()
    {
        anim.SetBool("salto",false);
        anim.SetBool("caer",false);
        anim.SetBool("caminar",false);
        anim.SetBool("escalar",false);
        anim.SetBool("dash",false);
        rb.velocity =  new Vector2(0,rb.velocity.y);
    }

    private void PlayWalk()
    {
        audioSource.PlayOneShot(walkSound);
    }
}
