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
        [SerializeField] GameObject skillParticleEffect = null;
        [SerializeField] float energyCost = 10f;
        [Header("NEED audioclip or won't compile")]
        [SerializeField] AudioClip[] audioClips = null;
        float cooldown = 3f;
        float cooldownTimer;
        bool abilityCooldownDone = false;

        protected AbilityBehaviour behaviour;

        public abstract AbilityBehaviour GetBehaviourComponent(GameObject objectToAttachTo);

        public void AttachAbilityTo(GameObject objectToAttachTo)
        {
            AbilityBehaviour behaviourComponent = GetBehaviourComponent(objectToAttachTo);
            behaviourComponent.setConfig(this);
            behaviour = behaviourComponent;
        }

        public void Use(AbilityUseParams useParams)
        {
            behaviour.Use(useParams);
        }

        public Image GetSkillIcon()
        {
            return skillIcon;
        }

        public GameObject GetSkillParticleEffect()
        {
            return skillParticleEffect;
        }


        public float GetEnergyCost()
        {
            return energyCost;
        }

        public float GetAbilityCooldownTimer()
        {
            return cooldownTimer;
        }
        
        public AudioClip GetRandomAbilitySound()
        {
            return audioClips[Random.Range(0, audioClips.Length)];
        }
    }
}