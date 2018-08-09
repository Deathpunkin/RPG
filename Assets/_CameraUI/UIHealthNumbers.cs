using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RPG.Characters;

namespace RPG.CameraUI
{
    public class UIHealthNumbers : MonoBehaviour
    {

        Player player;
        Text healthNumbers;
        float currentHealthPoints;
        float maxHealthPoints;
        // Use this for initialization
        void Start()
        {
            healthNumbers = GetComponent<Text>();
            player = FindObjectOfType<Player>();
            currentHealthPoints = player.currentHealthPoints;
            maxHealthPoints = player.maxHealthPoints;
            healthNumbers.text = "Health " + currentHealthPoints + "/" + maxHealthPoints;

        }

        // Update is called once per frame
        public void Update()
        {
            print("Healthnumberupdate");
            healthNumbers.text = player.currentHealthPoints.ToString("F0") + "/" + player.maxHealthPoints.ToString("F0");
        }
    }
}