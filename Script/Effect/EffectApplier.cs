using UnityEngine;
using System.Collections;

public abstract class EffectApplier : MonoBehaviour {

	public enum Ownership
	{
		player, petAi, enemyAi
	}
	
	public StatusManager status_manager;
	
	public void ApplyEffectEnemy(StatusManager enemy_sm)
	{
		AudioManager.PlaySound(apply_enemy_effect_sound, enemy_sm.gameObject.transform.position);
		if(!enemy_sm.invulnerable)
		{
			SkillApplyEffectEnemyHook(enemy_sm);
		}
		else
		{
			enemy_sm.text_generator.ShowStatus("Invulnerable!");	
		}
	}
	protected abstract void SkillApplyEffectEnemyHook(StatusManager enemy_sm);
	public AudioClip apply_enemy_effect_sound;
	public Ownership ownership; //whether the skill being used by AI but not the player
}
