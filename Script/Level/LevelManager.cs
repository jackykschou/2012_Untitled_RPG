using UnityEngine;
using System.Collections;

public class LevelManager : MonoBehaviour {
	
	public static GameObject manager;
	public AudioClip dead_music;
	public AudioSource background_music;
	public AudioSource environment_sound;
	public GameObject enter_effect;
	public AudioClip travel_sound;
	
	public Transform player_start_pos;
	public Transform invisible_pos;
	
	public GameObject main_character;
	public static PlayerStatusManager player_status;
	
	public void Start()
	{
		manager = GameObject.Find("LevelMessenger");
		main_character = GameObject.Find("MainCharacter");
		main_character.transform.position = player_start_pos.position;
		Destroy(Instantiate(enter_effect, player_start_pos.position, Quaternion.identity) as GameObject, 5.0f);
		AudioManager.PlaySound(travel_sound, transform.position);
		main_character.tag = "Player";
		player_status = (main_character.GetComponent("PlayerStatusManager") as PlayerStatusManager);
		
		player_status.HealFix(player_status.max_health);
		player_status.ChangeManaFix(player_status.max_mana);
		
		GameObject.Find("AudioManager").transform.position = invisible_pos.position;
		GameObject.Find("GUITextMessenger").transform.position = invisible_pos.position;
		GameObject.Find("MecanimEventManager").transform.position = invisible_pos.position;
		
		AudioManager.PlayEnvironmentSound(null);
		AudioManager.PlayBackgroundMusic(null);
	}
	
	public static void ChangeLevel(string level_name)
	{
		AudioManager.PlayEnvironmentSound(null);
		AudioManager.PlayBackgroundMusic(null);
		GameObject.Find("MainCharacter").tag = "Untagged";
		Application.LoadLevel(level_name);
	}
	
	private bool dead = false;
	
	public void Update()
	{
		if(player_status.is_dead && !dead)
		{
			player_status.current_exp = 0f;
			dead = true;
			StartDead();
		}
	}
	
	private IEnumerator StartDead()
	{
		Messenger.DisplayWarningMessage("You Are Dead");
		AudioManager.PlaySound(dead_music, transform.position);
		yield return new WaitForSeconds(3.0f);
		player_status.Relieve();
		ChangeLevel("base");
	}
}
