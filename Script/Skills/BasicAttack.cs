using UnityEngine;
using System.Collections;

//base class for basic attack skill
public abstract class BasicAttack : Skill {
	
	protected sealed override void SkillStartHook()
	{
		passive = false;
	}
	
	protected sealed override void SkillStartEffectHook()
	{
		BasicAttackStartEffectHook();
	}
	protected abstract void BasicAttackStartEffectHook();
	
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
		status_manager.animator.SetBool(anim_name, true);
		status_manager.animator.SetBool("finishAttack", false);
		actual_cooldown = original_cooldown / status_manager.attack_speed;
	}
	
	protected sealed override void SkillApplyCostHook(){}
	
	protected sealed override bool SkillCheckCastableHook()
	{
		return !status_manager.is_knocked && !status_manager.is_stun && 
			!status_manager.is_falling && !status_manager.is_move_casting && !status_manager.is_stand_casting;
	}
	
	protected sealed override void SkillSetUpHook()
	{
		actual_cooldown = original_cooldown / status_manager.attack_speed; //cool down based on attack speed
	}
	
	protected sealed override void SkillApplyEffectEnemyHook(StatusManager enemy_sm)
	{	
		if((Random.value <= enemy_sm.dodge_chance) && (enemy_sm.dodge_chance >= 0.005f))
		{
			enemy_sm.text_generator.ShowStatus("Dodged");
		}
		else
		{
			BasicAttackApplyEffectEnemyHook(enemy_sm);
			foreach(EffectApplier s in status_manager.basic_attack_passives.Keys)
			{
				for(int i = status_manager.basic_attack_passives[s]; i > 0; ++i)
				{
					s.ApplyEffectEnemy(enemy_sm);
				}
			}
		}
	}
	protected abstract void BasicAttackApplyEffectEnemyHook(StatusManager enemy_sm);
}
