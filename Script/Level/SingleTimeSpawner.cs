using UnityEngine;
using System.Collections;

public class SingleTimeSpawner : Spawner {
	
	private bool spawned = false;
	public float spawn_chance;
	
	public GameObject enemy;
	
	public override sealed void Spawn()
	{
		if(spawn_chance >= Random.value && !spawned)
		{
			spawned = true;
			AudioManager.PlaySound(spawn_sound, transform.position);
			Instantiate(enemy, transform.position, Quaternion.identity);
			if(spawn_effect != null)
			{
				Destroy(Instantiate(spawn_effect, transform.position, Quaternion.identity) as GameObject, 5.0f);
			}
		}
	}
	
	public override sealed void StopSpawn()
	{
		Destroy(gameObject, 3.0f);	
	}
}
