using UnityEngine;
using System.Collections;

public class BarPosition : MonoBehaviour {
	
	public EnergyBarRenderer render;
	public float height_offset;
	public float width_offset;
	public float previous_height_offsets;
	
	// Use this for initialization
	void Update () 
	{
		render.screenPosition.x = 0f;
		render.screenPosition.y = -Screen.height * previous_height_offsets;
		render.size.x = Screen.width * width_offset;
		render.size.y = Screen.height * height_offset;
	}
	
	
}
