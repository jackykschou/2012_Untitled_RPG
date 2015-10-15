using UnityEngine;
using System.Collections;

public class ItemBox : MonoBehaviour {

	public int item_id;
	public GameObject collect_effect;
	private bool collected = false;
	
	float rotationSpeed = 60.0f;
	
	public void Collected()
	{
		GameObject effect = Instantiate(collect_effect, transform.position, Quaternion.identity) as GameObject;
		GameObject.Find("Menues").SendMessage("AssignItem", item_id); //Unlock skill
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