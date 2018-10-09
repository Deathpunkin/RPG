using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.Characters
{
    [CreateAssetMenu(menuName = ("RPG/Special Ability/Healing"))]
    public class HealingConfig : AbilityConfig
    {
        [Header("Healing Config")]
        [SerializeField] float healthGained = 30;

        public override AbilityBehaviour GetBehaviourComponent(GameObject objectToAttachTo)
        {
            return objectToAttachTo.AddComponent<HealingBehaviour>();
        }

        public float GetHealthGained()
        {
            return healthGained;
        }
    }
}