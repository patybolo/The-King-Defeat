using TMPro;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(Rigidbody))]
public class BarrelManager : MonoBehaviour
{
    PlayerController playerController;
    Rigidbody rb;
    BoxCollider col;
    public GameObject barrelModel;
    public Canvas barrelCanvas;
    bool isDestroyed;
    public TextMeshProUGUI barrelHealthRepresentation;
    public int weapon;
    public int barrelHealth;
    float despawnTimer;
    float despawnCooldown;
    float barrelSpeed;
    float rotationSpeed; // Idealmente el barril tendria que "rotar" pero lo dejo para que lo mires primero

    private void Awake()
    {
        playerController = GameObject.FindObjectOfType<PlayerController>();
        rotationSpeed = -3;
        rb = GetComponent<Rigidbody>();
        col = GetComponent<BoxCollider>();
        //Habria que inicializar un Text que sea la vida del barril, pero eso se deberia hacer en el canvas y no lo puedo hacer funcionar
    }

    private void Start()
    {
        barrelSpeed=5f;
        barrelHealth = getBarrelHealth();
        barrelModel.SetActive(true);
        isDestroyed = false;
        despawnCooldown = 3.5f;
        col.isTrigger = false;
        barrelHealthRepresentation.text = $"X{barrelHealth}";
    }

    private void Update()
    {
        updateBarrelStatus();
        despawnWeaponOnTimer();
    }

    private void FixedUpdate()
    {
        moveBarrel();
        disableAll();
    }
    void moveBarrel()
    {
        Quaternion rot = Quaternion.Euler(0, rotationSpeed, 0);
        if (rb.position.z < -6) // Si lo pasa al jugador
        {
            Destroy(gameObject);
        }
        if(barrelHealth <= 0)
        {
            if (rb.position.z >= -5.5f)
            {
                rb.MovePosition(rb.position + transform.forward * Time.fixedDeltaTime * 25); // Que vaya muy rapido hacia el jugador
            }
            else if(!isDestroyed)
            {
                isDestroyed = true;
                despawnTimer = Time.time + despawnCooldown;
            }
        }
        else
        {
            rb.MovePosition(rb.position + transform.forward * Time.fixedDeltaTime * barrelSpeed);
        }
        barrelModel.transform.rotation*=rot;
    }

    int getBarrelHealth()
    {
        int health = 0;
        switch (weapon)
        {
            case 0:
                health = Random.Range(1, 5);
                break;
            case 1:
                health = Random.Range(10,16);
                break;
            case 2:
                health = Random.Range(5, 11); ;
                break;
        }
        return health;
    }

    void updateBarrelStatus()
    {
        //text = barrelHealth
        //text.transform.position = new Vector3(gameObject.transform.position)
        barrelHealthRepresentation.text = $"X{barrelHealth}";
        // Si el barril se rompe, el arma tiene que seguir para adelante (sacamos el barril)
        if (barrelHealth <= 0)
        {
            barrelModel.SetActive(false);
            col.isTrigger = true;
            rb.useGravity=false;
            barrelCanvas.enabled = false;
        }
    }

    void despawnWeaponOnTimer()
    {
        if(isDestroyed)
        {
            if(Time.time >= despawnTimer)
            {
                Destroy(gameObject);
            }
        }
    }
    void disableAll()
    {if(GameManager.sharedInstance.gameState==GameStates.Victory)
    {gameObject.SetActive(false);}
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerController.takeDamage(100); // No se rompió el barril, te mata
            Destroy(gameObject);
        }
        if (collision.gameObject.CompareTag("Clone"))
        {
            collision.gameObject.SetActive(false);
            Destroy(gameObject);
            GameManager.sharedInstance.livingClones--;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {playerController.weaponChange((WeaponType)weapon);
        Destroy(gameObject);}
    }
}
