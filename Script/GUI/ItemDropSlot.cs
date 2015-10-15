//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2012 Tasharen Entertainment (edited by Ka Seng Chou)
//----------------------------------------------

using UnityEngine;

/// <summary>
/// Simple example of an OnDrop event accepting a game object. In this case we check to see if there is a DragDropObject present,
/// and if so -- create its prefab on the surface, then destroy the object.
/// </summary>

[AddComponentMenu("NGUI/Examples/Drag and Drop Surface")]
public class ItemDropSlot : MonoBehaviour
{
	public bool rotatePlacedObject = false;
	public GameObject item_prefab;
	
	public AudioClip drop_sound;

	void OnDrop (GameObject go)
	{
		DragDropItem_new slot = go.GetComponent<DragDropItem_new>();
		if (slot != null && slot.item != null)
		{
			slot.AssignItem(null);
		}
		else
		{
			ItemSlot slot2 = go.GetComponent<ItemSlot>();
			if (slot2 != null && slot2.current_item != null)
			{
				ItemManager.ChangeItem(null, slot2.item_slot_id);
			}
		}
	}
	
	
}