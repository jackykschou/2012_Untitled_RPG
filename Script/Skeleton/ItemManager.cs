using UnityEngine;
using System.Collections;

public class ItemManager : MonoBehaviour {

	public static PlayerStatusManager status_manager; //access the status of the character
	public static Item[] items = new Item[6] ;
	
	//assign a new item to the manager (put into the list) return whether the change is successful
	public static bool ChangeItem(Item item, int index)
	{
		if(item == null)
		{
			if(items[index])
			{
				items[index].Remove();
				items[index] = null;
				StatusGUIManager.EmptyItemIcon(index);
				ItemSlotManager.EmptyItem(index);
			}
			
			return true;
		}
		
		else
		{
			if(item.level_req > status_manager.level)
			{
				return false;	
			}
			
			if(items[index] != null)
			{
				items[index].Remove();
				MetaDataManager.RemoveItemFromSlot(index);
			}
			
			MetaDataManager.AddItemToSlot(index, item.id_in_pool);
			StatusGUIManager.ChangeItemIcon(index, item.icon);
			ItemSlotManager.AssignItem(index, item);
			item.SetUp(status_manager);
			items[index] = item;
			return true;
		}
	}
	
	public static void InputActiviateItem(int index)
	{	
		if(items[index] != null && items[index].usable)
		{
			if(items[index].ready)
			{
				items[index].Activate();
				if(items[index].consumable)
				{
					ChangeItem(null, index);
				}
			}
			else
			{
				Messenger.DisplaySmallMessage("Item On Cooldown");		
			}
		}
	}
}
