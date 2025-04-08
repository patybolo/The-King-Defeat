using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

public enum WeaponType
{
    HANDGUN,
    AK,
    SHOTGUN
}

[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(Animator))]

public class PlayerController : MonoBehaviour
{
    int maxHealth;
    public int currentHealth;
    float speed;
    public int score;
    public Vector3 moveInput;
    bool isDead;
    float screenHeight;
    float screenWidth;
    Rigidbody rb;
    CapsuleCollider col;
    AudioSource audioSource;
    public List<AudioClip> sound;
    public List<GameObject> bulletsInMemory;
    [Tooltip("HandgunBullets")] public List<GameObject> bulletsInGameTypeOne;
    [Tooltip("ShotgunBullets")] public List<GameObject> bulletsInGameTypeTwo;
    [Tooltip("MachineGunBullets")] public List<GameObject> bulletsInGameTypeThree;
    public Transform bulletOrigin;
    Animator animator;
    public int weaponDamage;
    public float weaponRate;
    public WeaponType currentWeapon;
    float shootTimer;
    bool lvlDefeated;
    Canvas onGameCanvas;
    Canvas pauseCanvas;
    Canvas gameOverCanvas;
    Canvas victoryCanvas;
    Text scoreTxt;
    Text maxScoreTxt;
    Text continueTxt;
    Text retryTxt;
    Text retryTxt2;
    Text exitTxt;
    Text exitTxt2;
    Text nextTxt;
    Text victoryExitTxt;
    Text finalScoreTxt;
    Text highScoreTxt;
    int ShotgunLoad;
    PlayableDirector playableDirector;

    private void Awake()
    {
        playableDirector = GetComponent<PlayableDirector>();
        scoreTxt = GameObject.Find("ScoreTxt").GetComponent<Text>();
        maxScoreTxt = GameObject.Find("MaxScoreTxt").GetComponent<Text>();
        continueTxt = GameObject.Find("ContinueTxt").GetComponent<Text>();
        retryTxt = GameObject.Find("RetryTxt").GetComponent<Text>();
        retryTxt2 = GameObject.Find("RetryTxt2").GetComponent<Text>();
        exitTxt = GameObject.Find("ExitTxt").GetComponent<Text>();
        exitTxt2 = GameObject.Find("ExitTxt2").GetComponent<Text>();
        nextTxt = GameObject.Find("NextTxt").GetComponent<Text>();
        victoryExitTxt = GameObject.Find("VictoryExitTxt").GetComponent<Text>();
        finalScoreTxt = GameObject.Find("FinalScoreTxt").GetComponent<Text>();
        onGameCanvas = GameObject.Find("OnGameCanvas").GetComponent<Canvas>();
        pauseCanvas = GameObject.Find("PauseCanvas").GetComponent<Canvas>();
        gameOverCanvas = GameObject.Find("GameOverCanvas").GetComponent<Canvas>();
        victoryCanvas = GameObject.Find("VictoryCanvas").GetComponent<Canvas>();
        highScoreTxt = GameObject.Find("HighScoreTxt").GetComponent<Text>();
        
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        col = GetComponent<CapsuleCollider>();
        audioSource = GetComponent<AudioSource>();
        lvlDefeated = false;
        shootTimer = 0;
        maxHealth = 100;
        speed = 5f;
        ShotgunLoad = 0;
    }
    void Start()
    {
        screenHeight = (float)Screen.height / 2.0f;
        screenWidth = (float)Screen.width / 2.0f;
        isDead = false;
        currentHealth = maxHealth;
        weaponChange(WeaponType.HANDGUN);
        creationOfBullets();
        victoryCanvas.gameObject.SetActive(false);
        gameOverCanvas.enabled = false;
        onGameCanvas.enabled = true;
        pauseCanvas.enabled = false;
        localizeHUDText();
        animator.SetFloat("randiVictoryAnim",Random.Range(-0.1f,0.1f));
    }
    private void Update()
    {
        if(GameManager.sharedInstance.gameState==GameStates.OnGame)
        {
            fireBullet(currentWeapon);
        }
        anims();
        bulletCleaner();
        limitX();
    }
    private void FixedUpdate()
    {
        if (GameManager.sharedInstance.gameState == GameStates.OnGame){movement();}
    }
    public void takeDamage(int damageReceived)
    {
        if (currentHealth > 0)
        {
            currentHealth -= damageReceived;
            playerLifeLogic(); //lo hacemos aca y no en update porque el unico momento en el que tendrias que reevaluar si esta vivo o no es cuando recibe damage
        }
    }

    public void givePlayerValue(int value)
    { score += value;
        scoreTxt.text = $"{score}"; }

    void playerLifeLogic()
    {
        if (currentHealth <= 0)
        {
            isDead = true;
            gameOver();
            audioSource.clip=sound[3];
            audioSource.Play();
            if(!audioSource.isPlaying)
            {audioSource.mute=true;}
        }
    }

    void anims()
    { animator.SetBool("Dead",isDead);
     if (Input.touchCount == 0){animator.SetFloat("XDirection", Input.GetAxis("Horizontal"));}
      animator.SetBool("RunForward", lvlDefeated); }

    void limitX()
    {if(transform.position.x<=-3.8f){transform.position=new Vector3(-3.75f,transform.position.y,transform.position.z);}
    else if(transform.position.x>=3.8f){transform.position=new Vector3(3.75f,transform.position.y,transform.position.z);}}

    void movement()
    {
        if (currentHealth>0)
        {
            moveInput = new Vector3(Input.GetAxis("Horizontal") * speed * Time.fixedDeltaTime, 0, 0);
            rb.MovePosition(rb.position + moveInput);

            if (Input.touchCount > 0) // Logica celular
            {
                Touch t = Input.GetTouch(0);
                if (t.phase == TouchPhase.Stationary && t.position.x > screenWidth)
                { moveInput = new Vector3(1 * speed * Time.fixedDeltaTime, 0, 0); 
                 rb.MovePosition(rb.position + moveInput);
                 animator.SetFloat("XDirection", 1);
                }
                else if (t.phase == TouchPhase.Stationary && t.position.x < screenWidth)
                { moveInput = new Vector3(-1 * speed * Time.fixedDeltaTime, 0, 0); 
                 rb.MovePosition(rb.position + moveInput);
                 animator.SetFloat("XDirection", -1);
                }
            }
        }
    }

    void creationOfBullets()
    {
        for (int i = 0; i < 15; i++)
        {
            GameObject handgunBullets = Instantiate(bulletsInMemory[0]);
            bulletsInGameTypeOne.Add(handgunBullets);
            handgunBullets.SetActive(false);
        }
        for(int i = 0; i < 20; i++)
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

    void weaponGlow()
    {
        // TODO: efecto de disparo del arma
    }
    void fireBullet(WeaponType weaponInUse)
    {
        if(Time.time>shootTimer&&currentHealth>0)
        {
            if (weaponInUse == WeaponType.HANDGUN)
            {
                audioSource.PlayOneShot(sound[0]);
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
            else if (weaponInUse==WeaponType.SHOTGUN)
            {
                audioSource.PlayOneShot(sound[1]);
                if (ShotgunLoad == 0)
                {
                    for (int i = 0; i < bulletsInGameTypeTwo.Count*0.25; i++)
                    {
                        if (!bulletsInGameTypeTwo[i].activeSelf)
                        {
                             float randomizer=Random.Range(-0.5f,0.5f);
                            bulletsInGameTypeTwo[i].transform.position=bulletOrigin.position;
                            bulletsInGameTypeTwo[i].SetActive(true);
                            //weaponGlow();
                            shootTimer = Time.time + weaponRate;
                            ShotgunLoad = 1;
                        }
                    }
                }
                else if (ShotgunLoad == 1)
                {
                    for (int i = 5; i < bulletsInGameTypeTwo.Count*0.5; i++)
                    {
                        if (!bulletsInGameTypeTwo[i].activeSelf)
                        {
                             float randomizer=Random.Range(-0.5f,0.5f);
                            bulletsInGameTypeTwo[i].transform.position=bulletOrigin.position;
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
                            float randomizer=Random.Range(-0.5f,0.5f);
                           bulletsInGameTypeTwo[i].transform.position=bulletOrigin.position;
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
                        {float randomizer=Random.Range(-0.5f,0.5f);
                            bulletsInGameTypeTwo[i].transform.position=bulletOrigin.position;
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
                        audioSource.PlayOneShot(sound[2]);
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
            if (bulletsInGameTypeTwo[i].activeSelf && bulletsInGameTypeTwo[i].transform.position.z > 20) // Escopeta
            {
                bulletsInGameTypeTwo[i].SetActive(false);
            }
        }
        for(int i = 0; i < bulletsInGameTypeThree.Count; i++)
        {
            // Deberia ser el arma con mas rango
            if (bulletsInGameTypeThree[i].activeSelf && bulletsInGameTypeThree[i].transform.position.z > 30) // AK
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
                ShotgunLoad=0;
                break;
            case WeaponType.AK:
                weaponRate = 0.05f;
                weaponDamage = 2;
                ShotgunLoad=0;
                break;
            case WeaponType.SHOTGUN:
                weaponRate = 1f;
                weaponDamage = 20;
                break;
            default:
                weaponRate = 1f;
                weaponDamage = 1;
                ShotgunLoad=0;
                Debug.Log("weaponChange() switch got to default");
                break;
        }

    }

    void weaponModelChange(WeaponType weapon)
    {
        int weaponCount = bulletOrigin.transform.childCount;
        for(int i = 0; i < weaponCount; i++) bulletOrigin.GetChild(i).gameObject.SetActive(false);
        bulletOrigin.GetChild((int)weapon).gameObject.SetActive(true);
    }

    void localizeHUDText()
    {
        if (PlayerPrefs.GetString("lang").Equals("es"))
        {
            continueTxt.text = "Continuar";
            retryTxt.text = "Reintentar";
            retryTxt2.text = "Reintentar";
            exitTxt.text = "Salir";
            exitTxt2.text = "Salir";
            nextTxt.text = "Siguiente";
            victoryExitTxt.text = "Salir";
        }
        if (PlayerPrefs.GetString("lang").Equals("en"))
        {
            continueTxt.text = "Continue";
            retryTxt.text = "Retry";
            retryTxt2.text = "Retry";
            exitTxt.text = "Exit";
            exitTxt2.text = "Exit";
            nextTxt.text = "Next";
            victoryExitTxt.text = "Exit";
        }
    }

    public int getHighScore()
    {
        //Para el display de highscore en endless
        int hs = PlayerPrefs.GetInt("HIGH_SCORE", 0);
        return hs;
    }
    public void checkHighScore()
    {
        int highScore = getHighScore();
        if (score > highScore)
        {
            PlayerPrefs.SetInt("HIGH_SCORE", score);
        }
    }
    public void pauseGame()
    {GameManager.sharedInstance.gameState=GameStates.Pause;
    pauseCanvas.enabled=true;
    onGameCanvas.enabled=false;}
    public void victory()
    {
        GameManager.sharedInstance.gameState = GameStates.Victory;
        pauseCanvas.enabled = false;
        onGameCanvas.enabled = false;
        gameOverCanvas.enabled = false;
        playableDirector.enabled = true;
        bulletOrigin.gameObject.SetActive(false);
        animator.SetBool("Win",true);
        if(PlayerPrefs.GetString("lang").Equals("es"))
        {finalScoreTxt.text=$"Puntuacion Final:{score}";}
        else 
        {finalScoreTxt.text=$"Final Score:{score}";}
        if(!lvlDefeated) // Comprobacion para hacerlo solo una vez
        {
            if(GameManager.sharedInstance.lvlIndex == 1) { PlayerPrefs.SetInt("LEVEL_KEY", 2); }
            GameManager.sharedInstance.lastLvlUnlocked++;
            GameManager.sharedInstance.saveLastLvl();
            lvlDefeated = true;
        }
        
    }
    public void gameOver()
    {
        bool endless = GameManager.sharedInstance.isEndlessLvl;
        GameManager.sharedInstance.gameState = GameStates.GameOver;
        pauseCanvas.enabled = false;
        onGameCanvas.enabled = false;
        gameOverCanvas.enabled = true;
        victoryCanvas.enabled = false;
        bulletOrigin.gameObject.SetActive(false);
        checkHighScore();
        if (PlayerPrefs.GetString("lang").Equals("es"))
        { 
            maxScoreTxt.text = $"Puntuacion: {score}";
            if (endless) { highScoreTxt.text = $"Puntuacion Mas Alta: {getHighScore()}"; } 
        }
        else
        {
            if (endless) { highScoreTxt.text = $"High Score: {getHighScore()}"; }
            maxScoreTxt.text = $"Score: {score}"; 
        }
    }

    public void onGame()
    {GameManager.sharedInstance.gameState=GameStates.OnGame;
    pauseCanvas.enabled=false;
    onGameCanvas.enabled=true;
    gameOverCanvas.enabled=false;
    victoryCanvas.enabled=false;}
    public void retrLvl ()
    {GameManager.sharedInstance.retryLvlInstance();}
    public void nextLvl()
    {GameManager.sharedInstance.goToNextLvl();
    MusicManager.sharedInstance.lvlChange.Invoke();}
    public void goToMainMenu()
    {GameManager.sharedInstance.backToMainMenu();
    MusicManager.sharedInstance.lvlChange.Invoke();}

}
