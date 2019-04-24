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
            c.GetStrength().AddModifier(new StatModifier(StrengthBonus, StatModType.Flat, this));
        }
        if (AgilityBonus != 0)
        {
            c.GetAgility().AddModifier(new StatModifier(AgilityBonus, StatModType.Flat, this));
        }
        if (IntelligenceBonus != 0)
        {
            c.GetIntelligence().AddModifier(new StatModifier(IntelligenceBonus, StatModType.Flat, this));
        }
        if (VitalityBonus != 0)
        {
            c.GetVitality().AddModifier(new StatModifier(VitalityBonus, StatModType.Flat, this));
        }
        //Percent
        if (StrengthPercentBonus != 0)
        {
            c.GetStrength().AddModifier(new StatModifier(StrengthPercentBonus / 100, StatModType.PercentMult, this));
        }
        if (AgilityPercentBonus != 0)
        {
            c.GetAgility().AddModifier(new StatModifier(AgilityPercentBonus / 100, StatModType.PercentMult, this));
        }
        if (IntelligencePercentBonus != 0)
        {
            c.GetIntelligence().AddModifier(new StatModifier(IntelligencePercentBonus / 100, StatModType.PercentMult, this));
        }
        if (VitalityPercentBonus != 0)
        {
            c.GetVitality().AddModifier(new StatModifier(VitalityPercentBonus / 100, StatModType.PercentMult, this));
        }
        
    }

    public void Unequip(Character c)
    {
        c.GetStrength().RemoveAllModifiersFromSource(this);
        c.GetAgility().RemoveAllModifiersFromSource(this);
        c.GetIntelligence().RemoveAllModifiersFromSource(this);
        c.GetVitality().RemoveAllModifiersFromSource(this);
    }
    public GameObject GetItemPrefab()
    {
        return itemPrefab;
    }
}
