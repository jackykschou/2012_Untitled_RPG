using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class StartFirstLevelSetUp : MonoBehaviour {

	// Use this for initialization
	void Start () 
	{
		SetUp();
	}
	
	void SetUp()
	{
		MetaDataManager.skills_learnt = new HashSet<int>();
		MetaDataManager.skill_slots = new int[6];
		MetaDataManager.items_in_bag = new int[MetaDataManager.bag_size];
		MetaDataManager.items_in_slots = new int[6];
		MetaDataManager.items_in_storage = new int[MetaDataManager.storage_size];
		
		for(int i = 0; i < 6; ++i)
		{
			MetaDataManager.skill_slots[i] = -1;	
		}
		for(int i = 0; i < 6; ++i)
		{
			MetaDataManager.items_in_slots[i] = -1;	
		}
		for(int i = 0; i < MetaDataManager.bag_size; ++i)
		{
			MetaDataManager.items_in_bag[i] = -1;	
		}
		for(int i = 0; i < MetaDataManager.storage_size; ++i)
		{
			MetaDataManager.items_in_storage[i] = -1;	
		}
		
		
		DontDestroyOnLoad(GameObject.Find("AudioManager"));
		DontDestroyOnLoad(GameObject.Find("GUITextMessenger"));
		DontDestroyOnLoad(GameObject.Find("MecanimEventManager"));
		DontDestroyOnLoad(GameObject.Find("MainCharacter"));
		Application.LoadLevel("base");
	}
	
}
