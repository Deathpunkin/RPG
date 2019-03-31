using System;
using UnityEngine;
using UMA;
using UMA.CharacterSystem;

public class EquipmentPanel : MonoBehaviour
{
    [SerializeField] Transform equipementSlotsParent;
    [SerializeField] EquipmentSlot[] equipmentSlots;
    [SerializeField] DynamicCharacterAvatar _player;

    [SerializeField] GameObject currentMainHand;
    GameObject _item;


    public event Action<ItemSlot> OnPointerEnterEvent;
    public event Action<ItemSlot> OnPointerExitEvent;
    public event Action<ItemSlot> OnRightClickEvent;
    public event Action<ItemSlot> OnBeginDragEvent;
    public event Action<ItemSlot> OnDragEvent;
    public event Action<ItemSlot> OnEndDragEvent;
    public event Action<ItemSlot> OnDropEvent;

    private void Start()
    {
        for (int i = 0; i < equipmentSlots.Length; i++)
        {
            equipmentSlots[i].OnPointerEnterEvent += OnPointerEnterEvent;
            equipmentSlots[i].OnPointerExitEvent += OnPointerExitEvent;
            equipmentSlots[i].OnDoubleClickEvent += OnRightClickEvent;
            equipmentSlots[i].OnBeginDragEvent += OnBeginDragEvent;
            equipmentSlots[i].OnDragEvent += OnDragEvent;
            equipmentSlots[i].OnEndDragEvent += OnEndDragEvent;
            equipmentSlots[i].OnDropEvent += OnDropEvent;
        }
    }

    private void OnValidate()
    {
        equipmentSlots = equipementSlotsParent.GetComponentsInChildren<EquipmentSlot>();
    }

    public bool AddItem(EquippableItem item, out EquippableItem previousItem)
    {
        for (int i = 0; i < equipmentSlots.Length; i++)
        {
            if (equipmentSlots[i].EquipmentType == item.EquipmentType)
            {
                previousItem = (EquippableItem)equipmentSlots[i].Item;
                equipmentSlots[i].Item = item;
                return true;
            }
            if (equipmentSlots[i] != null)
            {
                UpdateCharacterEquip(equipmentSlots[i], item);
            }
            //Equip Gameobject
            if (equipmentSlots[3] != item)
            {
                Transform hand = _player.umaData.skeleton.GetBoneGameObject(UMASkeleton.StringToHash("RightHand")).transform;
                if (currentMainHand != item.GetItemPrefab() | currentMainHand == null)
                {
                    if(currentMainHand != null & currentMainHand != item.GetItemPrefab())
                    {
                        Destroy(_item.gameObject);
                    }
                    _item = item.GetItemPrefab();
                    _item = Instantiate(item.GetItemPrefab(), hand);
                    _item.transform.SetParent(hand);
                    _item.transform.localPosition = hand.transform.localPosition - new Vector3(-0.158f, 0.1f, 0.05f); //new Vector3(-0.158f, 0.372f, -0.02f);
                    _item.transform.localRotation = Quaternion.Euler(new Vector3(-0.02f, 356f, 8.12f));
                    currentMainHand = item.GetItemPrefab();
                }
            }
        }
        previousItem = null;
        return false;
    }

    public void UpdateCharacterEquip(EquipmentSlot slot, EquippableItem item)
    {
        for (int i = 0; i < equipmentSlots.Length; i++)
        {
            if (item.recipe == null)
            {
                return;
            }
            else
            {
                _player.SetSlot(equipmentSlots[i].EquipmentType.ToString(), item.recipe.name);
                //_player.SetSlot(equipmentSlots[i].EquipmentType.ToString(), item.femaleRecipe.name);

                _player.BuildCharacter();
            }
        }
    }

    public bool RemoveItem(EquippableItem item)
    {
        for (int i = 0; i < equipmentSlots.Length; i++)
        {
            if (equipmentSlots[i].Item == item)
            {
                equipmentSlots[i].Item = null;
                _player.ClearSlot(equipmentSlots[i].EquipmentType.ToString());
                _player.BuildCharacter();
                return true;
            }
        }
        return false;
    }
}
