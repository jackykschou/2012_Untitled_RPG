using UnityEngine;
using System.Collections;

public abstract class CastableSkill : Skill {
	
	protected sealed override void SkillStartHook()
	{
		passive = false;	
	}
	
	protected sealed override void SkillStartEffectHook(){CastableSkillStartEffectHook();}
	protected abstract void CastableSkillStartEffectHook();
	
	protected sealed override void SkillActiviateHook()
	{	
		if(movable)
		{
			status_manager.is_move_casting = true;
			status_manager.animator.SetLayerWeight(1, 1.0f);
		}
		else
		{
			status_manager.is_stand_casting = true;
			status_manager.animator.SetLayerWeight(2, 1.0f);
		}
		foreach(EffectApplier s in status_manager.cast_spell_passives.Keys)
		{
			for(int i = status_manager.cast_spell_passives[s]; i > 0; ++i)
			{
				s.ApplyEffectEnemy(null);
			}
		}
		CastableSkillActiviateHook();
		actual_cooldown = original_cooldown * (1 - status_manager.cdr);
		status_manager.animator.SetBool("finishCast", false);
	}
	protected abstract void CastableSkillActiviateHook();
	
	protected sealed override void  SkillApplyCostHook(){}
	
	protected override sealed bool SkillCheckCastableHook()
	{
		return !status_manager.is_slienced && !status_manager.is_knocked 
			&& !status_manager.is_stun && !status_manager.is_falling && !status_manager.is_move_casting && !status_manager.is_stand_casting;
	}
	
	protected override sealed void SkillSetUpHook()
	{
		actual_cooldown = original_cooldown * (1 - status_manager.cdr);
	}
	
	protected override sealed void SkillApplyEffectEnemyHook(StatusManager enemy_sm)
	{
		if(enemy_sm.spell_block)
		{
			enemy_sm.UseSpellBlock();
		}
		else
		{
			CastableSkillApplyEffectEnemyHook(enemy_sm);
		}
	}
	protected abstract void CastableSkillApplyEffectEnemyHook(StatusManager enemy_sm);
	
}
