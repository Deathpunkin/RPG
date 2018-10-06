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

        public override void AttachComponent(GameObject gameObjectToattachTo)
        {
            var behaviourComponent = gameObjectToattachTo.AddComponent<AreaofEffectBehaviour>();
            behaviourComponent.setConfig(this);
            behaviour = behaviourComponent;
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