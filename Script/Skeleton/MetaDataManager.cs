using UnityEngine;
using System.Collections;
using System.Collections.Generic; //HastSet
using System;

public class MetaDataManager : MonoBehaviour {
	
	public static GameObject manager;
	
	public static int level;
	
	public static HashSet<int> skills_learnt;
	public static int[] skill_slots;
	public static int[] items_in_bag;
	public static int[] items_in_slots;
	public static int[] items_in_storage;
	
	public static int bag_size = 30;
	public static int storage_size = 20;
	public static int skill_num;
	
	public static void AddLearntSkill(int id)
	{
		skills_learnt.Add(id);	
	}
	
	public static void AddSkillToSlot(int index_in_slot, int index_in_pool)
	{
		items_in_slots[index_in_slot] = index_in_pool;
	}
	
	public static void RemoveSkillFromSlot(int index_in_slot)
	{
		items_in_slots[index_in_slot] = -1;
	}
	
	public static void AddItemToBag(int index_in_bag, int index_in_pool)
	{
		items_in_bag[index_in_bag] = index_in_pool;
	}
	
	public static void RemoveItemFromBag(int index)
	{
		items_in_bag[index] = -1;
	}
	
	public static void AddItemToSlot(int index_in_slot, int index_in_pool)
	{
		items_in_slots[index_in_slot] = index_in_pool;
	}
	
	public static void RemoveItemFromSlot(int index)
	{
		items_in_slots[index] = -1;
	}
}
