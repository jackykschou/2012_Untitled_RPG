using UnityEngine;
using System.Collections;

public class SkillScroll : MonoBehaviour {

	public int skill_id;
	public GameObject collect_effect;
	private bool collected = false;
	
	float rotationSpeed = 30.0f;
	
	public void Collected()
	{
		GameObject effect = Instantiate(collect_effect, transform.position, Quaternion.identity) as GameObject;
		GameObject.Find("Menues").SendMessage("UnlockSkill", skill_id); //Unlock skill
		Destroy(effect, 1.5f);
		Destroy(gameObject);
	}
	
	void OnTriggerStay(Collider coll)
	{
		if(!collected && coll.tag == "Collector" && Input.GetKeyDown(KeyCode.F))
		{
			collected = true;
			Collected();	
		}
	}
	
	void Update()
	{
		transform.Rotate(new Vector3(0, rotationSpeed * Time.deltaTime, 0));
	}
}
