using UnityEngine;
using System.Collections;

public abstract class Spawner : MonoBehaviour {
	
	public GameObject spawn_effect;
	public AudioClip spawn_sound;
	
	public abstract void Spawn();
	public abstract void StopSpawn();
	
}
