using System.Collections;
using UnityEngine;

namespace RPG.Characters
{
    public abstract class AbilityBehaviour : MonoBehaviour
    {
        protected AbilityConfig config;

        const float PARTILCE_CLEAN_UP_DELAY = 20f;

        public abstract void Use(AbilityUseParams useParams);

        public void setConfig(AbilityConfig configToSet)
        {
            config = configToSet;
        }
        protected void PlaySkillParticleEffect()
        {
            var skillParticlePrefab = config.GetSkillParticleEffect();
            var skillParticleObject = Instantiate(skillParticlePrefab, transform.position, skillParticlePrefab.transform.rotation);
            //skillParticleObject.transform.parent = transform;
            skillParticleObject.GetComponent<ParticleSystem>().Play();
            StartCoroutine(DestroyParticleWhenFinished(skillParticleObject));
            Debug.Log("casting: " + skillParticlePrefab.name);
        }
        IEnumerator DestroyParticleWhenFinished(GameObject skillParticlePrefab)
        {
            while (skillParticlePrefab.GetComponent<ParticleSystem>().isPlaying)
            {
                yield return new WaitForSeconds(PARTILCE_CLEAN_UP_DELAY);
            }
            Destroy(skillParticlePrefab);
            yield return new WaitForEndOfFrame();
        }
        protected void PlayAbilitySound()
        {
            var abilitySound = config.GetRandomAbilitySound();
            var audioSource = GetComponent<AudioSource>();
            audioSource.PlayOneShot(abilitySound);
        }

    }
}