using UnityEngine;
using System.Collections;

[AddComponentMenu("Camera-Control/Mouse Look")]


public class MouseLookX : MonoBehaviour {

	public float sensitivityX = 15F;
	public float minimumX = -360F;
	public float maximumX = 360F;
	public Camera default_cam;
	
	public GameObject character;

	void Update ()
	{
		transform.Rotate(0, Input.GetAxis("Mouse X") * sensitivityX, 0);
		Vector3 cam_vector = default_cam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, Camera.main.nearClipPlane + 20f));
		character.transform.LookAt(new Vector3(cam_vector.x, character.transform.position.y, cam_vector.z));
	}
	
	void Start ()
	{
		// Make the rigid body not change rotation
		if (rigidbody)
			rigidbody.freezeRotation = true;
	}
}