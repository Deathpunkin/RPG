using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Characters
{
    [CreateAssetMenu(menuName = ("RPG/Special Ability/AoE Attack"))]
    public class AreaofEffectConfig : AbilityConfig
    {
        [Header("AoE Config")]
        [SerializeField] float radius = 5f;
        [SerializeField] float damageToEachTarget = 15f;

        public override AbilityBehaviour GetBehaviourComponent(GameObject objectToAttachTo)
        {
            return objectToAttachTo.AddComponent<AreaofEffectBehaviour>();
        }

        public float GetRadius()
        {
            return radius;
        }
        public float GetDamagetoEachTarget()
        {
            return damageToEachTarget;
        }
    }
}