using System;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Core
{
    public class Inventory : MonoBehaviour
    {
        [SerializeField] List<Item> startingItems;
        [SerializeField] Transform itemsParent;
        [SerializeField] ItemSlot[] itemSlots;

        public event Action<ItemSlot> OnPointerEnterEvent;
        public event Action<ItemSlot> OnPointerExitEvent;
        public event Action<ItemSlot> OnRightClickEvent;
        public event Action<ItemSlot> OnBeginDragEvent;
        public event Action<ItemSlot> OnDragEvent;
        public event Action<ItemSlot> OnEndDragEvent;
        public event Action<ItemSlot> OnDropEvent;

        private void Start()
        {
            for (int i = 0; i < itemSlots.Length; i++)
            {
                itemSlots[i].OnPointerEnterEvent += OnPointerEnterEvent;
                itemSlots[i].OnPointerExitEvent += OnPointerExitEvent;
                itemSlots[i].OnDoubleClickEvent += OnRightClickEvent;
                itemSlots[i].OnBeginDragEvent += OnBeginDragEvent;
                itemSlots[i].OnDragEvent += OnDragEvent;
                itemSlots[i].OnEndDragEvent += OnEndDragEvent;
                itemSlots[i].OnDropEvent += OnDropEvent;
            }
            SetStartingItems();
        }

        private void OnValidate()
        {
            if (itemsParent != null)
            {
                itemSlots = itemsParent.GetComponentsInChildren<ItemSlot>();
            }
            SetStartingItems();
        }

        private void SetStartingItems()
        {
            int i = 0;
            for (; i < startingItems.Count && i < itemSlots.Length; i++)
            {
                itemSlots[i].Item = startingItems[i];
            }

            for (; i < itemSlots.Length; i++)
            {
                itemSlots[i].Item = null;
            }
        }

        public bool AddItem(Item item)
        {
            for (int i = 0; i < itemSlots.Length; i++)
            {
                if (itemSlots[i].Item == null)
                {
                    itemSlots[i].Item = item;
                    return true;
                }
            }
            return false;
        }
        public bool AddItem(Item item, int amount)
        {
            bool _bool = false;
            for (int _c = 0; _c < amount; _c++)
            {
                for (int i = 0; i < itemSlots.Length; i++)
                {
                    if (itemSlots[i].Item == null)
                    {
                        itemSlots[i].Item = item;
                        _bool = true;
                    }
                }
                _bool = false;
            }
            return _bool;
        }


        public bool RemoveItem(Item item)
        {
            for (int i = 0; i < itemSlots.Length; i++)
            {
                if (itemSlots[i].Item == item)
                {
                    itemSlots[i].Item = null;
                    return true;
                }
            }
            return false;
        }

        public bool IsFull()
        {
            for (int i = 0; i < itemSlots.Length; i++)
            {
                return false;
            }
            return true;
        }
    }
}