using UnityEngine;
using System.Collections;
using System.Collections.Generic; //HashSet

public class OneTimeColliderEnemyCollector : MonoBehaviour {
	
	HashSet<StatusManager> enemies = new HashSet<StatusManager>();
	private bool start_collection;
	EffectApplier applier;
	
	GameObject effect;
	
	public void CollectEnemies(float collection_time, EffectApplier s, GameObject e = null)
	{
		applier = s;
		start_collection = true;
		effect = e;
		Destroy(this, collection_time);
	}
	
	//collect status of enemies while collection is on (while the attack is going on)
	void OnTriggerStay(Collider enemy)
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
				applier.ApplyEffectEnemy(enemy_sm);
				start_collection = false;
				if(effect != null)
				{
					Destroy(Instantiate(effect, transform.position, Quaternion.identity) , 3.0f);
				}
				Destroy(this, 3.0f);
			}
				
		}
	}
}

