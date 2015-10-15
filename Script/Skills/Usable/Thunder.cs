using UnityEngine;
using System.Collections;

public class Thunder : AreaOneInstance {
	
	public float damage;
	public float stun_chance;
	public float stun_duration;

	protected sealed override void AreaOneInstanceApplyEffectEnemyHook(StatusManager enemy_sm)
	{
		enemy_sm.ApplyInstanceMagicalDamageFix(damage * status_manager.spell_power, status_manager);
		if(Random.value <= stun_chance)
		{
			enemy_sm.ApplyStun(stun_duration);
		}
	}
	
	protected sealed override string SkillGetDescription()
	{
		return "strike a thunder to the ground, dealing " + damage + " * spell power(" + damage * SkillManager.status_manager.spell_power + ") magical damage in the area\n " +
			"thunder has " + stun_chance * 100 + "% chance to stun enemy for " + stun_duration + "s";
	}
	
}
