using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.Characters
{
    [CreateAssetMenu(menuName = ("RPG/Special Ability/Power Attack"))]
    public class PowerAttackConfig : AbilityConfig
    {
        [Header("Power Attack Config")]
        [SerializeField] float extraDamage = 10f;
        [SerializeField] float range = 5f;

        public override AbilityBehaviour GetBehaviourComponent(GameObject objectToAttachTo)
        {
            return objectToAttachTo.AddComponent<PowerAttackBehaviour>();
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