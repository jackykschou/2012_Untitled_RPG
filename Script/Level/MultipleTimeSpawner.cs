using UnityEngine;
using System.Collections;
using System;

public class MultipleTimeSpawner : Spawner {

	private bool triggered = false;
	
	public GameObject[] enemies;
	public float spawn_time;
	private float spawn_timer = 2.0f;
	
	public override sealed void Spawn()
	{
		triggered = true;
	}
	
	public override sealed void StopSpawn()
	{
		triggered = false;
	}
	
	public void Update()
	{
		if(triggered)
		{
			if(spawn_timer >= spawn_time)
			{
				spawn_timer = 0f;
				AudioManager.PlaySound(spawn_sound, transform.position);
				Instantiate(enemies[UnityEngine.Random.Range(0, enemies.GetLength(0))], transform.position, Quaternion.identity);
				if(spawn_effect != null)
				{
					Destroy(Instantiate(spawn_effect, transform.position, Quaternion.identity) as GameObject, 5.0f);
				}
			}
			else
			{
				spawn_timer +=	Time.deltaTime;
			}
		}
	}
}
