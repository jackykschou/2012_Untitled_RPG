using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour {
	
	public static GameObject manager;
	
	public AudioSource background_music;
	public AudioSource environment_sound;
	
	private AudioClip next_background_music;
	private AudioClip next_environment_sound;
	
	private bool bm_fade_out = false;
	private bool bm_fade_in = false;
	
	private bool env_fade_out = false;
	private bool env_fade_in = false;
	
	void OnLevelWasLoaded(int level)
	{
		manager = GameObject.Find("AudioManager");
	}
	
	public void Start()
	{
		manager = GameObject.Find("AudioManager");
	}
	
	public static void PlaySound(AudioClip clip, Vector3 pos)
	{
		if(clip != null)
		{
			AudioSource.PlayClipAtPoint(clip, pos);
		}
	}
	
	public static void PlayBackgroundMusic(AudioClip clip)
	{
		if(clip != null)
		{
			manager.SendMessage("PlayBackgroundMusicHelper", clip);
		}
		else
		{
			manager.SendMessage("StopBackgroundMusicHelper");
		}
	}
	
	public void PlayBackgroundMusicHelper(AudioClip clip)
	{
		next_background_music = clip;
		bm_fade_in = false;
		bm_fade_out = true;
	}
	
	public void StopBackgroundMusicHelper()
	{
		next_background_music = background_music.clip;
		bm_fade_in = false;
		bm_fade_out = true;
	}
	
	public void FadeInBackgroundMusic()
	{
		background_music.Stop();
		background_music.clip = next_background_music;
		background_music.Play();
		bm_fade_out = false;
		bm_fade_in = true;
	}
	
	public static void PlayEnvironmentSound(AudioClip clip)
	{
		if(clip != null)
		{
			manager.SendMessage("PlayEnvironmentSoundHelper", clip);
		}
		else
		{
			manager.SendMessage("StopEnvironmentSoundHelper");
		}
	}
	
	public void PlayEnvironmentSoundHelper(AudioClip clip)
	{
		next_environment_sound = clip;
		env_fade_in = false;
		env_fade_out = true;
	}
	
	public void StopEnvironmentSoundHelper()
	{
		next_environment_sound = environment_sound.clip;
		env_fade_in = false;
		env_fade_out = true;
	}
	
	public void FadeInEnvironmentSound()
	{
		environment_sound.Stop();
		environment_sound.clip = next_environment_sound;
		environment_sound.Play();
		env_fade_out = false;
		env_fade_in = true;
	}
	
	void Update()
	{
		if(bm_fade_in)
		{
			if(background_music.volume < 1.0f)
			{
				background_music.volume += Time.deltaTime * 0.25f;
			}
			else
			{
				bm_fade_in = false;
			}
		}
		else if(bm_fade_out)
		{
			if(background_music.volume > 0f)
			{
				background_music.volume -= Time.deltaTime * 0.25f;
			}
			else
			{
				bm_fade_out = false;
				FadeInBackgroundMusic();
			}
		}
		if(env_fade_in)
		{
			if(environment_sound.volume < 1.0f)
			{
				environment_sound.volume += Time.deltaTime * 0.25f;
			}
			else
			{
				env_fade_in = false;
			}
		}
		else if(env_fade_out)
		{
			if(environment_sound.volume > 0f)
			{
				environment_sound.volume -= Time.deltaTime * 0.25f;
			}
			else
			{
				env_fade_out = false;
				FadeInEnvironmentSound();
			}
		}
		if(Input.GetKeyDown(KeyCode.M))
		{
			if(AudioListener.volume >= 1.0f)
			{
				AudioListener.volume = 0f;
			}
			else
			{
				AudioListener.volume = 1.0f;
			}
		}
	}
}
