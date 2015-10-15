using UnityEngine;
using System.Collections;

public class DefaultCamSetUp : MonoBehaviour {
	
	public Camera cam;
	public UICamera cam_ui_root;
	
	void OnLevelWasLoaded (int level) 
	{
		CameraManager.default_cam = cam;
		CameraManager.default_root = cam_ui_root;
	}
	
	void Start()
	{
		CameraManager.default_cam = cam;
		CameraManager.default_root = cam_ui_root;
	}
	
}
