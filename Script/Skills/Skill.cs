/**
 * interface for Skills (all skills inherits it)
 **/

using UnityEngine;
using System.Collections;

//the base class of skill from where all skills derived
public abstract class Skill : EffectApplier
{
	public enum Quality
	{
		normal, rare, legendary
	}
	
	public enum AttackType
	{
		melee, range, anytype
	}
	
	public string weapon_req
	{
		get
		{
			string s = "";
			switch (attack_type)
			{
				case AttackType.melee: s = "melee weapon only"; break;
				case AttackType.range: s = "range weapon only"; break;
				case AttackType.anytype: s = ""; break;
			}
			return s;
		}
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
		
	public Texture2D icon;
	public string name;
	public int id; //unique skill id
	public Quality quality;
	public AttackType attack_type;
	public string anim_name;
	public int level_req;
	
	public AudioClip activate_sound;
	public AudioClip effect_sound;
	public AudioClip deny_activate_sound;
	
	public float original_cooldown;
	public float actual_cooldown;
	public float current_cooldown;
	public bool passive;
	public bool movable; //whether the skill can be casted while moving
	public bool ready; //whether the cooldown is finished
	
	public float current_health_cost;
	public float current_mana_cost;
	public float max_health_percent_cost;
	public float max_mana_percent_cost;
	public float current_health_percent_cost;
	public float current_mana_percent_cost;
	
	void Start()
	{
		SkillStartHook();	
	}
	protected abstract void SkillStartHook();
	
	public void StartEffect()
	{
		SkillStartEffectHook();
		AudioManager.PlaySound(effect_sound, transform.position);
	}
	protected abstract void SkillStartEffectHook();
	
	public void Activiate()
	{
		if(CheckCastable())
		{	
			SkillActiviateHook();
			ApplyCost();
			AudioManager.PlaySound(activate_sound, transform.position);
			StartCoroutine(ResetCoolDown());
		}
	}
	protected abstract void SkillActiviateHook();
	
	public void ApplyCost()
	{
		//only player have cost when using skill
		if(ownership == Ownership.player)
		{
			status_manager.ApplyPureDamageFix(current_health_cost);
			status_manager.ChangeManaFix(-current_mana_cost);
			status_manager.ApplyPureDamageMaxPercent(max_health_percent_cost);
			status_manager.ChangeManaMaxPercent(-max_mana_percent_cost);
			status_manager.ApplyPureDamageCurrentPercent(current_health_percent_cost);
			status_manager.ChangeManaCurrentPercent(-current_mana_percent_cost);
			SkillApplyCostHook();
		}
	}
	protected abstract void SkillApplyCostHook();
	
	//check if the character has the right condition to cast the spell
	private bool CheckCastable()
	{
		bool enough_cost = (status_manager.current_health > current_health_cost) && (status_manager.current_mana > current_mana_cost);
		bool result = ((ownership != Ownership.player) ? ready : (ready && enough_cost))
			&& !status_manager.is_dead && SkillCheckCastableHook();
		if(!result && ownership == Ownership.player && !passive)
		{
			if(!enough_cost)
			{
				AudioManager.PlaySound(deny_activate_sound, transform.position);
				Messenger.DisplaySmallMessage("Not Enough Cost");	
			}
			else if(!ready)
			{
				Messenger.DisplaySmallMessage("Skill On Cooldown");	
			}
		}
		return result;
	}
	protected abstract bool SkillCheckCastableHook();
	
	//called with the skill is assigned to a new character
	public void SetUp(StatusManager sm)
	{
		status_manager = sm;
		SkillSetUpHook();
		if(ownership != Ownership.player)
		{
			ready = true;
		}
		else
		{
			StartCoroutine(ResetCoolDown());	
		}
	}
	protected abstract void SkillSetUpHook();
	
	private IEnumerator ResetCoolDown()
	{
		if(original_cooldown > 0f)
		{
			ready = false;
			current_cooldown = actual_cooldown;
			while(current_cooldown >= 0f)
			{
				current_cooldown -= Time.deltaTime;
				yield return new WaitForSeconds(Time.deltaTime);
			}
		}
		ready = true;
	}
	
	private string ShowCost(float cost, string name)
	{
		if(cost != 0f)
		{
			return "" + cost + " " + name + "\n";
		}
		else
		{
			return "";
		}
	}
	
	public string GetDescription()
	{return "[" + NGUITools.EncodeColor(color) + "]" + name + "[-]\n" + 
		"[" + NGUITools.EncodeColor(Color.gray) + "]" + weapon_req + "[-]\n" + 
		"[" + NGUITools.EncodeColor(Color.red) + "]" + "Level Require: " + level_req + "[-]\n" + 
		"[" + NGUITools.EncodeColor(Color.white) + "]" + "cooldown: " + original_cooldown + "\n" + 
		"cost:\n" + ShowCost(current_health_cost, "current health") + ShowCost(current_mana_cost, "current mana") + ShowCost(max_health_percent_cost, "percent max health")
				+ ShowCost(max_mana_percent_cost, "percent max mana") + ShowCost(current_health_percent_cost, "percent current health") + ShowCost(current_mana_percent_cost, "percent current mana")
		+ "[-]\n" +
		"[" + NGUITools.EncodeColor(Color.white) + "]" + (passive ? "passive:\n" : "active:\n") + SkillGetDescription() + "[-]\n";}
	
	protected abstract string SkillGetDescription();
	
	public virtual void Remove(){}
}
