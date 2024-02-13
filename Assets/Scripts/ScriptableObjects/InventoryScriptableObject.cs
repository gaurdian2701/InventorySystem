using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "InventoryScriptableObject", menuName = "ScriptableObject/NewInventory")]
public class InventoryScriptableObject : ScriptableObject
{
    public int itemLimit;
    public float weightLimit;
}
