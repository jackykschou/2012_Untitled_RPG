
using UnityEngine;
using System.Collections;

public abstract class AreaSelectionSkill : CastableSkill {
	
	protected Vector3 cast_pos;
	
	public float range_unit;
	protected sealed override void CastableSkillStartEffectHook(){AreaSelectionSkillStartEffectHook();}
	protected abstract void AreaSelectionSkillStartEffectHook();
	
	protected sealed override void CastableSkillActiviateHook()
	{
		if(ownership == Ownership.player)
		{
			StartCoroutine(GetSelection());
		}
		else if(ownership == Ownership.enemyAi)
		{
			cast_pos = status_manager.target.transform.position;
			status_manager.animator.SetBool(anim_name, true);
		}
		else
		{
			cast_pos = GameObject.FindGameObjectWithTag("Enemy").transform.position;
			status_manager.animator.SetBool(anim_name, true);
		}
	}
	
	private IEnumerator GetSelection()
	{
		yield return StartCoroutine(status_manager.menu_selector.SkillSelection(range_unit));
		cast_pos = status_manager.menu_selector.select_pos;
		if(cast_pos != Vector3.zero)
		{
			status_manager.animator.SetBool(anim_name, true);
		}
		else
		{
			status_manager.FinishCast();	
		}
	}
	
	protected override sealed void CastableSkillApplyEffectEnemyHook(StatusManager enemy_sm){AreaSelectionSkillApplyEffectEnemyHook(enemy_sm);}
	protected abstract void AreaSelectionSkillApplyEffectEnemyHook(StatusManager enemy_sm);
	
}

