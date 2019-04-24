using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Core;
using RPG.Armor;

public class GiveItem : MonoBehaviour {

    [SerializeField] Item itemtoGive;
    [SerializeField] int amount;
    [SerializeField] string itemName;
    [SerializeField] Item randomizedItem;
    [SerializeField] Item randomizedItemtoGive;
    [SerializeField] Character player;
    [SerializeField] PlayerInput playerInput;

	// Use this for initialization
	void Start () {
        if(!player)
        {
            player = FindObjectOfType<Character>();
        }
        if(!playerInput)
        {
            playerInput = FindObjectOfType<PlayerInput>();
        }
        itemName = itemtoGive.ItemName;
    }
	
	// Update is called once per frame
	void Update () {
        {
            randomizedItem = Instantiate(itemtoGive);
            (randomizedItem as ChestArmor).SetPrefixRandom();
            (randomizedItem as ChestArmor).SetSuffixRandom();
            randomizedItemtoGive = randomizedItem;
            (randomizedItemtoGive as ChestArmor).ItemName = (randomizedItem as ChestArmor).GetPrefix() + " " + itemName + " " + (randomizedItem as ChestArmor).GetSuffix();

        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (itemtoGive is ChestArmor)
        {
            if ((randomizedItemtoGive as ChestArmor).GetSuffixList() == ArmorSuffix.ofFastCasting)
            {
                (randomizedItemtoGive as ChestArmor).recipe = (randomizedItemtoGive as ChestArmor).GetFastCastingRecipe();
            }
            if ((randomizedItemtoGive as ChestArmor).GetSuffixList() == ArmorSuffix.ofEnchanting)
            {
                (randomizedItemtoGive as ChestArmor).recipe = (randomizedItemtoGive as ChestArmor).GetOfEnchantingRecipe();
            }
            if(amount <= 1)
            {
                player.AddToInventory(randomizedItemtoGive);
            }
            else
            {
                player.AddToInventory(randomizedItemtoGive, amount);
            }
        }
        else
        {
            player.AddToInventory(itemtoGive);
        }

        if (other.gameObject == player)
        {
            if(Input.GetKeyDown(playerInput.interact))
            {
                player.AddToInventory(itemtoGive);
                Debug.Log("Interact!");
            }
        }
    }
}
