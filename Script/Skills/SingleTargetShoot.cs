using UnityEngine;
using System.Collections;

public abstract class SingleTargetShoot : CastableSkill {
	
	public GameObject bullet;
	public GameObject explosion;
	public float shoot_speed;
	public float duration;
	
	protected sealed override void CastableSkillStartEffectHook()
	{
		GameObject bullet_temp = Instantiate(bullet, status_manager.shoot_weapon_pos.position, status_manager.shoot_weapon_pos.rotation) as GameObject;
		SingleTargetBullet bullet_detector = bullet_temp.AddComponent("SingleTargetBullet") as SingleTargetBullet;
		if(ownership == Ownership.player) //if caster is player
		{
			Vector3 pos = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 70f));
			bullet_detector.StartShoot(this, pos , shoot_speed, explosion, duration);
		}
		else if(ownership == Ownership.enemyAi) //if caster is enemy ai
		{
			GameObject player = status_manager.target;
			if(player != null)
			{
				bullet_detector.StartShoot(this, player.transform.position , shoot_speed, explosion, duration);
			}
		}
		else //if caster is pet ai
		{
			GameObject enemy = status_manager.target;
			if(enemy != null)
			{
				bullet_detector.StartShoot(this, enemy.transform.position , shoot_speed, explosion, duration);
			}
		}
	}
	
	protected sealed override void CastableSkillActiviateHook()
	{
		status_manager.animator.SetBool(anim_name, true);
	}
	
	protected sealed override void CastableSkillApplyEffectEnemyHook(StatusManager enemy_sm)
	{
		SingleTargetShootApplyEffectEnemyHook(enemy_sm);
	}
	protected abstract void SingleTargetShootApplyEffectEnemyHook(StatusManager enemy_sm);
}
