using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Weapons;

namespace RPG.Characters
{
    public class HealingBehaviour : MonoBehaviour, ISpecialAbility
    {
        HealingConfig config;
        Player player;
        ParticleSystem skillEffect;

        public void setConfig(HealingConfig configToSet)
        {
            this.config = configToSet;
        }

        // Use this for initialization
        void Start()
        {
            player = GetComponent<Player>();
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void Use(AbilityUseParams useParams)
        {
            HealParticleEffect();
            if(player.currentHealthPoints != player.maxHealthPoints)
            {
                player.AdjustHealth(-config.GetHealthGained());
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
            var prefab = Instantiate(config.GetSkillEffect(), transform.position, Quaternion.Euler(-90, 0, 0));
            skillEffect = prefab.GetComponent<ParticleSystem>();
            skillEffect.Play();
            Destroy(prefab, skillEffect.main.duration);
        }
    }
}