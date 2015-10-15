using UnityEngine;
using System.Collections;

public class Messenger : MonoBehaviour 
{
	public GUIText big_message;
	public GUIText small_message;
	public GUIText warning_message;
	
	private static GameObject messenger;
	
	void OnLevelWasLoaded(int level)
	{
		messenger =	GameObject.Find("GUITextMessenger");
	}
	
	void Start()
	{
		messenger =	GameObject.Find("GUITextMessenger");
	}
	
	public static void DisplayBigMessage(string message)
	{
		messenger.SendMessage("ShowBigMessage", message); 
	}
	
	public static void DisplaySmallMessage(string message)
	{
		messenger.SendMessage("ShowSmallMessage", message); 
	}
	
	public static void DisplayWarningMessage(string message)
	{
		messenger.SendMessage("ShowWarningMessage", message); 
	}


	public void ShowBigMessage(string message)
	{
		StartCoroutine(ShowBigMessageIE(message));
	}
	
	private static float big_message_show_time = 0f;
	
	private IEnumerator ShowBigMessageIE(string message)
	{
		if(big_message_show_time > 0f)
		{
			big_message.text = message;
			big_message_show_time = 3.5f;
		}
		else
		{
			big_message_show_time = 3.5f;
			big_message.text = message;
			big_message.enabled = true;
			while(big_message_show_time > 0f)
			{
				yield return new WaitForSeconds(Time.deltaTime);
				big_message_show_time -= Time.deltaTime;
			}
			big_message.enabled = false;
		}
	}
	
	public void ShowSmallMessage(string message)
	{
		StartCoroutine(ShowSmallMessageIE(message));
	}
	
	private static float small_message_show_time = 0f;
	
	private IEnumerator ShowSmallMessageIE(string message)
	{
		if(small_message_show_time > 0f)
		{
			small_message.text = message;
			small_message_show_time = 2f;
		}
		else
		{
			small_message_show_time = 2f;
			small_message.text = message;
			small_message.enabled = true;
			while(small_message_show_time > 0f)
			{
				yield return new WaitForSeconds(Time.deltaTime);
				small_message_show_time -= Time.deltaTime;
			}
			small_message.enabled = false;
		}
	}
	
	public void ShowWarningMessage(string message)
	{
		StartCoroutine(ShowWarningMessageIE(message));
	}
	
	private static float warning_message_show_time = 0f;
	
	private IEnumerator ShowWarningMessageIE(string message)
	{
		if(warning_message_show_time > 0f)
		{
			warning_message.text = message;
			warning_message_show_time = 3.5f;
		}
		else
		{
			warning_message_show_time = 3.5f;
			warning_message.text = message;
			warning_message.enabled = true;
			while(warning_message_show_time > 0f)
			{
				yield return new WaitForSeconds(Time.deltaTime);
				warning_message_show_time -= Time.deltaTime;
			}
			warning_message.enabled = false;
		}
	}
	
}
