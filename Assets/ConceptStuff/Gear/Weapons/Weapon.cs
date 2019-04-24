using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using RPG.Characters;
public enum WeaponPrefix
{
    Vampiric,
    Piercing,
    Sharpened,
    //Place additions above this line
    //Count,
}
public enum WeaponSuffix
{
    [Description("of Fast Casting")]
    ofFastCasting,
    [Description("of Enchanting")]
    ofEnchanting,
    //Place additions above this line
    //Count,
}


public class Weapon : EquippableItem {

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
    [SerializeField] WeaponPrefix prefixList;
    [SerializeField] string prefix;
    [SerializeField] WeaponSuffix suffixList;
    [SerializeField] string suffix;


    void start()
    {
        audioSource = weaponPrefab.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
    }

    public static string GetPrettyName(System.Enum e)
    {
        var nm = e.ToString();
        var tp = e.GetType();
        var field = tp.GetField(nm);
        var attrib = System.Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;

        if (attrib != null)
            return attrib.Description;
        else
            return nm;
    }
    public WeaponPrefix GetPrefixList()
    {
        return prefixList;
    }

    public string GetPrefix()
    {
        return GetPrettyName(prefixList);
    }

    public WeaponSuffix GetSuffixList()
    {
        return suffixList;
    }

    public string GetSuffix()
    {
        return GetPrettyName(suffixList);
    }

    public void SetPrefixRandom()
    {
        prefixList = (WeaponPrefix)UnityEngine.Random.Range(0, System.Enum.GetValues(typeof(WeaponPrefix)).Length);
    }

    public void SetSuffixRandom()
    {
        suffixList = (WeaponSuffix)UnityEngine.Random.Range(0, System.Enum.GetValues(typeof(WeaponSuffix)).Length);
    }

    public void SetPrefix(WeaponPrefix _prefix)
    {
        prefixList = _prefix;
    }

    public void SetSuffix(WeaponSuffix _suffix)
    {
        suffixList = _suffix;
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

    public DamageType GetDamageType()
    {
        return damageType;
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
