using UnityEngine;
using System.Collections;
[RequireComponent(typeof (Collider))]

//control buttet that shoot straight towards a givne position
public class SingleTargetBullet : MonoBehaviour {

	EffectApplier applier;
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
		GameObject exp = null;
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
			applier.ApplyEffectEnemy(enemy_sm);
			exp = Instantiate(explosion, transform.position, transform.rotation) as GameObject;
			Destroy(exp, 5.0f);
			Destroy(gameObject);
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
