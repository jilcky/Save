using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Script for linking the properties of an inventory item to the user interface and back (consume).
/// </summary>

public class Inventory_UIItem : MonoBehaviour
{
    [ReadOnlyInspector] public Inventory.C_ItemAmount itemAmount;

    [Tooltip("Please specify a text element to show the current amount of this element.")]
    public Text amountText;
    [Tooltip("Please specify an image element to show the sprite of this element.")]
    public Image itemImage;
    [Tooltip("Please specify a text element to show the item name.")]
    public Text itemText;
    [Tooltip("Please specify a text element to show the item description.")]
    public Text descriptionText;
    [Tooltip("Please specify a UI button element to enable consumption of the item. The button property 'interactable' is automatically controlled. The consume-event is automatically added to this button by this script.")]
    public Button consumeButton;
    [Tooltip("Please define a text field for different consume texts.")]
    public Text consumeText;

    [Tooltip("Please specify graphical elements like text, images etc. for collor application from the itme.")]
    public Graphic[] iconGraphics;
    [Tooltip("Please specify graphical elements like text, images etc. for collor application from the itme.")]
    public Graphic[] primaryGraphics;
    [Tooltip("Please specify graphical elements like text, images etc. for collor application from the itme.")]
    public Graphic[] secondaryGraphics;

    //Set an item an its amount form an item-amount class (contains more information about the item, e.g. amount)
    public void SetItem(Inventory.C_ItemAmount ia)
    {
        itemAmount = ia;

        if (ia != null && ia.item != null)
        {
            if (itemImage != null)
            {
                itemImage.sprite = itemAmount.item.image;
            }
            if (itemText != null)
            {
                itemText.text = TranslationManager.translateIfAvail(itemAmount.item.itemName);
            }
            if (amountText != null)
            {
                amountText.text = itemAmount.amount.ToString();
            }
            if(descriptionText != null)
            {
                descriptionText.text = TranslationManager.translateIfAvail(itemAmount.item.description);
            }

            if(consumeText != null)
            {
                consumeText.text = TranslationManager.translateIfAvail(itemAmount.item.consumeText);
            }

            if(consumeButton != null)
            {
                switch (itemAmount.item.consumeType)
                {
                    case InventoryItem.E_ItemConsumeType.consumable:
                        //consumeButton.gameObject.SetActive(true);

                        //enable consume button only, if conditions for consume are met
                        if (itemAmount.item.consumeConditions.AreConditionsMet() == true)
                        {
                            consumeButton.interactable = true;
                        }
                        else
                        {
                            consumeButton.interactable = false;
                        }
                        //consumeButton.onClick.AddListener(()=>consumeItem()); //this is buggy and can lead to many calls
                        break;
                    case InventoryItem.E_ItemConsumeType.nonConsumable:
                        consumeButton.interactable = false;
                        break;
                    default:
                        Debug.LogWarning("Inventory consume type '" + itemAmount.item.consumeType.ToString() + "' is unknown.");
                        break;
                }
            }

            if(iconGraphics != null && iconGraphics.Length > 0)
            {
                foreach(Graphic g in iconGraphics)
                {
                    g.color = ia.item.itemColors.iconColor;
                }
            }
            if (primaryGraphics != null && primaryGraphics.Length > 0)
            {
                foreach (Graphic g in primaryGraphics)
                {
                    g.color = ia.item.itemColors.primaryColor;
                }
            }
            if (secondaryGraphics != null && secondaryGraphics.Length > 0)
            {
                foreach (Graphic g in secondaryGraphics)
                {
                    g.color = ia.item.itemColors.secondaryColor;
                }
            }
        }
    }

    public void ActualizeAfterConsume()
    {
        bool consumeEnabled = true;
        if (itemAmount != null)
        {
            if (amountText != null)
            {
                amountText.text = itemAmount.amount.ToString();
            }

            if (itemAmount.amount <= 0)
            {
                consumeEnabled = false;
            }

            if(itemAmount.item.consumeConditions.AreConditionsMet() == false)
            {
                consumeEnabled = false;
            }

        }
        else
        {
            consumeEnabled = false;
        }

        if (consumeButton != null)
        {
            consumeButton.interactable = consumeEnabled;
        }
    }

    //Delegate the consume of an item to the main item manager (Inventory.cs).
    public void ConsumeItem()
    {
        if (itemAmount.item != null)
        {
            //1. Do all the consume results defined at the item
            itemAmount.item.consumeEffect.ExecuteEffect();

            //2. Inform the inventory to do something on consume.
            Inventory.instance.ConsumeItem(itemAmount.item);

            //3. Actualization!
            ActualizeAfterConsume();
        }
    }

    /// <summary>
    /// Select the item of this element as selected.
    /// </summary>
    public void SelectItem()
    {
        Inventory.instance.SelectItem(itemAmount);
    }

}
