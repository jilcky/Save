using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// InventoryItem is a template for items of the Inventory. 
/// Properties/parameters of items are defined here.
/// </summary>

[CreateAssetMenu(fileName = "NewItem", menuName = "New Inventory Item", order = 51)]
public class InventoryItem: ScriptableObject
{
    //Enumeration of the consume type of an item. 
    //Moved to here from 'ItemDefinitions' because it needs some hard-coded evaluation and shall not be altered by the user. 
    public enum E_ItemConsumeType
    {
        nonConsumable,
        consumable
    }

    [Tooltip("The name of the item that can also be displayed in the UI. Please use distinguishable names for items")]
    public string itemName = "NewItem"; 
    [Tooltip("An description of the item. It can be displayed in the UI.")]
    public string description = "A new item. There was no description added.";
    [Tooltip("Add an string for the the consume of an item. E.g. 'Consume', 'Use'.")]
    public string consumeText = "Consume";

    [Tooltip("Please provide an image for the item which can be displayed in the UI.")]
    public Sprite image;
    [Tooltip("Maximal count of this item within the inventory.")]
    public int maxItems = 99;
    [Tooltip("Disable (set to false) visible if the item shall be hidden in the inventory.")]
    public bool visible = true;

    [Tooltip("Define if the item can be consumed or is non-consumable.")]
    public E_ItemConsumeType consumeType = E_ItemConsumeType.nonConsumable;
    [Tooltip("The item can be assigned to cathegories. You can define these categories yourself in the 'ItemDefinitions.cs' script. By cathegoring, e.g. only certain elements in certain user interfaces are displayed.")]
    public ItemDefinitions.E_ItemCategory category = ItemDefinitions.E_ItemCategory.none;

    [Tooltip("Define an override panel for new items. If none is selected, the default panel from the inventory script is used.")]
    public GameObject newItemOverridePanel;
    [Tooltip("Define an override panel for detail view of items. If none is selected, the default panel from the inventory script is used.")]
    public GameObject detailItemOverridePanel;

    [System.Serializable]
    public class C_ItemColors
    {
        public Color iconColor = Color.white;
        public Color primaryColor = Color.white;
        public Color secondaryColor = Color.white;
    }

    public C_ItemColors itemColors;

    //kings specific
    public ConditionsAndEffects.C_Conditions consumeConditions;
    public ConditionsAndEffects.C_Changes consumeEffect;

}

