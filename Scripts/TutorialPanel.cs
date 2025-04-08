using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialPanel : MonoBehaviour
{
    Text tutorialTxt;
    public int indicationIndex;
    Image celuLandScapeL;
    Image celuLandScapeR;
    Image keyARep;
    Image keyDRep;
    Image keyArrowL;
    Image keyArrowD;
    float timer;
    float delayTime;
    float delay;
    public int letterIndex;

    string text1EnCel;
    string text1EnPc;
    string text2EnCel;
    string text2EnPc;
    string text3En;
    string text4En;

    string text1EsCel;
    string text1EsPc;
    string text2EsCel;
    string text2EsPc;
    string text3Es;
    string text4Es;

    void Start()
    {
        delay=0.05f;
        letterIndex=0;
        celuLandScapeL=GameObject.Find("CeluLandScapeL").GetComponent<Image>();
        celuLandScapeR=GameObject.Find("CeluLandScapeR").GetComponent<Image>();
        keyARep=GameObject.Find("A").GetComponent<Image>();
        keyDRep=GameObject.Find("D").GetComponent<Image>();
        keyArrowL=GameObject.Find("ArrowL").GetComponent<Image>();
        keyArrowD=GameObject.Find("ArrowD").GetComponent<Image>();

        text1EnCel="Tap the left side of your screen to make the main character move to the left\nPress to continue";
        text1EnPc="Press the left arrow or the A key on your keyboard to move your character to the left\nPress one of the indicated keys";
        text2EnCel="Tap the Right side of your screen to make the main character move to the right\nPress to continue";
        text2EnPc="Press the right arrow or the D key on your keyboard to move your character to the right\nPress one of the indicated keys";
        text3En="Destroy barrels to get different weapons, avoid getting hit and the red arcs, the green arcs will provide you with support units";
        text4En="Defeat the hordes and then defeat the evil king to forge your legend\nGood luck";

        text1EsCel="Toca el lado izquierdo de tu pantalla para desplazar al personaje hacia la izquierda\nPresiona para continuar";
        text1EsPc="Toca la flecha izquierda o la tecla A de tu teclado para mover a tu personaje hacia la izquierda\nPresiona una de las teclas indicadas";
        text2EsCel="Toca el lado derecho de tu pantalla para desplazar al personaje hacia la derecha\nPresiona para continuar";
        text2EsPc="Toca la flecha derecha o la tecla D de tu teclado para mover a tu personaje hacia la derecha\nPresiona una de las teclas indicadas";
        text3Es="Destruye barriles para obtener diferentes armas, evita ser golpeado y los arcos rojos, los arcos verdes te darán compañeros de soporte";
        text4Es="Vence a la horda y derrota al malvado rey para forjar tu leyenda\nBuena suerte";

        tutorialTxt=GetComponentInChildren<Text>();
        Time.timeScale=0;
        GameManager.sharedInstance.lastLvlUnlocked++;
        GameManager.sharedInstance.saveLastLvl();
    }

    void Update()
    {
        timer=Time.realtimeSinceStartup;
        textBehaviour();
        if(indicationIndex==0&&Input.anyKeyDown||indicationIndex==0&&Input.touchCount>0)
        {indicationIndex++;
        tutorialTxt.text="";}

        if (MusicManager.sharedInstance.pc)
        {
            if (indicationIndex==1&&Input.GetAxisRaw("Horizontal")<0&&letterIndex>=text1EnCel.Length)
            { indicationIndex++;
            letterIndex=0;
            tutorialTxt.text="";}
            else if (indicationIndex==2&&Input.GetAxisRaw("Horizontal")>0&&letterIndex>=text2EnCel.Length)
            { indicationIndex++;
                letterIndex=0;
                tutorialTxt.text="";}
            else if (indicationIndex==3&&Input.anyKeyDown&&letterIndex>=text3En.Length)
            { indicationIndex++;
                letterIndex=0;
                tutorialTxt.text="";}
            else if(indicationIndex==4&&Input.anyKeyDown&&letterIndex>=text4Es.Length)
            {indicationIndex++;}
        }

        if (!MusicManager.sharedInstance.pc)
        {
            if(indicationIndex==1&&Input.touchCount>0&&letterIndex>=text1EnCel.Length)
            {
                indicationIndex++;
                letterIndex=0;
                tutorialTxt.text="";
            }
            else if(indicationIndex==2&&Input.touchCount>0&&letterIndex>=text2EnCel.Length)
            {
                indicationIndex++;
                letterIndex=0;
                tutorialTxt.text = "";
            }
            else if(indicationIndex==3&&Input.touchCount>0&&letterIndex>=text3En.Length)
            {indicationIndex++;
                letterIndex=0;
                tutorialTxt.text="";}
            else if(indicationIndex==4&&Input.touchCount>0&&letterIndex>=text4Es.Length)
            {indicationIndex++;}
        }
    }

    void textBehaviour()
    {
        if (PlayerPrefs.GetString("lang").Equals("en"))
        {
            switch (indicationIndex)
            {
                case 0:
                {tutorialTxt.text="The main character will automatically shoot to destroy obstacles and enemies";break;}
                case 1:
                if(!MusicManager.sharedInstance.pc)
                {if(delayTime<=timer&&letterIndex<text1EnCel.Length)
                {delayTime=timer+delay;
                tutorialTxt.text+=text1EnCel[letterIndex];
                letterIndex++;}
                if(letterIndex==text1EnCel.Length)
                {celuLandScapeL.enabled=true;}}
                else
                {if(delayTime<=timer&&letterIndex<text1EnPc.Length)
                {delayTime=timer+delay;
                tutorialTxt.text+=text1EnPc[letterIndex];
                letterIndex++;}
                if(letterIndex==text1EnPc.Length)
                {keyARep.enabled=true;keyArrowL.enabled=true;}}
                break;
                case 2:
                if(!MusicManager.sharedInstance.pc)
                {celuLandScapeL.enabled=false;
                if(delayTime<=timer&&letterIndex<text2EnCel.Length)
                {delayTime=timer+delay;
                tutorialTxt.text+=text2EnCel[letterIndex];
                letterIndex++;}
                if(letterIndex==text2EnCel.Length)
                {celuLandScapeR.enabled=true;}}
                else
                {keyArrowL.enabled=false;keyARep.enabled=false;
                if(delayTime<=timer&&letterIndex<text2EnPc.Length)
                {delayTime=timer+delay;
                tutorialTxt.text+=text2EnPc[letterIndex];
                letterIndex++;}
                if(letterIndex==text2EnPc.Length)
                {keyDRep.enabled=true;keyArrowD.enabled=true;}}
                break;
                case 3:
                {keyDRep.enabled=false;keyArrowD.enabled=false;celuLandScapeR.enabled=false;
                if(delayTime<=timer&&letterIndex<text3En.Length)
                {delayTime=timer+delay;
                tutorialTxt.text+=text3En[letterIndex];
                letterIndex++;}
                break;}
                case 4:
                {if(delayTime<=timer&&letterIndex<text4En.Length)
                {delayTime=timer+delay;
                tutorialTxt.text+=text4En[letterIndex];
                letterIndex++;}
                break;}
            }
        }
        else if(PlayerPrefs.GetString("lang").Equals("es"))
        {
            switch (indicationIndex)
            {
                case 0:
                {tutorialTxt.text="Tu personaje disparará automaticamente para destruir a sus obstáculos y enemigos";break;}
                case 1:
                if(!MusicManager.sharedInstance.pc)
                {if(delayTime<=timer&&letterIndex<text1EsCel.Length)
                {delayTime=timer+delay;
                tutorialTxt.text+=text1EsCel[letterIndex];
                letterIndex++;}
                if(letterIndex==text1EsCel.Length)
                {celuLandScapeL.enabled=true;}}
                else
                {if(delayTime<=timer&&letterIndex<text1EsPc.Length)
                {delayTime=timer+ delay;
                tutorialTxt.text+=text1EsPc[letterIndex];
                letterIndex++;}
                if(letterIndex==text1EsPc.Length)
                {keyARep.enabled=true;keyArrowL.enabled=true;}}break;
                case 2:
                if(!MusicManager.sharedInstance.pc)
                {celuLandScapeL.enabled=false;
                if(delayTime<=timer&&letterIndex<text2EsCel.Length)
                {delayTime=timer+delay;
                tutorialTxt.text+=text2EsCel[letterIndex];
                letterIndex++;}
                if(letterIndex==text2EsCel.Length)
                {celuLandScapeR.enabled=true;}}
                else
                {keyArrowL.enabled=false;keyARep.enabled=false;
                if(delayTime<=timer&&letterIndex<text2EsPc.Length)
                {delayTime=timer+ delay;
                tutorialTxt.text+= text2EsPc[letterIndex];
                letterIndex++;}
                if(letterIndex==text2EsPc.Length)
                {keyDRep.enabled=true;keyArrowD.enabled=true;}}break;
                case 3:
                {if(delayTime<=timer&&letterIndex<text3Es.Length)
                {delayTime=timer+ delay;
                tutorialTxt.text+= text3Es[letterIndex];
                letterIndex++;}
                keyDRep.enabled=false;keyArrowD.enabled=false;celuLandScapeR.enabled=false;break;}
                case 4:
                {if(delayTime<=timer&&letterIndex<text4Es.Length)
                {delayTime=timer+delay;
                tutorialTxt.text+=text4Es[letterIndex];
                letterIndex++;}
                break;}
            }
        }
        if(indicationIndex>4){Destroy(gameObject);Time.timeScale=1;}
    }
}