using UnityEngine;
using System.Collections;

public class SelectionPortal : MonoBehaviour {
	
	public AudioClip menu_click_sound;
	
	void OnTriggerStay(Collider coll)
	{
		if(coll.tag == "Player" && Input.GetKeyDown(KeyCode.E))
		{
			AudioManager.PlaySound(menu_click_sound, transform.position);
			if(!CameraManager.menu_selecting && !LevelManager.player_status.menu_selector.skill_selecting)
			{
				CameraManager.SwitchToMissionMenu(); //switch camera
			}
			else
			{
				CameraManager.SwitchToDefault(); //switch camera
			}
		}
	}
}