using UnityEngine;
using System.Collections;

public class IceBall : SingleTargetShoot {
	
	public float damage;
	public float slow_percent;
	public float slow_duration;
	
	
	
	protected sealed override void SingleTargetShootApplyEffectEnemyHook(StatusManager enemy_sm)
	{
		enemy_sm.ApplyInstanceMagicalDamageFix(damage * status_manager.spell_power, status_manager);
		enemy_sm.ApplySlowMove(slow_percent, slow_duration);
		enemy_sm.ApplySlowAttack(slow_percent, slow_duration);
	}
	
	protected sealed override string SkillGetDescription()
	{
		return "shoot a sinlge target ice pulse, dealing " + damage + " * spell power(" + damage * SkillManager.status_manager.spell_power 
			+ ") magical damage\nand slow movement and attack for " + slow_percent + "% for " + slow_duration + " seconds";
	}
}
