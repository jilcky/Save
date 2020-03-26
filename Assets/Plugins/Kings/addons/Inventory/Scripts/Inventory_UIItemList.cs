using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The item list is responsible for spawning a list of items into the scene.
/// This scipt can also be used for showing sub-inventories by filtering by categories.
/// </summary>

public class Inventory_UIItemList : MonoBehaviour
{
    //Link to the main script of the inventory.
    private Inventory inventory;

    [Tooltip("Provide a template for the display of an item within the UI.")]
    public GameObject UITemplate;

    //filter options, selectable from the inspector
    [System.Serializable]
    public enum E_ItemDisplayFilter
    {
        showAll,
        showTypes,
        hideTypes
    }

    [Tooltip("Select how the items for this element shall be filtered.")]
    public E_ItemDisplayFilter filterType;
    [Tooltip("Select for which categories the filter shall apply. This list has no effect if 'Show All' is selected.")]
    public List<ItemDefinitions.E_ItemCategory> categoryFilter;

    void getRequiredInstances()
    {
        inventory = Inventory.instance;
        inventory.OnInventoryChangeAction += OnInventoryChanged;   //Callback to get informed if something in the inventory changes.
    }


    private void Start()
    {
        getRequiredInstances();
    }

    //List of items to display in this (sub-) inventory.
    [HideInInspector] public List<GameObject> listElements;

    void OnInventoryChanged()
    {
        ActualizeUIList();
    }

    private void OnEnable()
    {
        StartCoroutine(delayedInit());
    }

    IEnumerator delayedInit()
    {
        if (inventory != null)
        {
            ActualizeUIList();  //if instances are know actualize immediatly
           // Debug.Log("delayed act");
        }
        else
        {
            yield return null;  //wait one frame until linking to enshure execution order
            getRequiredInstances();
            ActualizeUIList();  
        }
    }

    private void OnDestroy()
    {
        if (inventory != null)
        {
            inventory.OnInventoryChangeAction -= OnInventoryChanged;
            inventory = null;
        }
    }

    //Filter method to determine if the item shall be shown in this list.
    private bool GetShowItem(InventoryItem item)
    {
        if(item.visible == false)
        {
            return false;
        }

        switch (filterType)
        {
            case E_ItemDisplayFilter.showAll:
                return true;
            case E_ItemDisplayFilter.showTypes:
                foreach(ItemDefinitions.E_ItemCategory category in categoryFilter)
                {
                    if(category == item.category)
                    {
                        return true;
                    }
                }
                return false;
            case E_ItemDisplayFilter.hideTypes:
                foreach (ItemDefinitions.E_ItemCategory category in categoryFilter)
                {
                    if (category == item.category)
                    {
                        return false;
                    }
                }
                return true;
            default:
                Debug.LogWarning("The inventory filter '" + filterType.ToString() + "' is not recognized. Inventory item '" + item.name + "' is displayed.");
                return true;
        }
    }

    //Actualization of the inventory for the UI
    public void ActualizeUIList()
    {
        //Lazy update step 1: destroy all current displays.
        foreach(GameObject go in listElements)
        {
            Destroy(go);
        }

        //Lazy update step 2: insert all new displays.
        foreach (Inventory.C_ItemAmount ia in inventory.inventory.items)
        {
            if (ia.loadedFromCatalog == true )
            {
                if (GetShowItem(ia.item) == true)
                {
                    GameObject go = (GameObject)Instantiate(UITemplate);
                    go.transform.SetParent(gameObject.transform,false);
                    Inventory_UIItem uiItem = go.GetComponent<Inventory_UIItem>();
                    if (uiItem == null)
                    {
                        Debug.LogWarning("The Prefab for inventory items should have a 'Inventory_UIItem' script. This script was not found. Skipping display.");
                    }
                    else
                    {
                        uiItem.SetItem(ia);
                        listElements.Add(go);
                    }
                }
            }
        }
    }
}
