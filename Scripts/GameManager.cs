using Cinemachine;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameStates{OnGame,Pause,GameOver,Victory}

public class GameManager : MonoBehaviour
{
    public static GameManager sharedInstance;
    public CinemachineDollyCart lvlCart;
    PlayerController playerController;

    public GameStates gameState;

    public List<GameObject> normalEnemiesInMemory;
    public List<GameObject> normalEnemiesInGame;
    public List<GameObject> bossesInMemory;
    public List<GameObject> weaponBarrel;
    public List<GameObject> weaponsInGame;
    public List<GameObject> clones;
    public List<GameObject> clonesInGame;
    public GameObject kingBoss;
    public GameObject goodArcPF;
    public GameObject badArcPF;

    public Vector3[] clonesPositions;
    Vector3 weaponBarrelPos;
    Vector3 LEFT_ARC = new Vector3(-2.0f,1.5f,55.0f);
    Vector3 RIGHT_ARC = new Vector3(2.0f,1.5f,55.0f);

    float timerToSpawnBoss;
    float bossTimerPlus;
    float weaponSpawnTimer;
    float weaponSpawnCooldown;
    float arcSpawnCooldown;

    [Tooltip("Lo cambiamos para cada nivel")]public int enemyCountPerType;
    public int maxNumberOfBosses;
    public int livingNormalEnemies;
    public int  lvlIndex;
    public int livingBosses;
    public int lastLvlUnlocked;
    public int livingClones = 0;
    int numberOfBosses = 0;

    public bool isEndlessLvl;
    public bool canSpawn;
    bool isFirstBarrelSpawn;
    bool isKingLvl;
    bool hasSpawnedKing;
    
    void Awake()
    {
        playerController = GameObject.FindFirstObjectByType<PlayerController>();
        lvlCart=GameObject.FindFirstObjectByType<CinemachineDollyCart>();
        lvlIndex = SceneManager.GetActiveScene().buildIndex;
        sharedInstance =this;
        bossTimerPlus=5f;
        weaponSpawnCooldown = 2.5f;
        arcSpawnCooldown = 5f;
        timerToSpawnBoss=Time.time + bossTimerPlus;
        weaponSpawnTimer=Time.time + weaponSpawnCooldown;
    }

    void Start()
    {
        MusicManager.sharedInstance.lvlChange.Invoke();
        gameState = GameStates.OnGame;
        creationOfNormalEnemies(enemyCountPerType);
        initializeClones();
        livingNormalEnemies = normalEnemiesInGame.Count;
        livingBosses = maxNumberOfBosses;
        isFirstBarrelSpawn = true;
        canSpawn = true;
        isKingLvl = false;
        isEndlessLvl = false;
        hasSpawnedKing = false;
        if(lvlIndex == 5) // Nivel del Rey
        {
            isKingLvl = true;
        }
        else if(lvlIndex == 6) // Endless
        {
            isEndlessLvl = true;
        }
        lastLvlUnlocked=PlayerPrefs.GetInt("LEVEL_KEY");
    }

    void Update()
    {
        switch(gameState)
        {
            case GameStates.OnGame:
                if(Time.time>=timerToSpawnBoss&&numberOfBosses<maxNumberOfBosses)
                {
                    creationOfBosses();
                    timerToSpawnBoss=Time.time+bossTimerPlus;
                }
                checkForSpawn();
                checkForWin();
                arcSpawn();
                barrelSpawn();
                if(SceneManager.GetActiveScene().buildIndex!=1)
                {Time.timeScale=1;}
                break;
            case GameStates.Pause:
                Time.timeScale=0;
                break;
        }
    }

    void creationOfNormalEnemies(int amount)
    {
        int eIndex = 0;
        foreach (var enemy in normalEnemiesInMemory)
        {
            for (int i = 0; i < amount; i++)
            {
                GameObject e=Instantiate(normalEnemiesInMemory[eIndex]);
                normalEnemiesInGame.Add(e);
            }
            eIndex++;
        }
    }

    void creationOfBosses()
    { 
        GameObject b = Instantiate(bossesInMemory[Random.Range(0,bossesInMemory.Count)]);
        numberOfBosses++;
    }

    void barrelSpawn()
    {
        if((Time.time >= weaponSpawnTimer && canSpawn ) || isFirstBarrelSpawn)
        {
            //Spawnear un objeto que tenga el arma
            int newWeapon=Random.Range(0,3); // Hardcodeada la cantidad de armas
            if(lvlIndex == 4)
            {
                weaponBarrelPos = new Vector3(Random.Range(-1f, 1f), 0.8f, 55); //Siempre spawnea fuera de camara
            }
            else
            {
                weaponBarrelPos = new Vector3(Random.Range(-4f, 1.8f), 0.8f, 55); //Siempre spawnea fuera de camara
            }
            GameObject w=Instantiate(weaponBarrel[newWeapon]);
            w.GetComponent<BarrelManager>().weapon = newWeapon;
            w.transform.position = weaponBarrelPos;
            weaponSpawnTimer = Time.time + weaponSpawnCooldown;
            isFirstBarrelSpawn = false;
        }
    }
    void arcSpawn()
    {
        /*
         * Spawneamos uno bueno y uno malo a la vez
         */
        if (Time.time >= arcSpawnCooldown && canSpawn)
        {
            //Determinamos de que lado spawnea el bueno
            int side = Random.Range(1, 3); // [1,2] (no inclusivo el 3)
            GameObject goodArc = Instantiate(goodArcPF);
            goodArc.GetComponent<ArcBehaviour>().setType(true);
            goodArc.GetComponent<ArcBehaviour>().hasType = true;
            GameObject badArc = Instantiate(badArcPF);
            badArc.GetComponent<ArcBehaviour>().setType(false);
            badArc.GetComponent<ArcBehaviour>().hasType = true;
            if(side == 1) // 
            {
                goodArc.transform.position = LEFT_ARC;
                badArc.transform.position = RIGHT_ARC;
            }
            else
            {
                goodArc.transform.position = RIGHT_ARC;
                badArc.transform.position = LEFT_ARC;
            }
            arcSpawnCooldown = Time.time + Random.Range(4f, 9f); // Cooldown random entre 4 y 7 segundos
        }
    }
    void checkForSpawn()
    {
        if(livingBosses <= 0) // Si no hay mas bosses, no spawnea mas enemigos normales ni barriles
        {
            canSpawn=false; // Armas y enemigos
        }
        if(isKingLvl && !canSpawn && !hasSpawnedKing) // Si es el nivel del rey y ya murieron todos los bosses normales
        {
            GameObject king = Instantiate(kingBoss);
            livingBosses++; // Suma el rey al contador de bosses vivos
            hasSpawnedKing = true;
        }
    }
    void checkForWin()
    {
        if(!canSpawn) // Murieron los bosses
        {
            if(livingBosses<=0&&livingNormalEnemies<=0)
            {
                playerController.victory();
                lvlCart.m_Speed=4;
            }
        }
    }

    void initializeClones()
    {
        for (int i = 0; i < 6; i++)
        {
            GameObject c = Instantiate(clones[0]);
            clonesInGame.Add(c);
            c.SetActive(false);
            c.transform.parent=playerController.gameObject.transform;
        }

    }

    public void addCharacters(int count)
    {
        for (int i = 0; i < count; i++)
        {
            if(livingClones==6){break;}
            if(livingClones + count > 6) count--;
            clonesInGame[livingClones].SetActive(true);
            // Tendria que salir en la posicion del clon pero relativa al jugador, el problema es que pueden salir afuera del mapa
            clonesInGame[livingClones].transform.position = clonesPositions[livingClones] + playerController.transform.position;
            livingClones++;
        }
    }

    public void removeCharacters(int count)
    {
        for (int i = 0; i < count; i++)
        {
            if (livingClones == 0){break;}

            clonesInGame[livingClones - 1].SetActive(false);
            livingClones--;
        }
    }

    public void saveLastLvl()
    {
        if(lastLvlUnlocked>PlayerPrefs.GetInt("LEVEL_KEY"))
        {
            PlayerPrefs.SetInt("LEVEL_KEY",lastLvlUnlocked);
        }
    }

    public void retryLvlInstance()
    {SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);}

    public void goToNextLvl()
    {SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex+1);}

    public void backToMainMenu()
    {SceneManager.LoadScene(0);}
}
