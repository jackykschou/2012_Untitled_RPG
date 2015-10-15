using UnityEngine;
using System.Collections;

/**
 * control the operation and switching of mouse selection in spell cast and menu
 **/

public class MenuSelector : MonoBehaviour {
	
	public GameObject skill_select_icon;
	public GameObject skill_select_range;
	public Texture menu_select_icon;
	public AudioClip menu_click_sound;
	
	public bool skill_selecting = false;
	public Vector3 select_pos; //keep track of the position of the mouse in a selection
	public bool selected = false;
	
	public bool cancel_selection;
	
	public IEnumerator SkillSelection(float range_unit)
	{	
		select_pos = Vector3.zero;
		AudioManager.PlaySound(menu_click_sound, transform.position);
		cancel_selection = false;
		selected = false;
		skill_selecting = true;
		GameObject icon = GameObject.Instantiate(skill_select_icon, Input.mousePosition, Quaternion.identity) as GameObject; //show the select icon
		GameObject range = GameObject.Instantiate(skill_select_range, LevelManager.player_status.bottom_pos.transform.position, Quaternion.identity) as GameObject;
		range.transform.localScale = new Vector3(range_unit, 1f, range_unit);
		Transform edge = range.transform.FindChild("edge");
		float max_cast_distance = Vector3.Distance(LevelManager.player_status.bottom_pos.transform.position, new Vector3(edge.position.x, LevelManager.player_status.bottom_pos.transform.position.y, edge.position.z));
		CameraManager.SwitchToSpellCast(); //switch camera
		
		RaycastHit hit;
		int layer = (1 << 8); //raycast ignore all layers except ground
		while(!selected)
		{
			//keep updating the position of the select icon
			//only hit the ground layer
			if(Physics.Raycast(Camera.main.ScreenPointToRay (Input.mousePosition), out hit, Mathf.Infinity, layer))
			{
					icon.transform.position = hit.point + new Vector3(0f, 0.1f, 0f);
					select_pos = icon.transform.position;
			}
			yield return new WaitForSeconds(Time.deltaTime);	
		}
		if(cancel_selection)
		{
			select_pos = Vector3.zero;
		}
		else if(Vector3.Distance(LevelManager.player_status.bottom_pos.transform.position, new Vector3(select_pos.x, LevelManager.player_status.bottom_pos.transform.position.y, select_pos.z)) > max_cast_distance)
		{
			Messenger.DisplaySmallMessage("Out Of Range");
			select_pos = Vector3.zero;
		}
		Destroy(icon);
		Destroy(range);
		CameraManager.SwitchToDefault(); //switch camera
		skill_selecting = false;
	}
	
	void OnGUI()
	{
		if(CameraManager.menu_selecting)
		{
	    	GUI.DrawTexture(new Rect(Input.mousePosition.x, Screen.height - Input.mousePosition.y, 32, 32), menu_select_icon);
		}
	}
	
	public void TriggerSkillMenu()
	{
		AudioManager.PlaySound(menu_click_sound, transform.position);
		if(!CameraManager.menu_selecting)
		{
			CameraManager.SwitchToSpellMenu(); //switch camera
		}
		else
		{
			CameraManager.SwitchToDefault(); //switch camera
		}
	}
	
	public void TriggerStatusMenu()
	{
		AudioManager.PlaySound(menu_click_sound, transform.position);
		if(!CameraManager.menu_selecting)
		{
			CameraManager.SwitchToStatusMenu(); //switch camera
		}
		else
		{
			CameraManager.SwitchToDefault(); //switch camera
		}
	}
	
	public void TriggerItemMenu()
	{
		AudioManager.PlaySound(menu_click_sound, transform.position);
		if(!CameraManager.menu_selecting)
		{
			CameraManager.SwitchToItemMenu(); //switch camera
		}
		else
		{
			CameraManager.SwitchToDefault(); //switch camera
		}
	}
	
	public void TriggerHelpMenu()
	{
		AudioManager.PlaySound(menu_click_sound, transform.position);
		if(!CameraManager.menu_selecting)
		{
			CameraManager.SwitchToHelpMenu(); //switch camera
		}
		else
		{
			CameraManager.SwitchToDefault(); //switch camera
		}
	}
	
	
	void Start()
	{
		Screen.showCursor = false;
	}
	
}
