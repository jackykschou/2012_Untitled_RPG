using UnityEngine;
using System.Collections;
using System.Collections.Generic; //HashSet

public class StandardBasicAttack : BasicAttack {
	
	
	protected sealed override void BasicAttackStartEffectHook()
	{
		float attack_detection_time = 0.2f;
		OneTimeColliderEnemyCollector collector = status_manager.basic_attack_pos.gameObject.AddComponent("OneTimeColliderEnemyCollector") as OneTimeColliderEnemyCollector;
		collector.CollectEnemies(attack_detection_time, this);

	}
	
	protected sealed override void BasicAttackApplyEffectEnemyHook(StatusManager enemy_sm)
	{
		//apply damage, possible critical
		bool crit = (Random.value <= status_manager.crit_chance && status_manager.crit_chance != 0f);
		float damage = crit ? 
			(status_manager.attack_damage * status_manager.crit_damage) : (status_manager.attack_damage);
		enemy_sm.ApplyInstancePhysicalAttackDamageFix(damage, status_manager);
		
		enemy_sm.TempChangePhysicalDamagePerSecFix(status_manager.posion_damage, status_manager.posion_dur);
		enemy_sm.ApplySlowAttack(status_manager.posion_attack_slow, status_manager.posion_dur);
		enemy_sm.ApplySlowMove(status_manager.posion_move_slow, status_manager.posion_dur);
		
		if(Random.value <= status_manager.stun_chance && status_manager.stun_chance != 0f)
		{
			enemy_sm.ApplyStun(status_manager.stun_dur);
		}
	}
	
	protected sealed override string SkillGetDescription()
	{
		return "melee basic attack";
	}
}
