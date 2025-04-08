using UnityEngine;

[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(Rigidbody))]
public class BulletBehaviour : MonoBehaviour
{
    Rigidbody rb;
    PlayerController player;
    public float speed;
    BarrelManager weaponManager;
    Vector3 direction;

    private void Awake()
    {   
        rb = GetComponent<Rigidbody>();
        player = GameObject.FindAnyObjectByType<PlayerController>();
        if(name!="BulletTypeTwo(Clone)")
        {direction= Vector3.forward;}
        else if(name.Equals("BulletTypeTwo(Clone)"))
        {direction=Vector3.forward+new Vector3(Random.Range(-0.2f,0.2f),0,0);}
    }

    private void Start()
    {rb.mass = 0;}

    private void FixedUpdate()
    {rb.velocity=direction*speed*Time.fixedDeltaTime;}

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            collision.gameObject.GetComponent<EnemyBehaviour>().currentHealth -= player.weaponDamage;
            if (collision.gameObject.GetComponent<EnemyBehaviour>().isBoss)
            {
                collision.gameObject.GetComponent<EnemyBehaviour>().bossCanvas.enabled=true;

            }
            gameObject.SetActive(false);
        }
        else if(collision.gameObject.CompareTag("Barrel"))
        {
            if(collision.gameObject.GetComponent<BarrelManager>().barrelHealth > 0)
            {
                collision.gameObject.GetComponent<BarrelManager>().barrelHealth--;
            }
            gameObject.SetActive(false);
        }
    }
}
