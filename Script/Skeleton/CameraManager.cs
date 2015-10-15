using UnityEngine;
using System.Collections;

public class CameraManager : MonoBehaviour {
	
	public static Camera default_cam;
	public static UICamera default_root;
	public static Camera spell_cast_cam;
	public static Camera menu_cam;
	public static UICamera menu_root;
	public static GameObject skill;
	public static GameObject mission;
	public static GameObject item;
	
	public static GameObject help_menu;
	public static GameObject status_menu;
	
	public static bool menu_selecting = false;
	
	public static void SwitchToSpellCast()
	{
		default_cam.enabled = false;
		spell_cast_cam.enabled = true;
	}
	
	public static void SwitchToSpellMenu()
	{
		menu_selecting = true;
		default_cam.enabled = false;
		menu_cam.enabled = true;
		menu_root.enabled = true;
		iTween.MoveTo(skill, new Hashtable(){ {"y", 0f}, {"timer", 0.2f}, {"isLocal", true} });
		iTween.MoveTo(mission, new Hashtable(){ {"y", 1000f}, {"timer", 0.2f}, {"isLocal", true} });
		iTween.MoveTo(item, new Hashtable(){ {"y", 1000f}, {"timer", 0.2f}, {"isLocal", true} });
	}
	
	public static void SwitchToStatusMenu()
	{
		menu_selecting = true;
		status_menu.SetActive(true);
	}
	
	public static void SwitchToHelpMenu()
	{
		menu_selecting = true;
		help_menu.SetActive(true);
	}
	
	public static void SwitchToMissionMenu()
	{
		menu_selecting = true;
		default_cam.enabled = false;
		menu_cam.enabled = true;
		menu_root.enabled = true;
		iTween.MoveTo(mission, new Hashtable(){ {"y", 0f}, {"timer", 0.2f}, {"isLocal", true} });
		iTween.MoveTo(skill, new Hashtable(){ {"y", 1000f}, {"timer", 0.2f}, {"isLocal", true} });
		iTween.MoveTo(item, new Hashtable(){ {"y", 1000f}, {"timer", 0.2f}, {"isLocal", true} });
	}
	
	public static void SwitchToItemMenu()
	{
		menu_selecting = true;
		default_cam.enabled = false;
		menu_cam.enabled = true;
		menu_root.enabled = true;
		iTween.MoveTo(mission, new Hashtable(){ {"y", 1000f}, {"timer", 0.2f}, {"isLocal", true} });
		iTween.MoveTo(skill, new Hashtable(){ {"y", 1000f}, {"timer", 0.2f}, {"isLocal", true} });
		iTween.MoveTo(item, new Hashtable(){ {"y", 0f}, {"timer", 0.2f}, {"isLocal", true} });
	}
	
	public static void SwitchToDefault()
	{
		menu_selecting = false;
		default_cam.enabled = true;
		status_menu.SetActive(false);
		help_menu.SetActive(false);
		
		iTween.MoveTo(mission, new Hashtable(){ {"y", 1000f}, {"timer", 0.2f}, {"isLocal", true} });
		iTween.MoveTo(skill, new Hashtable(){ {"y", 1000f}, {"timer", 0.2f}, {"isLocal", true} });
		iTween.MoveTo(item, new Hashtable(){ {"y", 1000f}, {"timer", 0.2f}, {"isLocal", true} });
		menu_cam.enabled = false;
		menu_root.enabled = false;
		
	}

}
