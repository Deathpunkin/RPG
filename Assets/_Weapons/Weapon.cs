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
        [SerializeField] AnimationClip deathAnimation;
        [SerializeField] AnimationClip reviveAnimation;
        Player player;
        public Transform gripMainHandTransform; //Where to hold if Mainhand.
        public Transform gripOffHandTransform; //Where to hold if Offhand
        [SerializeField] float attackSpeed = .5f;
        [SerializeField] float maxAttackRange = 2f;
        [SerializeField] float minDamagePerHit = 10f;
        [SerializeField] float maxDamagePerHit = 20f;
        [SerializeField] AudioSource audioSource;
        [SerializeField] AudioClip hitSound;
        [SerializeField] AudioClip missSound;
        [SerializeField] AudioClip blockSound;

        void start()
        {
            audioSource = weaponPrefab.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }

        public AudioSource GetWeaponAudioSouce()
        {
            return audioSource; 
        }

        public float GetAttackSpeed()
        {
            return attackSpeed;
        }

        public float GetMaxAttackRange()
        {
            return maxAttackRange;
        }

        public float GetMinDamagePerHit() // TODO consider removing "PerHit"
        {
            return minDamagePerHit;
        }

        public float GetMaxDamagePerHit()
        {
            return maxDamagePerHit;
        }

        public GameObject GetWeaponPrefab()
        {
            return weaponPrefab;
        }

        public AudioClip GetWeaponHitSound()
        {
            return hitSound;
        }

        public AudioClip GetWeaponMissSound()
        {
            return missSound;
        }

        public AudioClip GetWeaponBlockSound()
        {
            return blockSound;
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

        public AnimationClip GetDeathAnimClip()
        {
            return deathAnimation;
        }

        public AnimationClip GetReviveAnimClip()
        {
            return reviveAnimation;
        }
    }
}