using UnityEngine;
using System.Collections;

public class FireCloud : MultiTargetShoot {
	
	public float damage;
	
	protected sealed override void MultiTargetShootApplyEffectEnemyHook(StatusManager enemy_sm)
	{
		enemy_sm.ApplyInstanceMagicalDamageFix(damage * status_manager.spell_power, status_manager);
	}
	
	protected sealed override string SkillGetDescription()
	{
		return "shoot a fire cloud, dealing " + damage + " * spell power(" + damage * SkillManager.status_manager.spell_power + ") magical damage to all enemies it passes through";
	}
}

