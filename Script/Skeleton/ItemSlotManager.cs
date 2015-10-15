using UnityEngine;
using System.Collections;

public class ItemSlotManager : MonoBehaviour {

	public ItemSlot[] slots = new ItemSlot[6];
	
	private static GameObject manager;
	
	void Start()
	{
		manager = GameObject.Find("Menues");
	}
	
	public static void AssignItem(int index, Item item)
	{
		item_to_assign = item;
		manager.SendMessage("AssignItemHelper", index);
	}
	
	private static Item item_to_assign;
	
	public void AssignItemHelper(int index)
	{
		slots[index].icon.enabled = true;
		slots[index].current_item = item_to_assign;
		slots[index].icon.mainTexture = item_to_assign.icon;
		MetaDataManager.AddItemToSlot(index, item_to_assign.id_in_pool);
	}
	
	public static void EmptyItem(int index)
	{
		manager.SendMessage("EmptyItemHelper", index);
	}
	
	public void EmptyItemHelper(int index)
	{
		slots[index].current_item = null;
		slots[index].icon.mainTexture = null;
		slots[index].icon.enabled = false;
		MetaDataManager.RemoveItemFromSlot(index);
	}
	
}
