using UnityEngine;
using System.Collections;

public class Mission : MonoBehaviour {
	
	public string name;
	public string description;
	public int level_req;
	public string mission_sence_name;
	public AudioClip deny_sound;
	
	public GameObject enter_effect;
	public AudioClip enter_sound;
	
	public void ToLevel()
	{
		if(LevelManager.player_status.level >= level_req)
		{
			Destroy(Instantiate(enter_effect, LevelManager.manager.transform.position, Quaternion.identity) as GameObject, 5.0f);
			AudioManager.PlaySound(enter_sound, transform.position);
			StartCoroutine(Enter());
		}
		else
		{
			AudioManager.PlaySound(deny_sound, transform.position);	
		}
	}
	
	public string GetDescription()
	{
		return "[" + NGUITools.EncodeColor(Color.white) + "]" + "Level Require: " + level_req + "\n"
			+ description + "[-]";
	}
	
	IEnumerator Enter()
	{
		yield return new WaitForSeconds(1.5f);
		LevelManager.ChangeLevel(mission_sence_name);
	}
}
