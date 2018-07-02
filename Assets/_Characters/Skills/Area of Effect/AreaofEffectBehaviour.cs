using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Characters;
using RPG.Core;

public class AreaofEffectBehaviour : MonoBehaviour, ISpecialAbility
{

    AreaofEffectConfig config;

    public void setConfig(AreaofEffectConfig configToSet)
    {
        this.config = configToSet;
    }
    // Use this for initialization
    void Start()
    {

    }

    public void Use(AbilityUseParams useParams)
    {
        print("Used AoE!");
        RaycastHit[] hits = Physics.SphereCastAll(
            transform.position, config.GetRadius(),
            Vector3.up,
            config.GetRadius()
            );
        foreach (RaycastHit hit in hits)
        {
            var damageable = hit.collider.gameObject.GetComponent<IDamageable>();
            if (damageable != null)
            {
                float damageToDeal = useParams.baseDamage + config.GetDamagetoEachTarget();
                damageable.TakeDamage(damageToDeal);
                print("Base Damage - " + useParams.baseDamage);
                print("AoE Damage - " + damageToDeal);
            }
        }
    }
}
