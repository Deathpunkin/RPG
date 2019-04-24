using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RPG.Characters;

namespace RPG.CameraUI
{
    public class UIHealthNumbers : MonoBehaviour
    {
        Player _player;
        Character player;
        Text healthNumbers;
        float currentHealthPoints;
        float maxHealthPoints;
        // Use this for initialization
        void Start()
        {
            healthNumbers = GetComponent<Text>();
            _player = FindObjectOfType<Player>();
            player = _player.GetComponent<Character>();
            currentHealthPoints = player.GetCurrentHealth();
            maxHealthPoints = player.GetMaxHealth();
            healthNumbers.text = "Health " + currentHealthPoints + "/" + maxHealthPoints;

        }

        // Update is called once per frame
        public void Update()
        {
            healthNumbers.text = player.GetCurrentHealth().ToString("F0") + "/" + player.GetMaxHealth().ToString("F0");
        }
    }
}