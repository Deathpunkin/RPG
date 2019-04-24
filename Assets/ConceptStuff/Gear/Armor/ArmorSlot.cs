using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmorSlot : EquipmentSlot {

    public EquipmentType EquipmentType;


    protected override void OnValidate()
    {
        base.OnValidate();
        gameObject.name = EquipmentType.ToString() + "Slot";
    }

    public override bool CanRecieveItem(Item item)
    {
        if (item == null)
        {
            return true;
        }
        EquippableItem equippableItem = item as EquippableItem;
        return equippableItem != null && equippableItem.EquipmentType == EquipmentType;

    }
}
