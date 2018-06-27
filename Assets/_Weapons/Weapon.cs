using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Characters;

namespace RPG.Weapons
{
    [CreateAssetMenu(menuName = ("RPG/Weapon"))]
    public class Weapon : ScriptableObject
    {
        
        [SerializeField] GameObject weaponPrefab;
        [SerializeField] AnimationClip attackAnimationMainHand;
        [SerializeField] AnimationClip attackAnimationOffHand;
        [SerializeField] AnimationClip blockAnimationMainHand;
        [SerializeField] AnimationClip blockAnimationOffHand;
        Player player;

        public Transform gripMainHandTransform; //Where to hold if Mainhand.
        public Transform gripOffHandTransform; //Where to hold if Offhand

        public GameObject GetWeaponPrefab()
        {
            return weaponPrefab;
        }

        public AnimationClip GetMainHandAttackAnimClip()
        {
            attackAnimationMainHand.events = new AnimationEvent[0]; //Remove all animation events from Weapons(for using asset packs).
            return attackAnimationMainHand;
        }
        //TODO Get OFFHAND and Block Animations working
        //public AnimationClip GetOffHandAttackAnimClip()
        //{
        //    attackAnimationOffHand.events = new AnimationEvent[0]; //Remove all animation events from Weapons(for using asset packs).
        //    return attackAnimationOffHand;
        //}
        //public AnimationClip GetMainHandBlockAnimClip()
        //{
        //    blockAnimationMainHand.events = new AnimationEvent[0]; //Remove all animation events from Weapons(for using asset packs).
        //    return blockAnimationMainHand;
        //}

        public AnimationClip GetOffHandBlockAnimClip()
        {
            blockAnimationOffHand.events = new AnimationEvent[0]; //Remove all animation events from Weapons(for using asset packs).
            return blockAnimationOffHand;
        }

    }
}