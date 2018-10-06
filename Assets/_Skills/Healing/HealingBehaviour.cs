using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Weapons;

namespace RPG.Characters
{
    public class HealingBehaviour : AbilityBehaviour
    {
        HealingConfig config = null;
        Player player = null;
        ParticleSystem skillEffect = null;
        AudioSource audioSource = null;

        public void setConfig(HealingConfig configToSet)
        {
            this.config = configToSet;
        }

        // Use this for initialization
        void Start()
        {
            player = GetComponent<Player>();
            audioSource = GetComponent<AudioSource>();
        }

        public override void Use(AbilityUseParams useParams)
        {
            HealParticleEffect();
            audioSource.clip = config.GetAudioClip();
            audioSource.Play();
            if(player.currentHealthPoints != player.maxHealthPoints)
            {
                player.Heal(config.GetHealthGained());
                DamageTextController.CreateFloatingHealingText(config.GetHealthGained().ToString(), transform);
                print("Healing Skill = " + -config.GetHealthGained());
            }
            else
            {
                return;
            }
        }

        public void HealParticleEffect()
        {
            var SkillEffectPrefab = config.GetSkillEffect();
            var prefab = Instantiate(SkillEffectPrefab, transform.position, SkillEffectPrefab.transform.rotation);
            prefab.transform.parent = transform;
            skillEffect = prefab.GetComponent<ParticleSystem>();
            skillEffect.Play();
            Destroy(prefab, skillEffect.main.duration);
        }
    }
}