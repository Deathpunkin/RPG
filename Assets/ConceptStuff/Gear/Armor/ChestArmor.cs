using UnityEngine;
using System;
using System.ComponentModel;
using UMA;
using UMA.CharacterSystem;

namespace RPG.Armor
{
    public enum ArmorPrefix
    {
        Hardened,
        Padded,
        Lightweight,
        //Place additions above this line
        //Count,
    }
    public enum ArmorSuffix
    {
        [Description("of Fast Casting")]
        ofFastCasting,
        [Description("of Enchanting")]
        ofEnchanting,
        //Place additions above this line
        //Count,
    }
    public enum Weight
    {
        Light,
        Medium,
        Heavy,
    }
    [CreateAssetMenu(menuName = ("RPG/Armor/Chest"))]
    public class ChestArmor : Armor
    {
        [SerializeField] ArmorPrefix prefixList;
        [SerializeField] string prefix;
        [SerializeField] ArmorSuffix suffixList;
        [SerializeField] string suffix;
        [SerializeField] Weight weight;
        [SerializeField] float defenceModifier = 0;

        [SerializeField] UMAWardrobeRecipe fastCastingRecipe;
        [SerializeField] UMAWardrobeRecipe enchantingRecipe;

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

        public UMAWardrobeRecipe GetFastCastingRecipe()
        {
            return fastCastingRecipe;
        }
        public UMAWardrobeRecipe GetOfEnchantingRecipe()
        {
            return enchantingRecipe;
        }

        public ArmorPrefix GetPrefixList()
        {
            return prefixList;
        }

        public string GetPrefix()
        {
            return GetPrettyName(prefixList);
        }

        public ArmorSuffix GetSuffixList()
        {
            return suffixList;
        }

        public string GetSuffix()
        {
            return GetPrettyName(suffixList);
        }

        public Weight GetWeight()
        {
            return weight;
        }

        public void SetPrefixRandom()
        {
            prefixList = (ArmorPrefix)UnityEngine.Random.Range(0, System.Enum.GetValues(typeof(ArmorPrefix)).Length);
        }

        public void SetSuffixRandom()
        {
            suffixList = (ArmorSuffix)UnityEngine.Random.Range(0, System.Enum.GetValues(typeof(ArmorSuffix)).Length);
        }

        public void SetPrefix(ArmorPrefix _prefix)
        {
            prefixList = _prefix;
        }

        public void SetSuffix(ArmorSuffix _suffix)
        {
            suffixList = _suffix;
        }

        public float GetDefenceMod()
        {
            if (prefixList == ArmorPrefix.Hardened)
            {
                defenceModifier = 50;
                SetDefenceMod(defenceModifier);
                return defenceModifier;
            }
            if (prefixList == ArmorPrefix.Padded)
            {
                defenceModifier = 25;
                SetDefenceMod(defenceModifier);
                return defenceModifier;
            }
            return defenceModifier;
        }
    }
}