using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Weapons;

namespace RPG.Characters
{
    public class PowerAttackBehaviour : MonoBehaviour, ISpecialAbility
    {
        Weapon weapon;
        PowerAttackConfig config;

        public void setConfig(PowerAttackConfig configToSet)
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
            float damageToDeal = useParams.baseDamage + config.GetExtraDamage();         
            useParams.target.AdjustHealth(damageToDeal);
            DamageTextController.CreateFloatingDamageText(damageToDeal.ToString(), transform);
            print("Power Attack Damage = " + damageToDeal);
            print("Player Base Damage = " + useParams.baseDamage);
        }
    }
}