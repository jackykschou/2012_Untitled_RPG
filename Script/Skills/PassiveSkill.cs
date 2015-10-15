using UnityEngine;
using System.Collections;

public abstract class PassiveSkill : Skill {
	
	public bool basic_attack;
	public bool spell_cast;
	public bool get_physical_dam;
	public bool get_magical_dam;
	public bool update;
	
	private bool trigger = false; //for "update type" passive skill
	
	protected override sealed void SkillStartHook () 
	{
		passive = true;
	}
	
	protected override sealed void SkillActiviateHook()
	{
		StartEffect();	
	}
	
	protected override sealed void SkillSetUpHook()
	{
		if(basic_attack)
		{
			status_manager.basic_attack_passives.Add(this, 1);	
		}
		if(spell_cast)
		{
			status_manager.cast_spell_passives.Add(this, 1);
		}
		if(get_physical_dam)
		{
			status_manager.get_physical_dam_passives.Add(this, 1);
		}
		if(get_magical_dam)
		{
			status_manager.get_magical_dam_passives.Add(this, 1);
		}
		if(update)
		{
			trigger = true;
		}
		PassiveSkillSetUpHook();
	}
	
	public override sealed void Remove()
	{
		if(basic_attack)
		{
			status_manager.basic_attack_passives[this] = status_manager.basic_attack_passives[this] - 1;	
		}
		if(spell_cast)
		{
			status_manager.cast_spell_passives[this] = status_manager.cast_spell_passives[this] - 1;
		}
		if(get_physical_dam)
		{
			status_manager.get_physical_dam_passives[this] = status_manager.get_physical_dam_passives[this] - 1;
		}
		if(get_magical_dam)
		{
			status_manager.get_magical_dam_passives[this] = status_manager.get_magical_dam_passives[this] - 1;
		}
		if(update)
		{
			trigger = false;
		}
		PassiveSkillRemoveHook();
	}
	
	protected override sealed void SkillApplyCostHook(){}
	protected override sealed bool SkillCheckCastableHook(){return true;}
	
	protected abstract void PassiveSkillSetUpHook();
	protected abstract void PassiveSkillRemoveHook();
	protected abstract void UpdateHook();
	
	void Update()
	{
		if(trigger)
		{
			UpdateHook();	
		}
	}
}
