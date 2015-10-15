using UnityEngine;
using System.Collections;

public class StatusPopUpSetUp : MonoBehaviour {
	
	public GameObject pop_up;
	
	void OnLevelWasLoaded (int level) 
	{
		CameraManager.status_menu = pop_up;
	}
	
	void Start()
	{
		CameraManager.status_menu = pop_up;
	}
	
}
