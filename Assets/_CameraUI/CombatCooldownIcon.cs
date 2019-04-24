using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.Characters
{
    [RequireComponent(typeof(Image))]
    public class CombatCooldownIcon : MonoBehaviour
    {
        Image combatCooldown;
        Character player;

        // Use this for initialization
        void Start()
        {
            player = FindObjectOfType<Character>();
            combatCooldown = GetComponent<Image>();
        }

        // Update is called once per frame
        void Update()
        {
            combatCooldown.fillAmount = player.GetRegenHealthDelay() - (Time.time - player.timeSinceLastDamaged);
        }
    }
}