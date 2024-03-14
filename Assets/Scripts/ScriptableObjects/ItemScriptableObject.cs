using UnityEngine.UIElements;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "ItemScriptableObject", menuName = "ScriptableObject/NewItem")]
public class ItemScriptableObject : ScriptableObject
{
    public GameObject ItemUIPrefab;
    public Sprite ItemIcon;
    public ItemType Type;
    public string ItemDescription;
    public ItemRarity Rarity;
    public int Quantity;
    public int BuyingPrice;
    public int SellingPrice;
    public float Weight;
}
