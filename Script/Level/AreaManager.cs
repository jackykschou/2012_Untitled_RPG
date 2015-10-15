using UnityEngine;
using System.Collections;

public class AreaManager : MonoBehaviour {
	
	public AudioClip background_music;
	public AudioClip environment_sound;
	
	public Spawner[] spawners;
	
	public GameObject[] objects;
	
	void OnTriggerEnter(Collider coll)
	{
		if(coll.tag == "Player")
		{
			if(background_music != null)
			{
				AudioManager.PlayBackgroundMusic(background_music);
			}
			if(environment_sound != null)
			{
				AudioManager.PlayEnvironmentSound(environment_sound);
			}
			foreach(Spawner s in spawners)
			{
				s.Spawn();
			}
			foreach(GameObject g in objects)
			{
				g.SetActive(true);
			}
		}
	}
	
	void OnTriggerExit(Collider coll)
	{
		if(coll.tag == "Player")
		{
			if(background_music != null)
			{
				AudioManager.PlayBackgroundMusic(null);
			}
			if(environment_sound != null)
			{
				AudioManager.PlayEnvironmentSound(null);
			}
			foreach(Spawner s in spawners)
			{
				s.StopSpawn();
			}
			foreach(GameObject g in objects)
			{
				g.SetActive(false);
			}
		}
	}
}
