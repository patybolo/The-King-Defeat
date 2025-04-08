using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(CapsuleCollider))]

public class EnemyBehaviour : MonoBehaviour
{
    public float maxHealth;
    public float currentHealth;
    public float movementSpeed;
    public int valueForPlayer;
    float speed;
    int damageToPlayer;
    float damageTimer;
    float damageCooldown;
    float deadCounterAnim;
    public AnimationClip deadAnim;
    public Vector3 initPos;
    public bool isBoss;
    bool isDead;
    [Tooltip("Barra de vida del jefe")]public Image bossLifeBar;
    AudioSource audioSource;
    Animator animator;
    Rigidbody rb;
    CapsuleCollider col;
    public float distanceFromTarget;
    PlayerController playerReference;
    public List<AudioClip> sound;
    public Canvas bossCanvas;
    float lifeBarTimerOut;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        animator = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody>();
        col = GetComponent<CapsuleCollider>();
        playerReference = GameObject.FindAnyObjectByType<PlayerController>();
        currentHealth = maxHealth;
        deadCounterAnim = deadAnim.length;
        speed = movementSpeed;
        damageToPlayer = 5;
        damageCooldown = 1f;
        if(bossCanvas!=null&&isBoss)
        {bossCanvas.enabled=false;
        lifeBarTimerOut=0.5f;}
    }

    private void Start()
    {
        if(GameManager.sharedInstance.lvlIndex == 4) // Nivel de la entrada al castillo
        {
            initPos = new Vector3(Random.Range(-2.9f, 2.9f), 0, Random.Range(45, 50));
        }
        else
        {
            initPos = new Vector3(Random.Range(-3.5f, 3.5f), 0, Random.Range(45, 50));
        }
        gameObject.transform.position=initPos;
    }

    private void Update()
    {
        animations();
        distanceFromPlayer();
        sounds();
        lifeLogic();
        limitPos();
        if(isBoss&&bossCanvas!=null)
        {updateBossBar();
        canvasFollow();}
    }

    void canvasFollow()
    {if(bossCanvas!=null)
     {bossLifeBar.transform.position=new Vector3(gameObject.transform.position.x,bossLifeBar.transform.position.y, gameObject.transform.position.z);}}

    private void FixedUpdate()
    {
        movement();
    }
    void animations()
    {
        animator.SetBool("Dead", isDead);
    }
    void sounds()
    {
        if (!audioSource.isPlaying && !isDead)
        {
            audioSource.clip = sound[0];
            audioSource.Play();
        }
        else if (!audioSource.isPlaying && isDead)
        {
            audioSource.clip = sound[1];
            audioSource.Play();
        }
    }
    void lifeLogic()
    {
        if (currentHealth <= 0)
        {
            isDead = true;
            speed = 0;
            col.enabled = false;
            deadCounterAnim -= Time.deltaTime;
            rb.velocity = Vector3.zero;
            if(deadCounterAnim<=0 && !isBoss)
            {
                if (!GameManager.sharedInstance.isEndlessLvl){ GameManager.sharedInstance.livingNormalEnemies--; }
                deadCounterAnim =deadAnim.length;
                reviveEnemy();
                playerReference.givePlayerValue(valueForPlayer);
            }
            else if(deadCounterAnim<=0 && isBoss) 
            {
                if (!GameManager.sharedInstance.isEndlessLvl) { GameManager.sharedInstance.livingBosses--; }
                bossCanvas.enabled = false;
                gameObject.SetActive(false);
            }
        }
        else
        {
            isDead = false;
            speed = movementSpeed;
            col.enabled = true;
        }
    }

    void limitPos()
    {
        // Como el radio del CapsuleCollider del Player es de 0.5f, los enemigos pueden entrar en ese rango (para poder hacerle daño al Player)
        // Pero no deben irse detras del jugador, dandote la posibilidad de ganar siempre
        // La posicion del Player es -5.5 en Z, por lo que como maximo que se acerquen hasta -5f y puedan entrar en colision
        // Otra solucion seria agregar una colision para los enemigos a la altura del player?
        if(rb.position.z <= -5f)
        {
            rb.position = new Vector3(rb.position.x, rb.position.y, -5f);
        }
        if(rb.position.x<-4f)
        {
            rb.position=new Vector3(-3.9f,rb.position.y,rb.position.z);
        }
        else if(rb.position.x>4f)
        {
            rb.position=new Vector3(3.9f,rb.position.y,rb.position.z);
        }
    }

    void reviveEnemy()
    {
        if(GameManager.sharedInstance.canSpawn)
        {
            initPos = new Vector3(Random.Range(-4, 4), 0, Random.Range(45, 55));
            rb.position = initPos; // Ahora respawnea bien con el Rigidbody
            currentHealth = maxHealth;
            GameManager.sharedInstance.livingNormalEnemies++;
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
    
    public void updateBossBar()
    {
        bossLifeBar.fillAmount=currentHealth/maxHealth;
        if(bossCanvas.enabled)
        {lifeBarTimerOut-=Time.deltaTime;
        if(lifeBarTimerOut<=0)
        {bossCanvas.enabled=false;
        lifeBarTimerOut=0.5f;}}
    }

    void movement()
    {
        if (distanceFromTarget > 10 && !isDead)
        {
            rb.MovePosition(rb.position + Vector3.back * Time.fixedDeltaTime * movementSpeed);
            transform.rotation = Quaternion.Euler(0, 180, 0);
        }
        else if (distanceFromTarget < 10 && !isDead)
        {
            Quaternion rot = Quaternion.LookRotation(new Vector3(playerReference.gameObject.transform.position.x, 0, playerReference.gameObject.transform.position.z) - new Vector3(transform.position.x, 0, transform.position.z)).normalized;
            rb.MoveRotation(rot);
            rb.MovePosition(rb.position + transform.forward * Time.fixedDeltaTime * movementSpeed);
        }
        else if (isDead)
        { 
            transform.position = rb.position;
            transform.rotation = Quaternion.Euler(0, 180, 0);
        }
    }
    void distanceFromPlayer()
    { distanceFromTarget = Vector3.Distance(transform.position, playerReference.gameObject.transform.position); }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (Time.time > damageTimer) // pasó el tiempo suficiente?
            {
                playerReference.takeDamage(damageToPlayer);
                damageTimer = Time.time + damageCooldown; // tiene que volver a pasar 1segundo de Time.time (tiempo de ejecucion) para que pueda volver a entrar a la funcion
                animator.SetTrigger("Attack");
            }
        }
        if(collision.gameObject.CompareTag("Clone"))
        {
            GameManager.sharedInstance.livingClones--;
            collision.gameObject.SetActive(false);
        }
    }
}