
using UnityEngine;
using System.Collections;
using System.Collections.Generic; //HashSet
[RequireComponent(typeof (Collider))]

//control buttet that shoot straight towards a givem position, damaging all the taget it passes through
public class MultiTargetBullet : MonoBehaviour {

	EffectApplier applier;
	HashSet<StatusManager> enemies = new HashSet<StatusManager>();
	public GameObject explosion;
	private bool start_detection;
	public float shoot_speed;
	
	public void StartShoot(EffectApplier s, Vector3 pos, float speed, GameObject exp, float time)
	{
		explosion = exp;
		Destroy(gameObject, time);
		shoot_speed = speed;
		transform.LookAt(new Vector3(pos.x, pos.y, pos.z));
		start_detection = true;
		applier = s;
	}
	
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
		if(start_detection && right_tag)
		{	
			Debug.Log(enemy);
			StatusManager enemy_sm = enemy.GetComponent<StatusManager>();
			if(!enemies.Contains(enemy_sm))
			{	
				applier.ApplyEffectEnemy(enemy_sm);
				enemies.Add(enemy_sm);
				GameObject exp = Instantiate(explosion, transform.position, transform.rotation) as GameObject;
				Destroy(exp, 5.0f);
			}
		}
	}
	
	void Update()
	{
		if(start_detection)
		{
			 transform.Translate(new Vector3(0f, 0f, shoot_speed * Time.deltaTime));
		}
	}
}

