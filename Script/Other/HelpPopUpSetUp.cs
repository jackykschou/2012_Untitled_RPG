using UnityEngine;
using System.Collections;

public class HelpPopUpSetUp : MonoBehaviour {

	public GameObject pop_up;
	
	void OnLevelWasLoaded (int level) 
	{
		CameraManager.help_menu = pop_up;
	}
	
	void Start()
	{
		CameraManager.help_menu = pop_up;
	}
}
