using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Characters;

namespace RPG.Weapons
{
    [CreateAssetMenu(menuName = ("RPG/Weapon"))]
    public class Staff : Weapon
    {

        [SerializeField] GameObject weaponPrefab;
        [SerializeField] AnimationClip attackAnimationMainHand;
        [SerializeField] AnimationClip attackAnimationOffHand;
        [SerializeField] AnimationClip blockAnimationMainHand;
        [SerializeField] AnimationClip blockAnimationOffHand;
        [SerializeField] AnimationClip deathAnimation;
        [SerializeField] AnimationClip reviveAnimation;
        Player player;
        [SerializeField] float attackSpeed = .5f;
        [SerializeField] float maxAttackRange = 2f;
        [SerializeField] float minDamagePerHit = 10f;
        [SerializeField] float maxDamagePerHit = 20f;
        [SerializeField] DamageType damageType;
        [SerializeField] AudioSource audioSource;
        [SerializeField] AudioClip hitSound;
        [SerializeField] AudioClip missSound;
        [SerializeField] AudioClip blockSound;

        void start()
        {
            audioSource = weaponPrefab.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }


    }
}