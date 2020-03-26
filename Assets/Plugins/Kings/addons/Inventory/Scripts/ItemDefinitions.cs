
/// <summary>
/// User definitions for inventory items of the LocalInventory System.
/// </summary>

public class ItemDefinitions 
{
 
    //Categories for an item. Can be user-extended to allow for more categories.
    //Multiplicity: Many items can be assigned to one category. 
    // ! Please be aware: this is an enumeration, inserting one element in the middle will change the assignement of the following elements.
    //It is safe to extend the list by adding elements at the end of the list.
    //It is safe to rename elements if they are not hard-coded. 
    public enum E_ItemCategory
    {
        none,       //default category: none
        consumable,
        non_consumable
    }

}
