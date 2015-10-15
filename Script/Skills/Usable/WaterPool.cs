using UnityEngine;
using System.Collections;

public class WaterPool : AreaLasting {
	
	public float damage;

	protected sealed override void AreaOneInstanceApplyEffectEnemyHook(StatusManager enemy_sm)
	{
		enemy_sm.ApplyInstanceMagicalDamageFix(damage * status_manager.spell_power, status_manager);
	}
	
	protected sealed override string SkillGetDescription()
	{
		return "summon a magical water force, dealing " + damage + " * spell power(" + damage * SkillManager.status_manager.spell_power + ") magical damage every second to enemies in the area for " + effect_duration + "s";
	}
	
}
