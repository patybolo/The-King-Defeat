using System.Collections.Generic;
using UnityEngine;

public class CloneBehaviour : MonoBehaviour
{
    PlayerController mainCharacter;
    Vector3 moveInput;
    public bool isDead;
    float screenHeight;
    float screenWidth;
    Rigidbody rb;
    CapsuleCollider col;
    public List<AudioClip> sound;
    public List<GameObject> bulletsInMemory;
    [Tooltip("HandgunBullets")] public List<GameObject> bulletsInGameTypeOne;
    [Tooltip("ShotgunBullets")] public List<GameObject> bulletsInGameTypeTwo;
    [Tooltip("MachineGunBullets")] public List<GameObject> bulletsInGameTypeThree;
    public Transform bulletOrigin;
    Animator animator;
    public int weaponDamage;
    float weaponRate;
    WeaponType currentWeapon;
    float shootTimer;
    bool lvlDefeated;
    int ShotgunLoad;

    private void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        col = GetComponent<CapsuleCollider>();
        lvlDefeated = false;
        shootTimer = 0;
        ShotgunLoad = 0;
        animator.SetFloat("randiVictoryAnim", Random.Range(-0.1f, 0.1f));
        screenHeight = (float)Screen.height / 2.0f;
        screenWidth = (float)Screen.width / 2.0f;
        isDead = false;
        mainCharacter = GameObject.FindAnyObjectByType<PlayerController>();
        weaponDamage = mainCharacter.weaponDamage;
        weaponRate = mainCharacter.weaponRate;
        currentWeapon = mainCharacter.currentWeapon;
        creationOfBullets();
    }

    private void Update()
    {
         if (GameManager.sharedInstance.gameState == GameStates.OnGame)
         {
             fireBullet(currentWeapon);
         }
         anims();
         bulletCleaner();
         if(GameManager.sharedInstance.gameState == GameStates.Victory){gameObject.SetActive(false);}
    }

    void anims()
    {
        animator.SetBool("Dead", isDead);
        if (mainCharacter.moveInput.x > 0)
        {
            animator.SetFloat("XDirection", 1); // derecha 
        }
        else if(mainCharacter.moveInput.x < 0)
        {
            animator.SetFloat("XDirection", -1);
        }
        else { animator.SetFloat("XDirection", 0); }
        animator.SetBool("RunForward", lvlDefeated);
    }

    void creationOfBullets()
    {
        for (int i = 0; i < 15; i++)
        {
            GameObject handgunBullets = Instantiate(bulletsInMemory[0]);
            bulletsInGameTypeOne.Add(handgunBullets);
            handgunBullets.SetActive(false);
        }
        for (int i = 0; i < 20; i++)
        {
            GameObject shotgunBullets = Instantiate(bulletsInMemory[1]);
            bulletsInGameTypeTwo.Add(shotgunBullets);
            shotgunBullets.SetActive(false);
        }
        for (int i = 0; i < 80; i++)
        {
            GameObject machineGunBullets = Instantiate(bulletsInMemory[2]);
            bulletsInGameTypeThree.Add(machineGunBullets);
            machineGunBullets.SetActive(false);
        }
    }
    void fireBullet(WeaponType weaponInUse)
    {
        if (Time.time > shootTimer && !isDead)
        {
            if (weaponInUse == WeaponType.HANDGUN)
            {
                for (int i = 0; i < bulletsInGameTypeOne.Count; i++)
                {
                    if (!bulletsInGameTypeOne[i].activeSelf)
                    {
                        bulletsInGameTypeOne[i].transform.position = bulletOrigin.position;
                        bulletsInGameTypeOne[i].SetActive(true);
                        shootTimer = Time.time + weaponRate;
                        break;
                    }
                }
            }
            else if (weaponInUse == WeaponType.SHOTGUN)
            {
                if (ShotgunLoad == 0)
                {
                    for (int i = 0; i < bulletsInGameTypeTwo.Count * 0.25; i++)
                    {
                        if (!bulletsInGameTypeTwo[i].activeSelf)
                        {
                            float randomizer = Random.Range(-0.5f, 0.5f);
                            bulletsInGameTypeTwo[i].transform.position = bulletOrigin.position;
                            bulletsInGameTypeTwo[i].SetActive(true);
                            //weaponGlow();
                            shootTimer = Time.time + weaponRate;
                            ShotgunLoad = 1;
                        }
                    }
                }
                else if (ShotgunLoad == 1)
                {
                    for (int i = 5; i < bulletsInGameTypeTwo.Count * 0.5; i++)
                    {
                        if (!bulletsInGameTypeTwo[i].activeSelf)
                        {
                            float randomizer = Random.Range(-0.5f, 0.5f);
                            bulletsInGameTypeTwo[i].transform.position = bulletOrigin.position;
                            bulletsInGameTypeTwo[i].SetActive(true);
                            //weaponGlow();
                            shootTimer = Time.time + weaponRate;
                            ShotgunLoad = 2;
                        }
                    }
                }
                else if (ShotgunLoad == 2)
                {
                    for (int i = 10; i < bulletsInGameTypeTwo.Count * 0.75; i++)
                    {
                        if (!bulletsInGameTypeTwo[i].activeSelf)
                        {
                            float randomizer = Random.Range(-0.5f, 0.5f);
                            bulletsInGameTypeTwo[i].transform.position = bulletOrigin.position;
                            bulletsInGameTypeTwo[i].SetActive(true);
                            //weaponGlow();
                            shootTimer = Time.time + weaponRate;
                            ShotgunLoad = 3;
                        }
                    }
                }
                else if (ShotgunLoad == 3)
                {
                    for (int i = 15; i < bulletsInGameTypeTwo.Count; i++)
                    {
                        if (!bulletsInGameTypeTwo[i].activeSelf)
                        {
                            float randomizer = Random.Range(-0.5f, 0.5f);
                            bulletsInGameTypeTwo[i].transform.position = bulletOrigin.position;
                            bulletsInGameTypeTwo[i].SetActive(true);
                            shootTimer = Time.time + weaponRate;
                            ShotgunLoad = 0;
                        }
                    }
                }
            }
            else if (weaponInUse == WeaponType.AK)
            {
                for (int i = 0; i < bulletsInGameTypeThree.Count; i++)
                {
                    if (!bulletsInGameTypeThree[i].activeSelf)
                    {
                        bulletsInGameTypeThree[i].transform.position = bulletOrigin.position;
                        bulletsInGameTypeThree[i].SetActive(true);
                        shootTimer = Time.time + weaponRate;
                        break;
                    }
                }
            }
        }
    }

    void bulletCleaner()
    {
        // Esta funcion hay que cambiarla a medida que se balancee el juego
        for (int i = 0; i < bulletsInGameTypeOne.Count; i++)
        {
            if (bulletsInGameTypeOne[i].activeSelf && bulletsInGameTypeOne[i].transform.position.z > 40) // Lo baje de 50 -> 40
            {
                bulletsInGameTypeOne[i].SetActive(false);
            }
        }
        for (int i = 0; i < bulletsInGameTypeTwo.Count; i++)
        {
            if (bulletsInGameTypeTwo[i].activeSelf && bulletsInGameTypeTwo[i].transform.position.z > 15) // Escopeta
            {
                bulletsInGameTypeTwo[i].SetActive(false);
            }
        }
        for (int i = 0; i < bulletsInGameTypeThree.Count; i++)
        {
            // Deberia ser el arma con mas rango
            if (bulletsInGameTypeThree[i].activeSelf && bulletsInGameTypeThree[i].transform.position.z > 50) // AK
            {
                bulletsInGameTypeThree[i].SetActive(false);
            }
        }
    }
    public void weaponChange(WeaponType weapon)
    {
        weaponModelChange(weapon);
        currentWeapon = weapon;
        switch (currentWeapon)
        {
            case WeaponType.HANDGUN:
                weaponRate = 0.25f;
                weaponDamage = 3;
                ShotgunLoad = 0;
                break;
            case WeaponType.AK:
                weaponRate = 0.05f;
                weaponDamage = 2;
                ShotgunLoad = 0;
                break;
            case WeaponType.SHOTGUN:
                weaponRate = 1f;
                weaponDamage = 20;
                break;
            default:
                weaponRate = 1f;
                weaponDamage = 1;
                ShotgunLoad = 0;
                Debug.Log("weaponChange() switch got to default");
                break;
        }

    }
    void weaponModelChange(WeaponType weapon)
    {
        int weaponCount = bulletOrigin.transform.childCount;
        for (int i = 0; i < weaponCount; i++) bulletOrigin.GetChild(i).gameObject.SetActive(false);
        bulletOrigin.GetChild((int)weapon).gameObject.SetActive(true);
    }

}
