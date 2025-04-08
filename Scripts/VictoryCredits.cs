using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class VictoryCredits : MonoBehaviour
{
    /*
     * TODO:
     * Agregar musica acorde y loopeada. El Index de esta escena es 6
     * Imagen de fondo
     */
    Canvas creditsCanvas;
    Canvas victoryCanvas;

    Text thanksTxt;
    Text unlockedLvl;
    Text creditsTxt;
    Text backTxt;

    bool _checked;
    private void Awake()
    {
        creditsCanvas = GetComponent<Canvas>();
        victoryCanvas = GameObject.Find("VictoryCanvas").GetComponent<Canvas>();
        thanksTxt = GameObject.Find("ThanksTxt").GetComponent<Text>();
        unlockedLvl = GameObject.Find("UnlockTxt").GetComponent<Text>();
        creditsTxt = GameObject.Find("CreditsTxt").GetComponent<Text>();
        backTxt = GameObject.Find("BackTxt").GetComponent<Text>();
        localizeText();
        _checked=false;
        // Force playerprefs
        //PlayerPrefs.SetInt("seen_credits", 0);
        setCreditsVisible(false);
    }

    private void Update()
    {
        if(!_checked && GameManager.sharedInstance.gameState == GameStates.Victory)
        {
            checkFirstTime();
            _checked=true;
        }
    }

    void checkFirstTime()
    {
        if (PlayerPrefs.GetInt("seen_credits", 0) == 0)
        {
            setCreditsVisible(true);
            PlayerPrefs.SetInt("seen_credits", 1);
            
        }
        else
        {
            setCreditsVisible(false);
        }
    }

    void setCreditsVisible(bool visible)
    {
        if (!visible)
        {
            creditsCanvas.enabled = false;
            victoryCanvas.enabled = true;
        }
        else
        {
            victoryCanvas.enabled = false;
            creditsCanvas.enabled = true;
        }
    }
    void localizeText()
    {
        if(PlayerPrefs.GetString("lang").Equals("es"))
        {
            thanksTxt.text = "¡Gracias por jugar!";
            unlockedLvl.text = "Se desbloqueo el Modo Infinito";
            creditsTxt.text = "Desarrolladores:\nJGameDev\nDario Camino";
            backTxt.text = "Volver al Menu";
        }
        else if(PlayerPrefs.GetString("lang").Equals("en"))
        {
            thanksTxt.text = "Thanks for playing!";
            unlockedLvl.text = "You have unlocked Endless Mode";
            creditsTxt.text = "Developers:\nJGameDev\nDario Camino";
            backTxt.text = "Back to Menu";
        }
    }
    public void backToMainMenu()
    {SceneManager.LoadScene(0);}
}
