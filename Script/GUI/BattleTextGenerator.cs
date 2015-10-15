using UnityEngine;
using System.Collections;

public class BattleTextGenerator : MeshTextGenerator {
	
	public StatusManager status_manager;
	public TextMesh heal;
	public TextMesh physical_damage;
	public TextMesh magic_damge;
	public TextMesh pure_damage;
	public TextMesh status;
	
	//the text will only be shown when larger than a certain value
	
	public void ShowHeal(float amount)
	{	
		if(!status_manager.is_dead)
		{
			if(amount >= status_manager.max_health * 0.02f && (status_manager.current_health != status_manager.max_health))
			{
				heal.text = ((int)amount).ToString();
				ShowMessageRandom(heal, transform);
			}
		}
	}
	
	public void ShowPhysicalDamage(float amount)
	{	
		if(!status_manager.is_dead)
		{
			if(status_manager.physics_resist == 1.0f)
			{
				status.text = "Physical Damage Immune";
				ShowMessageRandom(magic_damge, transform);
			}
			if(amount >= status_manager.current_health * 0.03f)
			{
				physical_damage.text = ((int)amount).ToString();
				ShowMessageRandom(physical_damage, transform);
			}
		}
	}
	
	public void ShowMagicDamage(float amount)
	{
		if(!status_manager.is_dead)
		{
			if(status_manager.magic_resist == 1.0f)
			{
				status.text = "Magical Damage Immune";
				ShowMessageRandom(magic_damge, transform);
			}
			else if(amount >= status_manager.max_health * 0.03f)
			{
				magic_damge.text = ((int)amount).ToString();
				ShowMessageRandom(magic_damge, transform);
			}
		}
	}
	
	public void ShowPureDamage(float amount)
	{
		if(!status_manager.is_dead)
		{
			if(amount >= status_manager.max_health * 0.03f)
			{
				pure_damage.text = ((int)amount).ToString();
				ShowMessageRandom(pure_damage, transform);
			}
		}
	}
	
	public void ShowStatus(string message)
	{
		if(!status_manager.is_dead)
		{
			status.text = message;
			ShowMessageRandom(status, transform);
		}
	}
}
