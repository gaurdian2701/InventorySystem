using UnityEngine.UIElements;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemScriptableObject", menuName = "ScriptableObject/NewItem")]
public class ItemScriptableObject : ScriptableObject
{
    public GameObject itemUIPrefab;
    public Sprite itemIcon;
    public ItemType type;
    public ItemRarity rarity;
    public int quantity;
    public int buyingPrice;
    public int sellingPrice;
    public float weight;
}
