using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehaviour : MonoBehaviour
{
    GameObject cart;
    GameObject player;

    private void Start()
    {
        player=GameObject.FindObjectOfType<PlayerController>().gameObject;
        cart=GameObject.Find("Dolly Cart");
    }

    void stablishPos()
    {
        if (GameManager.sharedInstance.gameState == GameStates.Victory)
        {
            transform.position = cart.transform.position;
            transform.LookAt(player.transform);
        }
    }

    // Update is called once per frame
    void Update()
    {
        stablishPos();
    }
}
