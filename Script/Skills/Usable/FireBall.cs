using UnityEngine;
using System.Collections;

public class FireBall : SingleTargetShoot {
	
	public float damage;
	
	protected sealed override void SingleTargetShootApplyEffectEnemyHook(StatusManager enemy_sm)
	{
		enemy_sm.ApplyInstanceMagicalDamageFix(damage * status_manager.spell_power, status_manager);
	}
	
	protected sealed override string SkillGetDescription()
	{
		return "shoot a sinlge target fire, dealing " + damage + " * spell power(" + damage * SkillManager.status_manager.spell_power + ") magical damage";
	}
}
