using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory_ChangeItem : MonoBehaviour
{
    [System.Serializable]
    public enum E_ItemModificationType
    {
        add,
        set
    }

    [System.Serializable]
    public class itemModifier
    {
        public E_ItemModificationType modificationType = E_ItemModificationType.add;
        public InventoryItem item;

        //Adding all this item-values to enable switching by the designer and saving it by the internal serialization.
        public int itemAmountAdd = 1;
        public int itemAmountSet = 0;
    }

    public itemModifier[] itemChanges;

    //execute a single item modification
    public static void executeItemChange(Inventory_ChangeItem.itemModifier im)
    {
        switch (im.modificationType)
        {
            case E_ItemModificationType.add:
                Inventory.instance.AddItemAmount(im.item, im.itemAmountAdd);
                break;
            case E_ItemModificationType.set:
                Inventory.instance.SetItemAmount(im.item, im.itemAmountSet);
                break;
            default:
                Debug.LogWarning("The Item modification type '" + im.modificationType.ToString() + "' is unknown.");
                break;
        }
    }

    public void AddItems()
    {
        foreach(itemModifier im in itemChanges)
        {
            executeItemChange(im);
        }
    }
}
