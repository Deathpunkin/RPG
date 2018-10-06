using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Characters;
using RPG.Core;

public class AreaofEffectBehaviour : AbilityBehaviour
{

    AreaofEffectConfig config = null;
    ParticleSystem skillEffect = null;
    AudioSource audioSource = null;


    public void setConfig(AreaofEffectConfig configToSet)
    {
        this.config = configToSet;
    }
    // Use this for initialization
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public override void Use(AbilityUseParams useParams)
    {
        AoEParticleEffect();
        audioSource.clip = config.GetAudioClip();
        audioSource.Play();

        print("Used AoE!");
        RaycastHit[] hits = Physics.SphereCastAll(
            transform.position, config.GetRadius(),
            Vector3.up,
            config.GetRadius()
            );
        foreach (RaycastHit hit in hits)
        {
            var damageable = hit.collider.gameObject.GetComponent<IDamageable>();
            bool hitPlayer = hit.collider.gameObject.GetComponent<Player>();
            if (damageable != null && !hitPlayer) //TODO rework for PvP - current No PLAYER damage
            {
                float damageToDeal = useParams.baseDamage + config.GetDamagetoEachTarget();
                damageable.TakeDamage(damageToDeal);
                print("Base Damage - " + useParams.baseDamage);
                print("AoE Damage - " + damageToDeal);
            }
        }
    }
    public void AoEParticleEffect()
    {
        var SkillEffectPrefab = config.GetSkillEffect();
        var prefab = Instantiate(SkillEffectPrefab, transform.position, SkillEffectPrefab.transform.rotation);
        prefab.transform.parent = transform;
        skillEffect = prefab.GetComponent<ParticleSystem>();
        skillEffect.Play();
        Destroy(prefab, skillEffect.main.duration);
    }

}
