#define KINGS_INVENTORY

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


/// <summary>
/// Main Script for the inventory.
/// 
/// Inventory is a small inventory system with lists of items (showing can be filtered).
/// The Inventory is stored in the player prefs.
/// The Inventory can be used outside of Kings by commenting #define KINGS_INVENTORY (//#define KINGS_INVENTORY) 
/// without the translation management.
/// 
/// Additional dependencies: ReadOnlyInspectorAttribute.cs is needed to show the elements correctly in the inspector.
/// You can also delete the '[ReadOnlyInspector]' strings, but this will be a little bit less cleaner in the inspector.
/// </summary>

#if KINGS_INVENTORY
public class Inventory : TranslatableContent
{
#else
public class Inventory : MonoBehaviour { 
#endif
    [System.Serializable] public class mEvent : UnityEvent { }

    [Tooltip("Enable/Disable extended debugging output.")]
    public bool verboseDebug = false;
    void mDebug(string txt)
    {
        if (verboseDebug == true)
        {
            Debug.Log(txt);
        }
    }

    #region item detail view
    //code part for the detail view of on item, including activation/deactivation options of the panel, button control etc.

    [System.Serializable]
    public class C_ItemDetail
    {
        [ReadOnlyInspector] public C_ItemAmount selectedElement;
        public bool ActivatePanelOnSelection = true;
        public bool DeactivatePanelOnConsume = true;
        [HideInInspector] public Inventory_UIItem uiItem;
        public GameObject detailViewPanel_Prefab;
        public GameObject detailViewPanel_Parent;
        [HideInInspector] public GameObject spawnedDetailViewPanel;
    }
    public C_ItemDetail itemDetailView;

    [System.Serializable]
    public class C_ItemUpdateInformation
    {
        [HideInInspector] public Inventory_UIItem uiItem;
        [ReadOnlyInspector] public List<C_ItemAmount> newItems = new List<C_ItemAmount>();
        public float panelActiveTime = 2f;
        public GameObject newItemPanel_Prefab;
        public GameObject newitempanel_Parent;
        [HideInInspector] public GameObject spawnedNewItemPanel = null;
        [HideInInspector] public bool lockedForShowing = false;
    }
    public C_ItemUpdateInformation itemUpdateView;
    public void SelectItem(C_ItemAmount ia)
    {
        itemDetailView.selectedElement = ia;
        if (itemDetailView.ActivatePanelOnSelection == true)
        {
            //destroy already active detail views
            if (itemDetailView.spawnedDetailViewPanel != null)
            {
                Destroy(itemDetailView.spawnedDetailViewPanel);
            }

            CreateNewDetailViewPanel(ia);

        }
        if (itemDetailView.uiItem != null)
        {
            itemDetailView.uiItem.SetItem(ia);
        }
    }
    private void TryToCloseItemDetailPanel(InventoryItem item)
    {
        if (itemDetailView.DeactivatePanelOnConsume == false)
        {
            return;
        }
        if (itemDetailView.selectedElement != null)
        {
            if (itemDetailView.spawnedDetailViewPanel != null)
            {
                Destroy(itemDetailView.spawnedDetailViewPanel);
            }
        }
    }
    private void CreateNewDetailViewPanel(C_ItemAmount itemAmount)
    {
        //Debug.Log("Create new update view for " + itemAmount.item.itemName);
        if (itemDetailView.detailViewPanel_Parent != null)
        {
            //select the default panel as prefab
            GameObject detailPanelPrefab = itemDetailView.detailViewPanel_Prefab;
            //if the item has an override panel, override the prefab with the specific one
            if (itemAmount != null)
            {
                if (itemAmount.item != null)
                {
                    if (itemAmount.item.detailItemOverridePanel != null)
                    {
                        detailPanelPrefab = itemAmount.item.detailItemOverridePanel;
                        //Debug.Log("selecting " + newPanelPrefab.name);
                    }
                }
            }

            if (detailPanelPrefab != null)
            {
                //Debug.Log("spawning " + detalPanelPrefab.name);
                itemDetailView.spawnedDetailViewPanel = (GameObject)Instantiate(detailPanelPrefab);
                itemDetailView.spawnedDetailViewPanel.transform.SetParent(itemDetailView.detailViewPanel_Parent.transform,false);
                itemDetailView.uiItem = itemDetailView.spawnedDetailViewPanel.GetComponent<Inventory_UIItem>();

                if (itemDetailView.uiItem == null)
                {
                    Debug.LogWarning("The 'Inventory_UIItem' script is missing at the spawned prefab '" + detailPanelPrefab.ToString() + "'. The panel can't be shown.");
                }
            }
        }
    }
    #endregion

    #region item update view
    private void CreateNewUpdateViewPanel(C_ItemAmount itemAmount)
    {
        //Debug.Log("Create new update view for " + itemAmount.item.itemName);
        if (itemUpdateView.newitempanel_Parent != null)
        {
            //select the default panel as prefab
            GameObject newPanelPrefab = itemUpdateView.newItemPanel_Prefab;
            //if the item has an override panel, override the prefab with the specific one
            if (itemAmount != null)
            {
                if (itemAmount.item != null)
                {
                    if (itemAmount.item.newItemOverridePanel != null)
                    {
                        newPanelPrefab = itemAmount.item.newItemOverridePanel;
                        //Debug.Log("selecting " + newPanelPrefab.name);
                    }
                }
            }

            if (newPanelPrefab != null)
            {
                // Debug.Log("spawning " + newPanelPrefab.name);
                itemUpdateView.spawnedNewItemPanel = (GameObject)Instantiate(newPanelPrefab);
                itemUpdateView.spawnedNewItemPanel.transform.SetParent(itemUpdateView.newitempanel_Parent.transform,false);
                itemUpdateView.uiItem = itemUpdateView.spawnedNewItemPanel.GetComponent<Inventory_UIItem>();

                if (itemUpdateView.uiItem == null)
                {
                    Debug.LogWarning("The 'Inventory_UIItem' script is missing at the spawned prefab '" + newPanelPrefab.ToString() + "'. The panel can't be shown.");
                }
            }
        }
    }
    //Showing of new items is put to an coroutine to enable showing one item after another.
    IEnumerator ShowNewItems()
    {
        // Copy the new items to a list and show this list. By this way adding new items while showing the list doesn't interfere with the showing itself;
        // Concurrency is handled by locking until another one is finished.

        while (itemUpdateView.lockedForShowing == true)
        {
            yield return null;
        }

        itemUpdateView.lockedForShowing = true;
        List<C_ItemAmount> elementsForShowing = new List<C_ItemAmount>(itemUpdateView.newItems);
        itemUpdateView.newItems.Clear();

        foreach (C_ItemAmount ia in elementsForShowing)
        {
            CreateNewUpdateViewPanel(ia);

            if (itemUpdateView.uiItem != null)
            {
                itemUpdateView.uiItem.SetItem(ia);
                yield return new WaitForSeconds(itemUpdateView.panelActiveTime);
                Destroy(itemUpdateView.spawnedNewItemPanel);
                yield return null;
            }
            else
            {
                Debug.LogWarning("The 'New Item Panel' for the inventory item '" + ia.item.itemName + "' could not be created.");
            }

        }
        yield return null;
        itemUpdateView.lockedForShowing = false;
    }
    #endregion

    public static Inventory instance;          //Hmm.. another singleton. This manager can only be used once per scene.
    string jsonKey = "Inventory";              //Save key for the Playerprefs.
    public System.Action OnInventoryChangeAction;   //Callback possibility for other scripts to get informed if something changes in the inventory.
    bool loaded = false;

    private void Awake()
    {
        instance = this;
        loadCatalog();
        load();
        StartCoroutine(delayedActualize());
    }

    private void Start()
    {
#if KINGS_INVENTORY
        TranslationManager.instance.registerTranslateableContentScript(this);
#endif
    }

    IEnumerator delayedActualize()
    {
        yield return null;
        inventoryChanged(false);
    }

    //definition of event types for the inventory and specific items
    [System.Serializable]
    public enum E_ItemEventType
    {
        consumed,       /* Event type if the item is consumed e.g. by pressing the consume button */
        increased,      /* Event type it the amount of the item increased */
        decreased,      /* Event type if the amount of the item decreased (not neccessary consumed) */
        itemsOverMax    /* Event type if the item amount would be increased over the maximal amount (the amount is still limited) */
    }

    [System.Serializable]
    public class C_ItemEvent
    {
        public E_ItemEventType eventType = E_ItemEventType.consumed;
        public InventoryItem item;
        public mEvent OnEvent;
    }
    //user definable list of item events
    public List<C_ItemEvent> ItemEvents = new List<C_ItemEvent>();

    [System.Serializable]
    public class C_CategoryEvent
    {
        public E_ItemEventType eventType = E_ItemEventType.consumed;
        public ItemDefinitions.E_ItemCategory category;
        public mEvent OnEvent;
    }
    //inspector definable list of category events
    public List<C_CategoryEvent> CategoryEvents = new List<C_CategoryEvent>();

    //Storage class for the items. 
    [System.Serializable]
    public class C_ItemAmount
    {
        [ReadOnlyInspector] public string itemKey = "";             //key (name) of the item
        [ReadOnlyInspector] public InventoryItem item;              //link to the actual item (saving by serialization of this entry is not usable but it is correctly loaded by the key and the catalog)
        [ReadOnlyInspector] public int amount = 0;                  //amount of available items
        [ReadOnlyInspector] public bool loadedFromCatalog = true;   //Memorize if the item key was found in the catalog. If not: Item was removed from the catalog and shall not be displayed anymore (but is still saved).
    }

    //catalog for all possible items (loaded from resources-folder)
    [ReadOnlyInspector] public List<InventoryItem> catalog = new List<InventoryItem>();

    //embed the item entry data to an class to allow correct serialization
    [System.Serializable]
    public class C_Inventory
    {
        public List<C_ItemAmount> items;
    }

    [ReadOnlyInspector] public C_Inventory inventory;

    //Return an item entry with amount, distinguished by its string key.
    public C_ItemAmount GetItemByKey(string key)
    {
        foreach (C_ItemAmount itemAmount in inventory.items)
        {
            if ((itemAmount.item != null) && (itemAmount.item.name == key))
            {
                return itemAmount;
            }
        }
        return null;
    }

    //Return an item from the resources catalog, distinguished by its string key.
    public InventoryItem GetItemFromCatalogByKey(string key)
    {
        foreach (InventoryItem item in catalog)
        {
            if (item != null && item.name == key)
            {
                return item;
            }
        }
        mDebug("The item with the key '" + key + "' could not be found within the item catalog.");
        return null;
    }

    //Remove an item from the inventory.
    private void RemoveItem(string key)
    {
        int indexToRemove = 0;
        bool found = false;
        for (int i = 0; i < inventory.items.Count; i++)
        {
            mDebug("Try to removoe '" + key + "' from iventory.");
            if (inventory.items[i].itemKey == key)
            {
                found = true;
                indexToRemove = i;
                break;
            }
        }
        if (found)
        {
            inventory.items.RemoveAt(indexToRemove);
        }
    }

    /// <summary>
    /// Delete all items from the inventory.
    /// This is absolute, the items can't be restored.
    /// </summary>
    public void DeleteInventory()
    {
        inventory = new C_Inventory();
        inventory.items = new List<C_ItemAmount>();
        inventoryChanged();
        save();
    }

    /// <summary>
    /// Add a specific amount of items to the inventory. Negative numbers remove items.
    /// </summary>
    /// <param name="item"></param>
    /// <param name="amount"></param>
    public void AddItemAmount(InventoryItem item, int amount)
    {
        mDebug("Add " + amount.ToString() + "x '" + item.name + "'.");
        C_ItemAmount ia = GetItemByKey(item.name);
        if (ia != null)
        {
            ia.amount += amount;

            if (ia.amount > ia.item.maxItems)
            {
                ia.amount = ia.item.maxItems;
                GenerateItemEvent(item, E_ItemEventType.itemsOverMax);
                GenerateCategoryEvent(item.category, E_ItemEventType.itemsOverMax);
            }


            //Item decrease event can only be fired if the item exists with an amount:
            if (amount < 0)
            {
                GenerateItemEvent(item, E_ItemEventType.decreased);
                GenerateCategoryEvent(item.category, E_ItemEventType.decreased);
            }
        }
        else
        {
            //create a new item
            ia = new C_ItemAmount();
            ia.item = item;
            ia.amount = amount;
            ia.itemKey = item.name;        //using the name of the scriptableObject as key
            inventory.items.Add(ia);
        }

        //Item increase event can be fired if added amount is bigger than 0
        if (amount > 0)
        {
            GenerateItemEvent(item, E_ItemEventType.increased);
            GenerateCategoryEvent(item.category, E_ItemEventType.increased);

            //show the new added item for the user
            C_ItemAmount newItemAmount = new C_ItemAmount();
            newItemAmount.item = ia.item;
            newItemAmount.amount = amount;
            itemUpdateView.newItems.Add(newItemAmount);
            StartCoroutine(ShowNewItems());
        }

        //remove the item, if the amount is 0
        if (ia.amount <= 0)
        {
            RemoveItem(item.name);
        }

        inventoryChanged();
        save();
    }

    /// <summary>
    /// Set a specific amount of items to the inventory. Negative numbers or 0 remove items.
    /// </summary>
    /// <param name="item"></param>
    /// <param name="amount"></param>
    public void SetItemAmount(InventoryItem item, int amount)
    {
        mDebug("Set " + amount.ToString() + "x '" + item.name + "'.");
        int storedItems = GetItemCount(item);

        //Reusing addItem, I dislike unneccesary redundancy:
        int diff = amount - storedItems;
        AddItemAmount(item, diff);
    }

    //Internal method to generete item and event-type specific events.
    private void GenerateItemEvent(InventoryItem item, E_ItemEventType requestedEventType)
    {
        int eventsFound = 0;
        foreach (C_ItemEvent ie in ItemEvents)
        {
            if (ie.item != null)
            {
                if (ie.item.name == item.name)
                {
                    if (ie.eventType == requestedEventType)
                    {
                        ie.OnEvent.Invoke();
                        eventsFound++;
                    }
                }
            }
            else
            {
                //the element is missing (in inspector)
            }
        }
        if (eventsFound != 1)
        {
            mDebug("There is a possibility for missing/duplicate events for the event '" + requestedEventType.ToString() + "' of item '" + item.name + "': There were " + eventsFound.ToString() + " events excuted.");
        }
    }

    //Internal method to generete category and event-type specific events.
    private void GenerateCategoryEvent(ItemDefinitions.E_ItemCategory category, E_ItemEventType requestedEventType)
    {
        int eventsFound = 0;
        foreach (C_CategoryEvent ce in CategoryEvents)
        {
            if (ce.category == category)
            {
                if (ce.eventType == requestedEventType)
                {
                    ce.OnEvent.Invoke();
                    eventsFound++;
                }
            }
        }
        if (eventsFound != 1)
        {
            mDebug("There is a possibility for missing/duplicate events for the event '" + requestedEventType.ToString() + "' of category '" + category.ToString() + "': There were " + eventsFound.ToString() + " events excuted.");
        }
    }

    /// <summary>
    /// Consume an item. This will also fire the according public events.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public void ConsumeItem(InventoryItem item)
    {
        mDebug("Conusme item '" + item.itemName + "'");
        AddItemAmount(item, -1);
        GenerateItemEvent(item, E_ItemEventType.consumed);
        GenerateCategoryEvent(item.category, E_ItemEventType.consumed);
        TryToCloseItemDetailPanel(item);
        AfterAnyItemConsume.Invoke();
    }

    /// <summary>
    /// Return the number of items.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public int GetItemCount(InventoryItem item)
    {
        return GetItemCount(item.name);
    }

    public int GetItemCount(string key)
    {
        C_ItemAmount i = GetItemByKey(key);
        if (i != null)
        {
            return i.amount;
        }
        return 0;
    }

    //Internal method. This is called from various places if something in the inventory changes.
    private void inventoryChanged(bool fireUserEvents = true)
    {
        //Execute all requested callbacks (e.g. from the UI script to actualize the list), if not null;
        if (OnInventoryChangeAction != null)
        {
            OnInventoryChangeAction.Invoke();
        }

        if (fireUserEvents == true)
        {
            //Execute User Events
            OnInventoryChange.Invoke();
        }
    }

    //load inventory from the playerprefs
    private void load()
    {
        string json = SecurePlayerPrefs.GetString(jsonKey);
        if (!string.IsNullOrEmpty(json))
        {
            JsonUtility.FromJsonOverwrite(json, inventory);
        }

        //Json loads some IDs to reference to item. Because of mismatching IDs to the actual IDs this resulted in missing/empty items.
        //To bypass this, the name of the scriptableObject is used as a key and the corresponding object is inserted from the catalog.

        foreach (C_ItemAmount ia in inventory.items)
        {
            InventoryItem item = GetItemFromCatalogByKey(ia.itemKey);
            if (item != null)
            {
                ia.item = item;
                ia.loadedFromCatalog = true;
            }
            else
            {
                ia.loadedFromCatalog = false;
            }
        }

        loaded = true;
    }

    //Load all possible items from the "Resource" folder of the project and add it to a catalog.
    private void loadCatalog()
    {
        catalog.Clear();

        Object[] objects = Resources.LoadAll("", typeof(InventoryItem));

        if (objects != null)
        {
            for (int i = 0; i < objects.Length; i++)
            {
                catalog.Add((InventoryItem)objects[i]);
            }
        }
    }

    //save the inventory to the playerprefs
    private void save()
    {
        if (loaded)
        {
            string json = JsonUtility.ToJson(inventory);
            SecurePlayerPrefs.SetString(jsonKey, json);
        }
        else
        {
            Debug.LogError("Inventor can not be altered bevore it is loaded. Your change is lost.");
        }
    }
    //General event something in the inventory changes (item added, set, removed)
    public mEvent AfterAnyItemConsume;
    //General event something in the inventory changes (item added, set, removed)
    public mEvent OnInventoryChange;

 //Return all possible translatable terms for the translation managment in Kings.
#if KINGS_INVENTORY
    public override List<string> getTranslatableTerms()
    {
        List<string> terms = new List<string>();
        terms.Clear();

        foreach (InventoryItem item in catalog)
        {
            terms.Add(item.itemName);
            terms.Add(item.description);
            terms.Add(item.consumeText);
        }

        return terms;
    }
#endif
}
