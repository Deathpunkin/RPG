using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Characters;

public class CrouchTriggerZone : MonoBehaviour {

    Player player;
    int playerLayer = 10;
    // Update is called once per frame
    void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.layer == playerLayer)
        {
            //player.m_Crouching = true;
        }
    }
}
