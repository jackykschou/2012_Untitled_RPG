using UnityEngine;
using System.Collections;

public abstract class AreaLasting : AreaSelectionSkill {
	
	public GameObject effect;
	public float effect_duration;
	
	protected sealed override void AreaSelectionSkillStartEffectHook()
	{
		GameObject effect_temp = Instantiate(effect, cast_pos, Quaternion.identity) as GameObject;
		LastingColliderEnemyCollector collector = effect_temp.AddComponent("LastingColliderEnemyCollector") as LastingColliderEnemyCollector;
		collector.CollectEnemies(effect_duration, this);
		Destroy(effect_temp, effect_duration);
	}
	
	protected sealed override void AreaSelectionSkillApplyEffectEnemyHook(StatusManager enemy_sm){AreaOneInstanceApplyEffectEnemyHook(enemy_sm);}
	protected abstract void AreaOneInstanceApplyEffectEnemyHook(StatusManager enemy_sm);
}
