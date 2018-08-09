using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.Characters
{
    [CreateAssetMenu(menuName = ("RPG/Special Ability/Power Attack"))]
    public class PowerAttackConfig : SpecialAbility
    {
        [Header("Power Attack Config")]
        [SerializeField] float extraDamage = 10f;
        [SerializeField] float range = 5f;

        public override void AttachComponent(GameObject gameObjectToattachTo)
        {
            var behaviourComponent = gameObjectToattachTo.AddComponent<PowerAttackBehaviour>();
            behaviourComponent.setConfig(this);
            behaviour = behaviourComponent;
        }
        public float GetExtraDamage()
        {
            return extraDamage;
        }
        public float getRange()
        {
            return range;
        }
    }
}