using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Item : EffectApplier {
	
	public enum Quality
	{
		normal, rare, legendary
	}
	
	public Color color
	{
		get
		{
			Color c = Color.white;
			switch (quality)
			{
				case Quality.normal:	c = Color.white; break;
				case Quality.rare:	c = Color.yellow; break;
				case Quality.legendary:	c = Color.green; break;
			}
			return c;
		}
	}
	
	public int id_in_pool;
	
	public Texture2D icon;
	public string name;
	public string description;
	public Quality quality;
	public bool usable; //whether the item can be used
	public bool consumable;
	public bool unique; //whther the item can be only equipped one instance at a time
	public int level_req;
	
	public AudioClip activate_sound;
	public AudioClip deny_activate_sound;
	
	public float cooldown;
	public float current_cooldown;
	public bool ready; //whether the cooldown is finished
	
	protected override void SkillApplyEffectEnemyHook(StatusManager enemy_sm){}
	
	public void SetUp(StatusManager sm)
	{
		ownership = Ownership.player;
		status_manager = sm;
		if(usable)
		{
			StartCoroutine(ResetCoolDown());
		}
		
		status_manager.ChangeMaxHealth(max_health);
		status_manager.ChangeMaxMana(max_mana);
		status_manager.ChangeHealthRegen(health_regen);
		status_manager.ChangeManaRegen(mana_regen);
		move_speed_amount = status_manager.move_speed * move_speed;
		status_manager.move_speed += move_speed_amount;
		dodge_speed_amount = status_manager.dodge_speed * dodge_speed;
		status_manager.dodge_speed += dodge_speed_amount;
		attack_speed_amount = status_manager.attack_speed * attack_speed;
		status_manager.attack_speed += attack_speed_amount;
		status_manager.ChangeDodgeChance(dodge_chance);
		status_manager.ChangeAttackDamage(attack_damage);
		status_manager.ChangeSpellPower(spell_power);
		status_manager.ChangeLifeSteal(life_steal);
		status_manager.ChangeSpellSteal(spell_steal);
		status_manager.ChangeManaSteal(mana_steal);
		status_manager.ChangeCritChance(crit_chance);
		status_manager.ChangeCritDamage(crit_damage);
		status_manager.heal_emp += heal_emp;
		status_manager.damage_emp += damage_emp;
		status_manager.ChangeBlockDamage(block_damage);
		status_manager.ChangePhysicalResist(physics_resist);
		status_manager.ChangeMagicalResist(magic_resist);
		status_manager.ChangeTenacity(tenacity);
		status_manager.ChangeCDR(cdr);
		status_manager.ChangeStunChance(stun_chance);
		status_manager.ChangeStunDur(stun_dur);
		
		SetUpHook(sm);
	}
	
	public virtual void SetUpHook(StatusManager sm){}
	
	public void Remove()
	{
		status_manager.ChangeMaxHealth(-max_health);
		status_manager.ChangeMaxMana(-max_mana);
		status_manager.ChangeHealthRegen(-health_regen);
		status_manager.ChangeManaRegen(-mana_regen);
		status_manager.move_speed -= move_speed_amount;
		status_manager.dodge_speed -= dodge_speed_amount;
		status_manager.attack_speed -= attack_speed_amount;
		status_manager.ChangeDodgeChance(-dodge_chance);
		status_manager.ChangeAttackDamage(-attack_damage);
		status_manager.ChangeSpellPower(-spell_power);
		status_manager.ChangeLifeSteal(-life_steal);
		status_manager.ChangeSpellSteal(-spell_steal);
		status_manager.ChangeManaSteal(-mana_steal);
		status_manager.ChangeCritChance(-crit_chance);
		status_manager.ChangeCritDamage(-crit_damage);
		status_manager.heal_emp -= heal_emp;
		status_manager.damage_emp -= damage_emp;
		status_manager.ChangeBlockDamage(-block_damage);
		status_manager.ChangePhysicalResist(-physics_resist);
		status_manager.ChangeMagicalResist(-magic_resist);
		status_manager.ChangeTenacity(-tenacity);
		status_manager.ChangeCDR(-cdr);
		status_manager.ChangeStunChance(-stun_chance);
		status_manager.ChangeStunDur(-stun_dur);
		
		RemoveHook();
	}
	
	protected virtual void RemoveHook(){}
	
	private IEnumerator ResetCoolDown()
	{
		if(cooldown > 0f)
		{
			ready = false;
			current_cooldown = cooldown;
			while(current_cooldown >= 0f)
			{
				current_cooldown -= Time.deltaTime;
				yield return new WaitForSeconds(Time.deltaTime);
			}
		}
		ready = true;
	}
	
	public void Activate()
	{
		StartCoroutine(ResetCoolDown());
		StartEffect();
	}
	
	protected virtual void StartEffect()
	{
		
	}
	
	private string ShowAttribute(float attribute, string name, bool percent)
	{
		if(attribute != 0f)
		{
			return "" + (attribute > 0f ? "+" : "-") + attribute * (percent ? 100f : 1.0f) + (percent ? "%" : "") + " " + name + "\n";
		}
		else
		{
			return "";
		}
	}
	
	public string GetDescription()
	{
		return "[" + NGUITools.EncodeColor(color) + "]" + name + "[-]\n" + 
		(unique ? "UNIQUE" : "") +
		"[" + NGUITools.EncodeColor(Color.red) + "]" + "Level Require: " + level_req + "[-]\n" + 
		"[" + NGUITools.EncodeColor(Color.white) + "]" 
		+ ShowAttribute(max_health, "max health", false)
		+ ShowAttribute(max_mana, "max mana", false)
		+ ShowAttribute(health_regen, "health regen", false)
		+ ShowAttribute(mana_regen, "mana regen", false)
		+ ShowAttribute(move_speed, "movement speed", true)
		+ ShowAttribute(dodge_speed, "dodge speed", true)
		+ ShowAttribute(attack_damage, "attack damage", false)
		+ ShowAttribute(spell_power, "spell power", true)
		+ ShowAttribute(attack_speed, "attack speed", true)
		+ ShowAttribute(life_steal, "life steal", true)
		+ ShowAttribute(spell_steal, "spell steal", true)
		+ ShowAttribute(dodge_chance, "dodge chance", true)
		+ ShowAttribute(crit_chance, "critical chance", true)
		+ ShowAttribute(crit_damage, "critical damage", true)
		+ ShowAttribute(block_damage, "block damage", false)
		+ ShowAttribute(physics_resist, "physical resistance", true)
		+ ShowAttribute(magic_resist, "magical resistance", true)
		+ ShowAttribute(tenacity, "tenacity", true)
		+ ShowAttribute(cdr, "cooldown reduction", true)
		+ "\n"
		+ ((heal_emp != 0f) ? "Increases all healing effects by " + heal_emp * 100f +"%\n": "")
		+ ((damage_emp != 0f) ? "Increases all damage taken by " + damage_emp * 100f +"%\n": "")
		+ ((stun_chance != 0f) ? "Increases stackable stun chance from basic attack by " + stun_chance * 100f +"%\n": "") + "[-]"
		+ ((stun_dur != 0f) ? "Increases stackable stun duration from basic attack by " + stun_dur * 100f +"%\n": "") + "[-]"
		+ "[" + NGUITools.EncodeColor(Color.magenta) + "]" 
		+ (consumable ? "Consumable\n" : "")
		+ (usable ? "Active:\n" : "")
		+ description + "[-]";
		
	}
	
	public float max_health;
	public float max_mana;
	public float health_regen;
	public float mana_regen;
	public float move_speed;
	public float move_speed_amount;
	public float dodge_speed;
	public float dodge_speed_amount;
	public float attack_damage;
	public float spell_power;
	public float attack_speed;
	public float attack_speed_amount;
	public float life_steal;
	public float spell_steal;
	public float mana_steal;
	public float dodge_chance;
	public float crit_chance;
	public float crit_damage;
	public float stun_chance;
	public float stun_dur;
	
	public float heal_emp;
	public float damage_emp;
	
	public float block_damage;
	public float physics_resist;
	public float magic_resist;
	public float tenacity;
	public float cdr;
}
