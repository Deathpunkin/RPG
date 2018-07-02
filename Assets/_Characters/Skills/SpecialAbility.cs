using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Core;

namespace RPG.Characters
{
    public struct AbilityUseParams
    {
        public IDamageable target;
        public float baseDamage;

        public AbilityUseParams(IDamageable target, float baseDamage)
        {
            this.target = target;
            this.baseDamage = baseDamage;
        }
    }
    public abstract class SpecialAbility : ScriptableObject
    {

        [Header("Special Ability General")]
        [SerializeField]
        float energyCost = 10f;
        float cooldown = 3f;
        float cooldownTimer;
        bool abilityCooldownDone = false;

        protected ISpecialAbility behaviour;

        abstract public void AttachComponent(GameObject gameObjectToattachTo);

        public void Use(AbilityUseParams useParams)
        {
            behaviour.Use(useParams);
        }

        public float GetEnergyCost()
        {
            return energyCost;
        }

        public float GetAbilityCooldownTimer()
        {
            return cooldownTimer;
        }
    }
    public interface ISpecialAbility
    {
        void Use(AbilityUseParams useParams);
    }
}