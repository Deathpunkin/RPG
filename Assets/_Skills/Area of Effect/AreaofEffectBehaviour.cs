using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Characters;
using RPG.Core;

public class AreaofEffectBehaviour : AbilityBehaviour
{

    // Use this for initialization
    public override void Use(AbilityUseParams useParams)
    {
        PlayAbilitySound();
        PlaySkillParticleEffect();
        DealRadialDamage(useParams);
        print("Used AoE!");
    }

    private void DealRadialDamage(AbilityUseParams useParams)
    {
        RaycastHit[] hits = Physics.SphereCastAll(
    transform.position, (config as AreaofEffectConfig).GetRadius(),
    Vector3.up,
    (config as AreaofEffectConfig).GetRadius()
    );
        foreach (RaycastHit hit in hits)
        {
            var damageable = hit.collider.gameObject.GetComponent<IDamageable>();
            bool hitPlayer = hit.collider.gameObject.GetComponent<Player>();
            if (damageable != null && !hitPlayer) //TODO rework for PvP - current No PLAYER damage
            {
                float damageToDeal = useParams.baseDamage + (config as AreaofEffectConfig).GetDamagetoEachTarget();
                damageable.TakeDamage(damageToDeal);
                print("Base Damage - " + useParams.baseDamage);
                print("AoE Damage - " + damageToDeal);
            }
        }

    }
}
