using UnityEngine;
using System.Collections;
using System.Collections.Generic; //HashSet

public class LastingColliderEnemyCollector : MonoBehaviour {

	HashSet<StatusManager> enemies = new HashSet<StatusManager>();
	private bool start_collection;
	EffectApplier applier;
	
	public void CollectEnemies(float collection_time, EffectApplier s)
	{
		applier = s;
		start_collection = true;
		StartCoroutine(ApplyEffectPerSec());
		Destroy(this, collection_time);
	}
	
	//collect status of enemies when they enter the collider of the effect
	void OnTriggerEnter(Collider enemy)
	{
		bool right_tag = false;
		if(applier.ownership == EffectApplier.Ownership.enemyAi)
		{
			right_tag = ((enemy.tag == "Player") || (enemy.tag == "Pet"));
		}
		else
		{
			right_tag = (enemy.tag == "Enemy");
		}
		if(start_collection && right_tag)
		{
			StatusManager enemy_sm = enemy.GetComponent<StatusManager>();
			if(!enemies.Contains(enemy_sm))
			{
				enemies.Add(enemy_sm);
			}
				
		}
	}
	
	//get rid of the status of enemies when they leave the collider of the effect
	void OnTriggerExit(Collider enemy)
	{
		bool right_tag = false;
		if(applier.ownership == EffectApplier.Ownership.enemyAi)
		{
			right_tag = ((enemy.tag == "Player") || (enemy.tag == "Pet"));
		}
		else
		{
			right_tag = (enemy.tag == "Enemy");
		}
		if(right_tag)
		{
			StatusManager enemy_sm = enemy.GetComponent<StatusManager>();
			if(enemies.Contains(enemy_sm))
			{
				enemies.Remove(enemy_sm);
			}
		}
	}
	
	IEnumerator ApplyEffectPerSec()
	{
		while(start_collection)
		{
			foreach(StatusManager sm in enemies)
			{
				applier.ApplyEffectEnemy(sm);
			}
			yield return new WaitForSeconds(1f);
		}
	}
}
