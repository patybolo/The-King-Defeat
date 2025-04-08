using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour
{
    public static MusicManager sharedInstance;
    AudioSource lvlAudioSource;
    public UnityEvent lvlChange;
    public AudioClip[]lvlSongs;
    public bool pc;

    private void Awake()
    {
        if(MusicManager.sharedInstance!=null)
        {Destroy(gameObject);}
        else
        {sharedInstance=this;
        DontDestroyOnLoad(gameObject);}
        lvlAudioSource=GetComponent<AudioSource>();
    }

    private void Start()
    {lvlChange=new UnityEvent();
    lvlChange.AddListener(changeTrack);
    changeTrack();
    PlayerPrefs.SetString("lang","en");}

    private void Update()
    {if (SceneManager.GetActiveScene().buildIndex!=0&&GameManager.sharedInstance.gameState==GameStates.Victory)
        {lvlAudioSource.volume-=Time.deltaTime * 0.5f;}
        else
        {lvlAudioSource.volume=0.3f;}
    }

    void changeTrack()
    {int index=SceneManager.GetActiveScene().buildIndex;
    lvlAudioSource.clip=lvlSongs[index];
    lvlAudioSource.Play();}

    public void changePlatform()
    {if(pc){pc=false;}
    else{pc=true;}}
}
