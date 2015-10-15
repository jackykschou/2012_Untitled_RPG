using UnityEngine;
using System.Collections;

public abstract class AreaOneInstance : AreaSelectionSkill {
	
	public GameObject effect;
	public float effect_duration;
	
	protected sealed override void AreaSelectionSkillStartEffectHook()
	{
		GameObject effect_temp = Instantiate(effect, cast_pos, Quaternion.identity) as GameObject;
		OneInstanceColliderEnemyCollector collector = effect_temp.AddComponent("OneInstanceColliderEnemyCollector") as OneInstanceColliderEnemyCollector;
		collector.CollectEnemies(effect_duration, this);
		Destroy(effect_temp, effect_duration);
	}
	
	protected sealed override void AreaSelectionSkillApplyEffectEnemyHook(StatusManager enemy_sm){AreaOneInstanceApplyEffectEnemyHook(enemy_sm);}
	protected abstract void AreaOneInstanceApplyEffectEnemyHook(StatusManager enemy_sm);
}
