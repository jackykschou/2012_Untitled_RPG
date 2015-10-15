using UnityEngine;
using System.Collections;

public class StatusGUIManager : MonoBehaviour {
	
	public static GameObject manager;
	
	public EnergyBar health_bar;
	public EnergyBar mana_bar;
	public EnergyBar exp_bar;
	public AudioClip dying_sound;
	
	public PlayerStatusManager status_manager;
	
	public UISlider[] skill_cooldown_counters = new UISlider[6];
	public UITexture[] skill_icons = new UITexture[6];
	
	public UISlider[] item_cooldown_counters = new UISlider[6];
	public UITexture[] item_icons = new UITexture[6];
	
	
	void Start()
	{
		manager = GameObject.Find("MainCharacter");
	}
	
	void Update()
	{
		health_bar.SetValueF(status_manager.current_health / status_manager.max_health);
		mana_bar.SetValueF(status_manager.current_mana / status_manager.max_mana);
		exp_bar.SetValueF(status_manager.current_exp / status_manager.exp_to_level_up);
		if((status_manager.current_health / status_manager.max_health)  <= 0.1f)
		{
			PlayDyingSound();
		}
		UpdateSkillCoolDown(0);
		UpdateSkillCoolDown(1);
		UpdateSkillCoolDown(2);
		UpdateSkillCoolDown(3);
		UpdateSkillCoolDown(4);
		UpdateSkillCoolDown(5);
		
		UpdateItemCoolDown(0);
		UpdateItemCoolDown(1);
		UpdateItemCoolDown(2);
		UpdateItemCoolDown(3);
		UpdateItemCoolDown(4);
		UpdateItemCoolDown(5);
	}
	
	void UpdateSkillCoolDown(int i)
	{
		if(SkillManager.skills[i] != null)
		{
			skill_cooldown_counters[i].sliderValue = SkillManager.skills[i].current_cooldown / SkillManager.skills[i].actual_cooldown;
		}
		else
		{
			skill_cooldown_counters[i].sliderValue = 1f;
		}
	}
	
	void UpdateItemCoolDown(int i)
	{
		if(ItemManager.items[i] != null && ItemManager.items[i].usable)
		{
			item_cooldown_counters[i].sliderValue = ItemManager.items[i].current_cooldown / ItemManager.items[i].cooldown;
		}
		else
		{
			item_cooldown_counters[i].sliderValue = 0f;
		}
	}
	
	private static Texture item_icon_helper;
	
	public static void ChangeItemIcon(int index, Texture icon)
	{
		item_icon_helper = icon;
		manager.SendMessage("ChangeItemIconHelper", index);
	}
	
	public void ChangeItemIconHelper(int index)
	{
		item_icons[index].enabled = true;
		item_icons[index].mainTexture = item_icon_helper;
	}
	
	public static void EmptyItemIcon(int index)
	{
		manager.SendMessage("EmptyItemIconHelper", index);
	}
	
	public void EmptyItemIconHelper(int index)
	{
		item_icons[index].enabled = false;
		item_icons[index].mainTexture = null;
	}
	
	private static Texture skill_icon_helper;
	
	public static void ChangeSkillIcon(int index, Texture icon)
	{
		skill_icon_helper = icon;
		manager.SendMessage("ChangeSkillIconHelper", index);
	}
	
	public void ChangeSkillIconHelper(int index)
	{
		skill_icons[index].enabled = true;
		skill_icons[index].mainTexture = skill_icon_helper;
	}
	
	public static void EmptySkillIcon(int index)
	{
		manager.SendMessage("EmptySkillIconHelper", index);
	}
	
	public void EmptySkillIconHelper(int index)
	{
		skill_icons[index].mainTexture = null;
		skill_icons[index].enabled = false;
	}
	
	private float dying_sound_timer = 0f;
	
	public void PlayDyingSound()
	{
		if(dying_sound_timer <= 0)
		{
//			dying_sound_timer = dying_sound.length;
			AudioManager.PlaySound(dying_sound, transform.position);
		}
		else
		{
			dying_sound_timer -= Time.deltaTime;
		}
	}
}
