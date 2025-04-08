using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ArcBehaviour : MonoBehaviour
{
    public Canvas arcCanvas;
    public TextMeshProUGUI arcText;
    BoxCollider col;
    Rigidbody rb;

    public Vector3 initDir;

    int currentCharacters;
    int charactersToAdd;
    int charactersToRemove;

    float xSpeed;

    public bool hasType;
    bool isInitialized;
    [SerializeField]bool isGoodArc;
    
    private void Awake()
    {
        xSpeed = Random.Range(0f,0.5f);
        col = GetComponent<BoxCollider>();
        rb = GetComponent<Rigidbody>();
        hasType = false;
        isInitialized = false;
        currentCharacters = GameManager.sharedInstance.livingClones;
    }

    private void Start()
    {
        initDir = new Vector3(Random.Range(-1,1),0,-1);
    }

    private void FixedUpdate()
    {
        movement();
    }
    
    private void Update()
    {
        if(!isInitialized && hasType)
        {
            if(isGoodArc) { goodArcLogic(); }
            else { badArcLogic(); }
            isInitialized = true;
        }
        destroyOnThreshold();
        disableAll();
    }

    public void setType(bool isGood)
    {
        isGoodArc = isGood;
    }

    void goodArcLogic()
    {
        if(currentCharacters == 6)
        {
            arcText.text = "MH"; // Provisional?
            charactersToAdd = 0;
            return;
        }
        charactersToAdd = Random.Range(1, 3);
        arcText.text = $"+{charactersToAdd}";
}

    void badArcLogic()
    {
        charactersToRemove = Random.Range(1, 3);
        if (currentCharacters == 1) // Player
        {
            charactersToRemove = 1;
        }
        arcText.text = $"-{charactersToRemove}";
    }

    void movement()
    {
        rb.MovePosition(rb.position + initDir * Time.fixedDeltaTime * 4);
        if(transform.position.x>=2.50f){initDir.x=-xSpeed;}
        else if(transform.position.x<=-2.50f){initDir.x=xSpeed;}
    }

    void destroyOnThreshold()
    {
        if(transform.position.z < -6) // si lo pasa al jugador
        {
            Destroy(gameObject);
        }
    }

    void disableAll()
    {
        if (GameManager.sharedInstance.gameState == GameStates.Victory)
        { gameObject.SetActive(false); }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Clone"))
        {
            if(isGoodArc)
            {
                if(other.CompareTag("Player") && GameManager.sharedInstance.livingClones == 6)
                {
                    other.gameObject.GetComponent<PlayerController>().currentHealth = 100;
                }
                else
                {
                    GameManager.sharedInstance.addCharacters(charactersToAdd);
                }
            }
            else
            {
                    GameManager.sharedInstance.removeCharacters(charactersToRemove);
            }
            Destroy(gameObject);
        }
    }


}
