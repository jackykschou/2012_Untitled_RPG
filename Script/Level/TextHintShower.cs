using UnityEngine;
using System.Collections;

public class TextHintShower : MonoBehaviour {
	
	public string message;
	public bool warning;
	public bool one_time;
	public AudioClip sound_effect;
	bool triggered = false;
	
	void OnTriggerEnter()
	{
		if(!triggered)
		{
			if(one_time)
			{
				triggered = true;
			}
			AudioManager.PlaySound(sound_effect, transform.position);
			if(warning)
			{
				Messenger.DisplayWarningMessage(message);	
			}
			else
			{
				Messenger.DisplayBigMessage(message);
			}
		}
	}
}
