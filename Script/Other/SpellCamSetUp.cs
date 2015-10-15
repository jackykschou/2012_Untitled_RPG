using UnityEngine;
using System.Collections;

public class SpellCamSetUp : MonoBehaviour {
	
	public Camera spell_cam;
	
	void OnLevelWasLoaded (int level) 
	{
		CameraManager.spell_cast_cam = spell_cam;
	}
	
	void Start()
	{
		CameraManager.spell_cast_cam = spell_cam;
	}
}
