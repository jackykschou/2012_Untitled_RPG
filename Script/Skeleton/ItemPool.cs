using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ItemPool : MonoBehaviour {

	public DragDropItem_new[] bag = new DragDropItem_new[MetaDataManager.bag_size];
	public Item[] item_pool;
	public AudioClip pick_sound;
	public AudioClip deny_pick_sound;
	public AudioClip drop_sound;
	
	public GameObject drop_item;
	
	public void AssignItem(int id)
	{
		for(int i = 0; i < MetaDataManager.bag_size; ++i)
		{
			if(bag[i] != null && bag[i].item == null)
			{
				MetaDataManager.AddItemToBag(i, id);
				bag[i].AssignItem(item_pool[id]);	
				AudioManager.PlaySound(pick_sound, transform.position);
				return;
			}
		}
		Messenger.DisplaySmallMessage("Your bag is full");
		AudioManager.PlaySound(deny_pick_sound, transform.position);
	}
	
}
