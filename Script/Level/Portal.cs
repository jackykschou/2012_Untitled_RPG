using UnityEngine;
using System.Collections;

public class Portal : MonoBehaviour {
	
	public string to_level_name;
	public GameObject enter_effect;
	public AudioClip enter_sound;
	
	public bool entered = false;
	
	void OnTriggerStay(Collider coll)
	{
		if(!entered && coll.tag == "Player" && Input.GetKeyDown(KeyCode.E))
		{
			entered = true;
			StartCoroutine(Enter());
		}
	}
	
	IEnumerator Enter()
	{
		yield return new WaitForSeconds(1.5f);
		LevelManager.ChangeLevel(to_level_name);
	}
}
