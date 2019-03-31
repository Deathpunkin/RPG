using UnityEngine;
using UMA;
using UMA.CharacterSystem;

public enum EquipmentType
{
    Helmet,
    Chest,
    Gloves,
    Boots,
    Weapon1, //mainhand?
    Weapon2, //Offhand?
    Accessory1,
    Accessory2,
}

[CreateAssetMenu]
public class EquippableItem : Item
{
    public int StrengthBonus;
    public int AgilityBonus;
    public int IntelligenceBonus;
    public int VitalityBonus;
    [Space]
    public int StrengthPercentBonus;
    public int AgilityPercentBonus;
    public int IntelligencePercentBonus;
    public int VitalityPercentBonus;
    [Space]
    public EquipmentType EquipmentType;
    [Space]
    [SerializeField] GameObject itemPrefab;
    public UMAWardrobeRecipe recipe;
    //public UMAWardrobeRecipe femaleRecipe;

    public void Equip(Character c)
    {
        //Flat
        if (StrengthBonus != 0)
        {
            c.Strength.AddModifier(new StatModifier(StrengthBonus, StatModType.Flat, this));
        }
        if (AgilityBonus != 0)
        {
            c.Agility.AddModifier(new StatModifier(AgilityBonus, StatModType.Flat, this));
        }
        if (IntelligenceBonus != 0)
        {
            c.Intelligence.AddModifier(new StatModifier(IntelligenceBonus, StatModType.Flat, this));
        }
        if (VitalityBonus != 0)
        {
            c.Vitality.AddModifier(new StatModifier(VitalityBonus, StatModType.Flat, this));
        }
        //Percent
        if (StrengthPercentBonus != 0)
        {
            c.Strength.AddModifier(new StatModifier(StrengthPercentBonus / 100, StatModType.PercentMult, this));
        }
        if (AgilityPercentBonus != 0)
        {
            c.Agility.AddModifier(new StatModifier(AgilityPercentBonus / 100, StatModType.PercentMult, this));
        }
        if (IntelligencePercentBonus != 0)
        {
            c.Intelligence.AddModifier(new StatModifier(IntelligencePercentBonus / 100, StatModType.PercentMult, this));
        }
        if (VitalityPercentBonus != 0)
        {
            c.Vitality.AddModifier(new StatModifier(VitalityPercentBonus / 100, StatModType.PercentMult, this));
        }
    }

    public void Unequip(Character c)
    {
        c.Strength.RemoveAllModifiersFromSource(this);
        c.Agility.RemoveAllModifiersFromSource(this);
        c.Intelligence.RemoveAllModifiersFromSource(this);
        c.Vitality.RemoveAllModifiersFromSource(this);
    }
    public GameObject GetItemPrefab()
    {
        return itemPrefab;
    }
}
