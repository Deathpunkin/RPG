using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Characters;
using RPG.Core;

public enum ArmorType
{
    Cloth,
    Medium,
    Heavy,
}
namespace RPG.Armor
{
    public class Armor : EquippableItem
    {

        [SerializeField] GameObject armorPrefab;
        [SerializeField] ArmorType armorType;
        [SerializeField] double defence;


        public void SetDefenceMod(double modifier)
        { 
            defence *= modifier;
        }

        public double GetDefence()
        {
            return defence;
        }

    }
}