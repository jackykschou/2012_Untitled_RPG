using UnityEngine;
using System.Collections;

public class MenuesSetup : MonoBehaviour {

	public Camera cam;
	public UICamera cam_ui_root;
	public GameObject skill;
	public GameObject mission;
	public GameObject item;
	
	void OnLevelWasLoaded (int level) 
	{
		CameraManager.menu_cam = cam;
		CameraManager.menu_root = cam_ui_root;
		CameraManager.skill = skill;
		CameraManager.mission = mission;
		CameraManager.item = item;
	}
	
	void Start()
	{
		CameraManager.menu_cam = cam;
		CameraManager.menu_root = cam_ui_root;
		CameraManager.skill = skill;
		CameraManager.mission = mission;
		iTween.MoveTo(mission, new Hashtable(){ {"y", 1000f}, {"timer", 0.2f}, {"isLocal", true} });
		iTween.MoveTo(skill, new Hashtable(){ {"y", 1000f}, {"timer", 0.2f}, {"isLocal", true} });
		iTween.MoveTo(item, new Hashtable(){ {"y", 1000f}, {"timer", 0.2f}, {"isLocal", true} });
	}
}
