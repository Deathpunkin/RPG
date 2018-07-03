using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Weapons;

namespace RPG.Characters
{
    public class HealingBehaviour : MonoBehaviour, ISpecialAbility
    {
        Weapon weapon;
        HealingConfig config;

        public void setConfig(HealingConfig configToSet)
        {
            this.config = configToSet;
        }

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void Use(AbilityUseParams useParams)
        {
            float healthGained = config.GetHealthGained();
            useParams.target.TakeDamage(healthGained);
            DamageTextController.CreateFloatingDamageText(healthGained.ToString(), transform);
            print("Healing Skill = " + healthGained);
        }
    }
}