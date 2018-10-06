using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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
    public abstract class AbilityConfig : ScriptableObject
    {

        [Header("Special Ability General")]
        [SerializeField] Image skillIcon = null;
        [SerializeField] GameObject skillEffect = null;
        [SerializeField] float energyCost = 10f;
        [SerializeField] AudioClip audioClip = null;
        float cooldown = 3f;
        float cooldownTimer;
        bool abilityCooldownDone = false;

        protected AbilityBehaviour behaviour;

        abstract public void AttachComponent(GameObject gameObjectToattachTo);

        public void Use(AbilityUseParams useParams)
        {
            behaviour.Use(useParams);
        }

        public Image GetSkillIcon()
        {
            return skillIcon;
        }

        public GameObject GetSkillEffect()
        {
            return skillEffect;
        }


        public float GetEnergyCost()
        {
            return energyCost;
        }

        public float GetAbilityCooldownTimer()
        {
            return cooldownTimer;
        }
        
        public AudioClip GetAudioClip()
        {
            return audioClip;
        }
    }
}