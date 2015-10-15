using UnityEngine;
using System.Collections;

public class PlayerStatusManager : StatusManager {
	
	public int level;
	public float current_exp;
	public float exp_to_level_up;
	
	public AudioClip level_up_sound;
	
	public float max_health_up;
	public float max_mana_up;
	public float health_regen_up;
	public float mana_regen_up;
	public float attack_damage_up;
	public float spell_power_up;
	public float attack_speed_up;
	public float dodge_chance_up;
	public float crit_chance_up;
	public float block_damage_up;
	public float physics_resist_up;
	public float magic_resist_up;
	
	
	protected override sealed void StartHook()
	{
		SkillManager.status_manager = this;
		ItemManager.status_manager = this;
	}
	
	public void AddExp(float exp)
	{
		if((current_exp + exp) >= exp_to_level_up)
		{
			float original_exp_to_level_up = exp_to_level_up;
			LevelUp();
			AddExp((current_exp + exp) - original_exp_to_level_up);
		}
		else
		{
			current_exp += exp;
		}
	}
	
	public void LevelUp()
	{
		AudioManager.PlaySound(level_up_sound, transform.position);
		Messenger.DisplayBigMessage("Level Up!");
		++level;
		current_exp = 0f;
		exp_to_level_up = exp_to_level_up * 2.0f;
		max_health += max_health_up;
		HealFix(max_health);
		max_mana += max_mana_up;
		ChangeManaFix(max_mana);
		health_regen += health_regen_up;
		mana_regen += mana_regen_up;
		attack_damage += attack_damage_up;
		spell_power += spell_power_up;
		attack_speed += attack_speed_up;
		dodge_chance += dodge_chance_up;
		crit_chance += crit_chance_up;
		block_damage += block_damage_up;
		magic_resist += magic_resist_up;
		physics_resist += physics_resist_up;
	}
}
