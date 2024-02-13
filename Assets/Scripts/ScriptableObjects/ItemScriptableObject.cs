using UnityEngine.UIElements;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "ItemScriptableObject", menuName = "ScriptableObject/NewItem")]
public class ItemScriptableObject : ScriptableObject
{
    public GameObject itemUIPrefab;
    public Sprite itemIcon;
    public ItemType type;
    public string itemDescription;
    public ItemRarity rarity;
    public int quantity;
    public int buyingPrice;
    public int sellingPrice;
    public float weight;
}
