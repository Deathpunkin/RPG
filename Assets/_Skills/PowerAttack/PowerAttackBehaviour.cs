using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Weapons;

namespace RPG.Characters
{
    public class PowerAttackBehaviour : AbilityBehaviour
    {
        Weapon weapon;
        PowerAttackConfig config;

        public void setConfig(PowerAttackConfig configToSet)
        {
            this.config = configToSet;
        }

        public override void Use(AbilityUseParams useParams)
        {
            float damageToDeal = useParams.baseDamage + config.GetExtraDamage();         
            useParams.target.TakeDamage(damageToDeal);
            DamageTextController.CreateFloatingDamageText(damageToDeal.ToString(), transform);
            print("Power Attack Damage = " + damageToDeal);
            print("Player Base Damage = " + useParams.baseDamage);
        }
    }
}