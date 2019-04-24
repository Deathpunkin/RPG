using UnityEngine;

public enum Rarity
{
    Common,
    Uncommon,
    Rare,
    Mythical,
}

[CreateAssetMenu]
public class Item : ScriptableObject
{
    public string ItemName;
    public Sprite Icon;
    public Rarity rarity;

}
