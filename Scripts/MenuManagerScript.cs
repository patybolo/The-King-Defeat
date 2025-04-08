using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManagerScript : MonoBehaviour
{
    Canvas menuCanvas;
    Canvas languageCanvas;
    Canvas creditsCanvas;
    Canvas selectLevelCanvas;
    Text playTxt;
    Text languageTxt;
    Text creditsTxt;
    Text creditsInformationTxt;
    Text englishTxt;
    Text spanishTxt;
    Text back1Txt;
    Text back2Txt;
    Text back3Txt;
    Text consoleButtonTxt;
    public bool showButtonConsole;
    public bool showXButton;
    GameObject xButton;

    private void Awake()
    {
        xButton = GameObject.Find("ExitButton");
        menuCanvas=GameObject.Find("MenuCanvas").GetComponent<Canvas>();
        languageCanvas=GameObject.Find("LanguageCanvas").GetComponent<Canvas>();
        creditsCanvas=GameObject.Find("CreditsCanvas").GetComponent<Canvas>();
        selectLevelCanvas=GameObject.Find("SelectLevelCanvas").GetComponent<Canvas>();
        playTxt=GameObject.Find("playTxt").GetComponent<Text>();
        languageTxt=GameObject.Find("languageTxt").GetComponent<Text>();
        creditsTxt=GameObject.Find("creditsTxt").GetComponent<Text>();
        creditsInformationTxt=GameObject.Find("creditsInformationTxt").GetComponent<Text>();
        englishTxt =GameObject.Find("englishTxt").GetComponent<Text>();
        spanishTxt=GameObject.Find("spanishTxt").GetComponent<Text>();
        back1Txt=GameObject.Find("back1Txt").GetComponent<Text>();
        back2Txt=GameObject.Find("back2Txt").GetComponent<Text>();
        back3Txt=GameObject.Find("back3Txt").GetComponent<Text>();
        consoleButtonTxt=GameObject.Find("ConsoleButtonTxt").GetComponent<Text>();
        localizeText();
    }

    private void Start()
    {
        MusicManager.sharedInstance.lvlChange.Invoke();
        if(MusicManager.sharedInstance.pc)
        {consoleButtonTxt.text="PC";}
        else{consoleButtonTxt.text="Cel";}
        if(PlayerPrefs.GetInt("LEVEL_KEY")>=2)
        {GameObject.Find("Lvl2").GetComponent<Button>().interactable=true;}
        if(PlayerPrefs.GetInt("LEVEL_KEY")>=3)
        {GameObject.Find("Lvl3").GetComponent<Button>().interactable=true;}
        if(PlayerPrefs.GetInt("LEVEL_KEY")>=4)
        {GameObject.Find("Lvl4").GetComponent<Button>().interactable=true;}
        if(PlayerPrefs.GetInt("LEVEL_KEY")>=5)
        {GameObject.Find("Lvl5").GetComponent<Button>().interactable=true;}
        if(PlayerPrefs.GetInt("LEVEL_KEY")>=6)
        {GameObject.Find("EndlessLvl").GetComponent<Button>().interactable=true;}
        if(!showButtonConsole){consoleButtonTxt.transform.parent.gameObject.SetActive(false);}
        if(!showXButton){xButton.SetActive(false);}
    }

    public void lvlSelector() 
    {if (PlayerPrefs.GetInt("LEVEL_KEY") > 0)
        {
            selectLevelCanvas.sortingOrder = 1;
            menuCanvas.sortingOrder = 0;
            languageCanvas.sortingOrder = 0;
            creditsCanvas.sortingOrder = 0;
        }
        else{SceneManager.LoadScene(1);}
    }
    public void languageMenu()
    {
        selectLevelCanvas.sortingOrder = 0;
        menuCanvas.sortingOrder = 0;
        languageCanvas.sortingOrder = 1;
        creditsCanvas.sortingOrder = 0;
    }

    public void creditsMenu()
    {
        selectLevelCanvas.sortingOrder = 0;
        menuCanvas.sortingOrder = 0;
        languageCanvas.sortingOrder = 0;
        creditsCanvas.sortingOrder = 1;
    }

    public void backToMainMenu()
    {
        selectLevelCanvas.sortingOrder = 0;
        menuCanvas.sortingOrder = 1;
        languageCanvas.sortingOrder = 0;
        creditsCanvas.sortingOrder = 0;
    }

    public void english()
    {
        PlayerPrefs.SetString("lang","en");
        PlayerPrefs.Save();
        localizeText();
    }

    public void spanish()
    {
        PlayerPrefs.SetString("lang","es");
        PlayerPrefs.Save();
        localizeText();
    }

    public void goToLvl(int lvlIndex)
    {SceneManager.LoadScene(lvlIndex);}

    void localizeText()
    {
        if (PlayerPrefs.GetString("lang").Equals("en"))
        {
            playTxt.text = "Play";
            languageTxt.text = "Language";
            creditsTxt.text = "+Games";
            creditsInformationTxt.text = "Designers and programmers:\r\nJGameDev\r\nDario Camino";
            englishTxt.text = "English";
            spanishTxt.text = "Spanish";
            back1Txt.text = "Back";
            back2Txt.text = "Back";
            back3Txt.text = "Back";
        }
        else if (PlayerPrefs.GetString("lang").Equals("es"))
        {
            playTxt.text = "Jugar";
            languageTxt.text = "Idioma";
            creditsTxt.text = "+Juegos";
            creditsInformationTxt.text = "Diseñadores y programadores:\r\nJGameDev\r\nDario Camino";
            englishTxt.text = "Inglés";
            spanishTxt.text = "Español";
            back1Txt.text = "Volver";
            back2Txt.text = "Volver";
            back3Txt.text = "Volver";
        }
    }

    public void ConsoleButton()
    {MusicManager.sharedInstance.changePlatform();
    if(MusicManager.sharedInstance.pc)
    {consoleButtonTxt.text="PC";}
    else{consoleButtonTxt.text="Cel";}}

    public void urlLink(String dom){Application.OpenURL(dom);}

    public void quit(){Application.Quit();}
}
