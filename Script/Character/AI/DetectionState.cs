using UnityEngine;
using System.Collections;
using System;

public class DetectionState : AIStatus {
	
	
	protected override sealed IEnumerator Execute()
	{
		GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
		GameObject[] pets = GameObject.FindGameObjectsWithTag("Pet");
		GameObject player = GameObject.FindGameObjectWithTag("Player");
		
		//get target
		if(ai.status_manager.target == null || !CheckDistance(gameObject, ai.status_manager.target, ai.detection_range) || (ai.status_manager.target.GetComponent("StatusManager") as StatusManager).is_dead)
		{
			ai.status_manager.target = null;

			switch(ai.ai_type)
			{
			case AI.Type.pet:
				if(enemies.GetLength(0) != 0)
				{
					float min_distance = Vector3.Distance(enemies[0].transform.position, transform.position);
					foreach(GameObject t in enemies)
					{
						if(CheckDistance(gameObject, t, ai.detection_range) && (Vector3.Distance(t.transform.position, transform.position) <= min_distance) && !(t.GetComponent("StatusManager") as StatusManager).is_dead)
						{
							min_distance = Vector3.Distance(t.transform.position, transform.position);
							ai.status_manager.target = t;
						}
					}
				}
				if(ai.status_manager.target == null && !(player.GetComponent("StatusManager") as StatusManager).is_dead)
				{
					ai.status_manager.target = player;
				}
				break;
				
			case AI.Type.normal:
				if(pets.GetLength(0) != 0)
				{
					float min_distance = Vector3.Distance(pets[0].transform.position, transform.position);
					foreach(GameObject t in pets)
					{
						if(CheckDistance(gameObject, t, ai.detection_range) && (Vector3.Distance(t.transform.position, transform.position) <= min_distance) && !(t.GetComponent("StatusManager") as StatusManager).is_dead)
						{
							min_distance = Vector3.Distance(t.transform.position, transform.position);
							ai.status_manager.target = t;
						}
					}
				}
				if(!((player.GetComponent("StatusManager") as StatusManager).is_dead) && (ai.status_manager.target == null || (Vector3.Distance(player.transform.position, transform.position) < Vector3.Distance(ai.status_manager.target.transform.position, transform.position))) && CheckDistance(player, gameObject, ai.detection_range))
				{
					ai.status_manager.target = player;
				}
				break;
				
			case AI.Type.challenger:
				if(CheckDistance(player, gameObject, ai.detection_range) && !((player.GetComponent("StatusManager") as StatusManager).is_dead))
				{
					ai.status_manager.target = player;
				}
				break;
				
			case AI.Type.coward:
				if(pets.GetLength(0) != 0)
				{
					float min_distance = Vector3.Distance(pets[0].transform.position, transform.position);
					foreach(GameObject t in pets)
					{
						if(CheckDistance(gameObject, t, ai.detection_range) && (Vector3.Distance(t.transform.position, transform.position) <= min_distance) && !(t.GetComponent("StatusManager") as StatusManager).is_dead)
						{
							min_distance = Vector3.Distance(t.transform.position, transform.position);
							ai.status_manager.target = t;
						}
					}
				}
				if(ai.status_manager.target == null && CheckDistance(player, gameObject, ai.detection_range) && !((player.GetComponent("StatusManager") as StatusManager).is_dead))
				{
					ai.status_manager.target = player;
				}
				break;
			}
		}
		else if((ai.ai_type == AI.Type.pet) && (ai.status_manager.target.tag == "Player") && (enemies.GetLength(0) != 0)) //try to find new enemy as a pet
		{
			float min_distance = Vector3.Distance(enemies[0].transform.position, transform.position);
			foreach(GameObject t in enemies)
			{
				if(CheckDistance(gameObject, t, ai.detection_range) && (Vector3.Distance(t.transform.position, transform.position) <= min_distance) && !(t.GetComponent("StatusManager") as StatusManager).is_dead)
				{
					min_distance = Vector3.Distance(t.transform.position, transform.position);
					ai.status_manager.target = t;
				}
			}
		}
		
		//follow the player if it is a pet and there is no enemy nearby
		yield return StartCoroutine(Follow());
		yield return StartCoroutine(ExecuteSkills());
		yield return StartCoroutine(LookAt());
	}
	
	protected override sealed bool CheckPreviousState()
	{
		return false;	
	}
	
	protected override sealed bool CheckNextState()
	{
		if(ai.status_manager.target != null && !(ai.ai_type == AI.Type.pet && ai.status_manager.target.tag == "Player"))
		{
			return true;
		}
		return false;
	}
	
	protected override sealed void ToPreviousState(){}
	
	protected override sealed void ToNextState()
	{
		AudioManager.PlaySound(ai.detect_sound, transform.position);
		StartCoroutine(ai.chasing.StartStatus(ai));
	}
	
}