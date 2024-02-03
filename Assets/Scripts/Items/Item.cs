using UnityEngine.UIElements;

public class Item
{
    private Image itemIcon;
    private ItemType type;
    private ItemRarity rarity;
    private int quantity;
    private int buyingPrice;
    private int sellingPrice;
    private float weight;

    public Item(ItemScriptableObject ItemSO)
    {
        itemIcon.sprite = ItemSO.itemIcon;
        type = ItemSO.type;
        rarity = ItemSO.rarity;
        quantity = ItemSO.quantity;
        buyingPrice = ItemSO.buyingPrice;
        sellingPrice = ItemSO.sellingPrice;
        weight = ItemSO.weight;
    }
}
