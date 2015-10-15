using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/*
 * store diffenernt status am imformation about a character / monster and manage them
 **/

public class StatusManager : MonoBehaviour 
{	
	public Animator animator; //animator of the character
	public BattleTextGenerator text_generator; //used to procuce different text that will be shown when different effects happen
	public CharacterController controller;
	
	public AudioClip[] walk_sounds;
	public AudioClip dead_sound;
	
	public Dictionary<EffectApplier, int> basic_attack_passives = new Dictionary<EffectApplier, int>();
	public Dictionary<EffectApplier, int> cast_spell_passives = new Dictionary<EffectApplier, int>();
	public Dictionary<EffectApplier, int> get_physical_dam_passives = new Dictionary<EffectApplier, int>();
	public Dictionary<EffectApplier, int> get_magical_dam_passives = new Dictionary<EffectApplier, int>();
	
	public MenuSelector menu_selector; //only player status manager can have it (Skill need to malipulate this object)
	public GameObject target; //only pet and enemy ai can have it (Skill need to malipulate this object)
	public Skill.AttackType weapon_type;
	
	//the positions of the occurence of different effects of the character
	public Transform basic_attack_pos;
	public Transform center_pos;
	public Transform head_pos;
	public Transform bottom_pos;
	public Transform channel_weapon_pos;
	public Transform shoot_weapon_pos;
	public Transform general_spell_weapon_pos;
	public Transform left_shoulder;
	public Transform right_shoulder;
	
	//diameter of the character
	public float diameter;
	
	public bool is_stun;
	public bool is_rooted;
	public bool is_slienced;
	public bool is_knocked;
	public bool invulnerable;
	public bool is_dead;
	public bool is_falling;
	public bool is_stand_casting;
	public bool is_move_casting;
	
	public float max_health;
	public float max_mana;
	public float current_health;
	public float current_mana;
	public float health_regen;
	public float mana_regen;
	public float move_speed;
	public float dodge_speed;
	public float attack_damage;
	public float spell_power;
	public float attack_speed;
	public float life_steal;
	public float spell_steal;
	public float mana_steal;
	public float dodge_chance;
	public float crit_chance;
	public float crit_damage;
	public float stun_chance; //chance to stun enemy on basic attack
	public float stun_dur; //duration of stun of basic attack
	public float heal_emp = 1.0f;
	public float damage_emp = 1.0f;
	
	//posion attack
	public float posion_damage;
	public float posion_attack_slow; //attack slow on basic attack
	public float posion_move_slow; //movement speed slow on basic attack
	public float posion_dur; //the duration of posion
	
	public float block_damage; //physical damage blocker
	public float physics_resist; //physical resistance (in percentage)
	public float magic_resist; //magic resistance (in percentage)
	public bool spell_block;
	public float tenacity;
	public float cdr; //cool down reduction
	
	public float heal_per_sec_fix = 0;
	public float heal_per_sec_max_percent = 0;
	public float heal_per_sec_current_percent = 0;
	public float mana_per_sec_change_fix = 0;
	public float mana_per_sec_change_max_percent = 0;
	public float damage_per_sec_phy_fix = 0;
	public float damage_per_sec_phy_max_percent = 0;
	public float damage_per_sec_phy_current_percent = 0;
	public float damage_per_sec_magic_fix = 0;
	public float damage_per_sec_magic_max_percent = 0;
	public float damage_per_sec_magic_current_percent = 0;
	public float damage_per_sec_pure_fix = 0;
	public float damage_per_sec_pure_max_percent = 0;
	public float damage_per_sec_pure_current_percent = 0;
	
	void Start()
	{
		InvokeRepeating("Regen", 2.0f, 1.0f);
		InvokeRepeating("HealPerSec", 2.0f, 1.0f);
		InvokeRepeating("ManaChangePerSec", 2.0f, 1.0f);
		InvokeRepeating("DamagePerSec", 2.0f, 1.0f);
		diameter = (collider.bounds.size.x > collider.bounds.size.y) ? (collider.bounds.size.x) : ( collider.bounds.size.y);
		StartHook();
	}
	protected virtual void StartHook(){}
	
	//-----------------------------------
	// functions that change the statuses
	// functions that end with the end IE are IEnumerator, which are wrapped by public non-IEnumerator
	//-----------------------------------
	
	private void Regen()
	{
	 	HealFix(health_regen);
		ChangeMana(mana_regen);
	}
	
	private void HealPerSec()
	{
		HealFix(heal_per_sec_fix);
		HealCurrentPercent(heal_per_sec_current_percent);
		HealMaxPercent(heal_per_sec_max_percent);
	}
	
	private void ManaChangePerSec()
	{
		ChangeManaFix(mana_per_sec_change_fix);
		ChangeManaMaxPercent(mana_per_sec_change_max_percent);
	}
	
	private void DamagePerSec()
	{
		ApplyMagicalDamageFix(damage_per_sec_magic_fix, null);
		ApplyMagicalDamageCurrentPercent(damage_per_sec_magic_current_percent, null);
		ApplyMagicalDamageMaxPercent(damage_per_sec_magic_max_percent, null);
		ApplyPhysicalDamageFix(damage_per_sec_phy_fix, null);
		ApplyPhysicalDamageCurrentPercent(damage_per_sec_phy_current_percent, null);
		ApplyPhysicalDamageMaxPercent(damage_per_sec_phy_max_percent, null);
		ApplyPureDamageFix(damage_per_sec_pure_fix);
		ApplyPureDamageCurrentPercent(damage_per_sec_pure_current_percent);
		ApplyPureDamageMaxPercent(damage_per_sec_pure_max_percent);
	
	}
	
	public void ChangeHealPerSecFix(float amount)
	{	
		heal_per_sec_fix = Mathf.Min(Mathf.Max(0f, heal_per_sec_fix + amount), Mathf.Infinity);
	}
	
	public void TempChangeHealPerSecFix(float amount, float time)
	{
		StartCoroutine(TempChangeHealPerSecFixIE(amount, time));
	}
	
	private IEnumerator TempChangeHealPerSecFixIE(float amount, float time)
	{
		ChangeHealPerSecFix(amount);
		yield return new WaitForSeconds(time);
		ChangeHealPerSecFix(-amount);
	}
	
	public void ChangeHealPerSecCurrentPercent(float amount)
	{
		heal_per_sec_current_percent = Mathf.Min(Mathf.Max(0f, heal_per_sec_current_percent + amount), 1.0f);
	}
	
	public void TempChangeHealPerSecCurrentPercent(float amount, float time)
	{
		StartCoroutine(TempChangeHealPerSecCurrentPercentIE(amount, time));
	}
	
	private IEnumerator TempChangeHealPerSecCurrentPercentIE(float percent, float time)
	{
		ChangeHealPerSecCurrentPercent(percent);
		yield return new WaitForSeconds(time);
		ChangeHealPerSecCurrentPercent(-percent);
	}
	
	public void ChangeHealPerSecMaxPercent(float amount)
	{
		heal_per_sec_max_percent = Mathf.Min(Mathf.Max(0, heal_per_sec_max_percent + amount), 1.0f);
	}
	
	public void TempChangeHealPerSecMaxPercent(float amount, float time)
	{
		StartCoroutine(TempChangeHealPerSecMaxPercentIE(amount, time));
	}
	
	private IEnumerator TempChangeHealPerSecMaxPercentIE(float percent, float time)
	{
		ChangeHealPerSecMaxPercent(percent);
		yield return new WaitForSeconds(time);
		ChangeHealPerSecMaxPercent(-percent);
	}
	
	public void ChangeManaPerSecFix(float amount)
	{
		mana_per_sec_change_fix = Mathf.Min(Mathf.Max(Mathf.NegativeInfinity, mana_per_sec_change_fix + amount), Mathf.Infinity);
	}
	
	public void TempChangeManaPerSecFix(float amount, float time)
	{
		StartCoroutine(TempChangeManaPerSecFixIE(amount, time));
	}
	
	private IEnumerator TempChangeManaPerSecFixIE(float amount, float time)
	{
		ChangeManaPerSecFix(amount);
		yield return new WaitForSeconds(time);
		ChangeManaPerSecFix(-amount);
	}
	
	public void ChangeManaPerSecMaxPercent(float amount)
	{
		mana_per_sec_change_max_percent = Mathf.Min(Mathf.Max(0f, mana_per_sec_change_max_percent + amount), 1.0f);
	}
	
	public void TempChangeManaPerSecMaxPercent(float amount, float time)
	{
		StartCoroutine(TempChangeManaPerSecMaxPercentIE(amount, time));
	}
	
	private IEnumerator TempChangeManaPerSecMaxPercentIE(float percent, float time)
	{
		ChangeManaPerSecMaxPercent(percent);
		yield return new WaitForSeconds(time);
		ChangeManaPerSecMaxPercent(-percent);
	}
	
	public void ChangeMagicalDamagePerSecFix(float amount)
	{
		damage_per_sec_magic_fix = Mathf.Min(Mathf.Max(0f, damage_per_sec_magic_fix + amount), Mathf.Infinity);
	}
	
	public void TempChangeMagicalDamagePerSecFix(float amount, float time)
	{
		StartCoroutine(TempChangeMagicalDamagePerSecFixIE(amount, time));
	}
	
	private IEnumerator TempChangeMagicalDamagePerSecFixIE(float amount, float time)
	{
		ChangeMagicalDamagePerSecFix(amount);
		yield return new WaitForSeconds(time);
		ChangeMagicalDamagePerSecFix(-amount);
	}
	
	public void ChangePhysicalDamagePerSecFix(float amount)
	{
		damage_per_sec_phy_fix = Mathf.Min(Mathf.Max(0f, damage_per_sec_phy_fix + amount), Mathf.Infinity);
	}
	
	public void TempChangePhysicalDamagePerSecFix(float amount, float time)
	{
		StartCoroutine(TempChangePhysicalDamagePerSecFixIE(amount, time));
	}
	
	private IEnumerator TempChangePhysicalDamagePerSecFixIE(float amount, float time)
	{
		ChangePhysicalDamagePerSecFix(amount);
		yield return new WaitForSeconds(time);
		ChangePhysicalDamagePerSecFix(-amount);
	}
	
	public void ChangePureDamagePerSecFix(float amount)
	{
		damage_per_sec_pure_fix = Mathf.Min(Mathf.Max(0f, damage_per_sec_pure_fix + amount), Mathf.Infinity);
	}
	
	public void TempChangePureDamagePerSecFix(float amount, float time)
	{
		StartCoroutine(TempChangePureDamagePerSecFixIE(amount, time));
	}
	
	private IEnumerator TempChangePureDamagePerSecFixIE(float amount, float time)
	{
		ChangePureDamagePerSecFix(amount);
		yield return new WaitForSeconds(time);
		ChangePureDamagePerSecFix(-amount);
	}
	
	public void ChangeMagicalDamagePerSecCurrentPercent(float amount)
	{
		damage_per_sec_magic_current_percent = Mathf.Min(Mathf.Max(0f, damage_per_sec_magic_current_percent + amount), 1.0f);
	}
	
	public void TempChangeMagicalDamagePerSecCurrentPercent(float amount, float time)
	{
		StartCoroutine(TempChangeMagicalDamagePerSecCurrentPercentIE(amount, time));
	}
	
	private IEnumerator TempChangeMagicalDamagePerSecCurrentPercentIE(float amount, float time)
	{
		ChangeMagicalDamagePerSecCurrentPercent(amount);
		yield return new WaitForSeconds(time);
		ChangeMagicalDamagePerSecCurrentPercent(-amount);
	}
	
	public void ChangePhysicalDamagePerSecCurrentPercent(float amount)
	{
		damage_per_sec_phy_current_percent = Mathf.Min(Mathf.Max(0f, damage_per_sec_phy_current_percent + amount), 1.0f);
	}
	
	public void TempChangePhysicalDamagePerSecCurrentPercent(float amount, float time)
	{
		StartCoroutine(TempChangePhysicalDamagePerSecCurrentPercentIE(amount, time));
	}
	
	private IEnumerator TempChangePhysicalDamagePerSecCurrentPercentIE(float amount, float time)
	{
		ChangePhysicalDamagePerSecCurrentPercent(amount);
		yield return new WaitForSeconds(time);
		ChangePhysicalDamagePerSecCurrentPercent(-amount);
	}
	
	public void ChangePureDamagePerSecCurrentPercent(float amount)
	{
		damage_per_sec_pure_current_percent = Mathf.Min(Mathf.Max(0f, damage_per_sec_pure_current_percent + amount), 1.0f);
	}
	
	public void TempChangePureDamagePerSecCurrentPercent(float amount, float time)
	{
		StartCoroutine(TempChangePureDamagePerSecCurrentPercentIE(amount, time));
	}
	
	private IEnumerator TempChangePureDamagePerSecCurrentPercentIE(float amount, float time)
	{
		ChangePureDamagePerSecCurrentPercent(amount);
		yield return new WaitForSeconds(time);
		ChangePureDamagePerSecCurrentPercent(-amount);
	}
	
	public void ChangeMagicalDamagePerSecMaxPercent(float amount)
	{
		damage_per_sec_magic_max_percent = Mathf.Min(Mathf.Max(0f, damage_per_sec_magic_max_percent + amount), 1.0f);
	}
	
	public void TempChangeMagicalDamagePerSecMaxPercent(float amount, float time)
	{
		StartCoroutine(TempChangeMagicalDamagePerSecMaxPercentIE(amount, time));
	}
	
	private IEnumerator TempChangeMagicalDamagePerSecMaxPercentIE(float amount, float time)
	{
		ChangeMagicalDamagePerSecMaxPercent(amount);
		yield return new WaitForSeconds(time);
		ChangeMagicalDamagePerSecMaxPercent(-amount);
	}
	
	public void ChangePhysicalDamagePerSecMaxPercent(float amount)
	{
		damage_per_sec_phy_max_percent = Mathf.Min(Mathf.Max(0f, damage_per_sec_phy_max_percent + amount), 1.0f);
	}
	
	public void TempChangePhysicalDamagePerSecMaxPercent(float amount, float time)
	{
		StartCoroutine(TempChangePhysicalDamagePerSecMaxPercentIE(amount, time));
	}
	
	private IEnumerator TempChangePhysicalDamagePerSecMaxPercentIE(float amount, float time)
	{
		ChangePhysicalDamagePerSecMaxPercent(amount);
		yield return new WaitForSeconds(time);
		ChangePhysicalDamagePerSecMaxPercent(-amount);
	}
	
	public void ChangePureDamagePerSecMaxPercent(float amount)
	{
		damage_per_sec_pure_max_percent = Mathf.Min(Mathf.Max(0f, damage_per_sec_phy_max_percent + amount), 1.0f);
	}
	
	public void TempChangePureDamagePerSecMaxPercent(float amount, float time)
	{
		StartCoroutine(TempChangePureDamagePerSecMaxPercentIE(amount, time));
	}
	
	private IEnumerator TempChangePureDamagePerSecMaxPercentIE(float amount, float time)
	{
		ChangePureDamagePerSecMaxPercent(amount);
		yield return new WaitForSeconds(time);
		ChangePureDamagePerSecMaxPercent(-amount);
	}
	
	
	//-----------------------------------
	// crowd control
	//-----------------------------------
	
	public void SetInvulnerable(float time)
	{
		StartCoroutine(SetInvulnerableIE(time));
	}
	
	private float invulnerable_time = 0;
	
	private IEnumerator SetInvulnerableIE(float time)
	{
		if(invulnerable)
		{
			invulnerable_time = Mathf.Max(time, invulnerable_time);
		}
		else
		{
			invulnerable_time = time;
			invulnerable = true;
			while(invulnerable_time > 0f)
			{
				yield return new WaitForSeconds(Time.deltaTime);
				invulnerable_time -= Time.deltaTime;
			}
			invulnerable = false;
		}
	}
	
	public void Kill()
	{
		CancelInvoke("Regen");
		CancelInvoke("HealPerSec");
		CancelInvoke("ManaChangePerSec");
		CancelInvoke("DamagePerSec");
		animator.SetBool("relieve", false);
		AudioManager.PlaySound(dead_sound, transform.position);
		is_dead = true;
		current_health = 0;
		FinishCast();
		FinishAttack();
		animator.SetBool("dead", true);
	}
	
	public void ApplySlowMove(float percent, float time)
	{
		if(percent > 0.05f)
		{
			text_generator.ShowStatus("Slowed");
		}
		StartCoroutine(ApplySlowMoveIE(percent, time));
	}
	
	private IEnumerator ApplySlowMoveIE(float percent, float time)
	{
		percent *= (1 - tenacity); 
		float move_amount = move_speed * percent;
		float dodge_amount = dodge_speed * percent;
		move_speed -= move_amount; 
		dodge_speed -= dodge_amount;
		yield return new WaitForSeconds(time);
		move_speed += move_amount;
		dodge_speed += dodge_amount;
	}
	
	public void ApplySlowAttack(float percent, float time)
	{
		StartCoroutine(ApplySlowAttackIE(percent, time));
	}
	
	private IEnumerator ApplySlowAttackIE(float percent, float time)
	{
		percent *= (1 - tenacity);
		float amount = attack_speed * percent;
		attack_speed -= amount;
		yield return new WaitForSeconds(time);
		attack_speed += amount;
	}
	
	public void ApplyStun(float time)
	{
		FinishCast();
		FinishAttack();
		text_generator.ShowStatus("Stunned");
		StartCoroutine(ApplyStunIE(time));
	}
	
	private float stun_time = 0;
	
	private IEnumerator ApplyStunIE(float time)
	{
		time *= (1 - tenacity);
		
		if(is_stun)
		{
			stun_time = Mathf.Max(time, stun_time);
		}
		else
		{
			stun_time = time;
			is_stun = true;
			animator.SetBool("finishStun", false);
			animator.SetBool("stun", true);
			while(stun_time > 0f)
			{
				yield return new WaitForSeconds(Time.deltaTime);
				stun_time -= Time.deltaTime;
			}
			is_stun = false;
			animator.SetBool("stun", false);
			animator.SetBool("finishStun", true);
		}
	}
	
	public void ApplyRooted(float time)
	{
		text_generator.ShowStatus("Rooted");
		StartCoroutine(ApplyRootedIE(time));
	}
	
	private float root_time = 0f;
	
	private IEnumerator ApplyRootedIE(float time)
	{
		time *= (1 - tenacity);
		
		if(is_rooted)
		{
			root_time = Mathf.Max(time, root_time);
		}
		else
		{
			root_time = time;
			is_rooted = true;
			while(root_time > 0f)
			{
				yield return new WaitForSeconds(Time.deltaTime);
				root_time -= Time.deltaTime;
			}
			is_rooted = false;
		}
		
	}
	
	public void ApplySlience(float time)
	{
		FinishCast();
		FinishAttack();
		text_generator.ShowStatus("Slienced");
		StartCoroutine(ApplySlienceIE(time));
	}
	
	private float slience_time = 0f;
	
	private IEnumerator ApplySlienceIE(float time)
	{
		time *= (1 - tenacity);
		
		if(is_slienced)
		{
			slience_time = Mathf.Max(time, slience_time);
		}
		else
		{
			slience_time = time;
			is_slienced = true;
			while(slience_time > 0f)
			{
				yield return new WaitForSeconds(Time.deltaTime);
				slience_time -= Time.deltaTime;
			}
			is_slienced = false;
		}
	}
	
	public void ApplyKnock(float amount, float time)
	{
		FinishCast();
		FinishAttack();
		if(!is_knocked) //cannot apply knock while character is getting knocked
		{
			is_knocked = true;
			animator.SetBool("finishKnocked", false);
			animator.SetBool("knocked", true);
			animator.SetBool("knocked", false);
			controller.Move(new Vector3(0, 0, -amount));
		}
	}
	
	//these functions are called by the end of the animation or in the midlle when the action get interrupted
	
	public void FinishKnock()
	{
		animator.SetBool("knocked", false);
		animator.SetBool("finishKnocked", true);
		is_knocked = false;
	}
	
	public void FinishCast()
	{
		is_move_casting = false;
		is_stand_casting = false;
		animator.SetBool("finishCast", true);
		animator.SetBool("generalSpell", false);
		animator.SetBool("buff", false);
		animator.SetBool("shakeGround", false);
		animator.SetBool("channel", false);
		animator.SetBool("mChannel", false);
		animator.SetBool("shoot", false);
		animator.SetBool("mShoot", false);
		animator.SetLayerWeight(1, 0f);
		animator.SetLayerWeight(2, 0f);
	}
	
	public void FinishAttack()
	{
		is_move_casting = false;
		is_stand_casting = false;
		animator.SetBool("basicAttack", false);
		animator.SetBool("mBasicAttack", false);
		animator.SetBool("finishAttack", true);
		
		animator.SetLayerWeight(1, 0f);
		animator.SetLayerWeight(2, 0f);
	}
	
	public void ChangeMoveSpeed(float percent)
	{
		float move_amount = move_speed * percent;
		float dodge_amount = dodge_speed * percent;
		move_speed += move_amount; 
		dodge_speed += dodge_amount;
	}
	
	public void TempChangeMoveSpeed(float percent, float time)
	{
		StartCoroutine(TempChangeMoveSpeedIE(percent, time));
	}
	
	private IEnumerator TempChangeMoveSpeedIE(float percent, float time)
	{
		ChangeMoveSpeed(percent);
		yield return new WaitForSeconds(time);
		ChangeMoveSpeed(-percent);
	}
	
	public void ChangeMaxHealth(float amount)
	{
		max_health = Mathf.Min(Mathf.Max(0f, max_health + amount), Mathf.Infinity);
		current_health = Mathf.Min(Mathf.Max(0f, current_health + amount), max_health);
	}
			
	public void TempChangeMaxHealth(float amount, float time)
	{
		StartCoroutine(TempChangeMaxHealthIE(amount, time));
	}

	private IEnumerator TempChangeMaxHealthIE(float amount, float time)
	{
		ChangeMaxHealth(amount);
		current_health += amount;
		yield return new WaitForSeconds(time);
		ChangeMaxHealth(-amount);
	}
	
	public void ChangeMaxMana(float amount)
	{
		max_mana = Mathf.Min(Mathf.Max(0f, max_mana + amount), Mathf.Infinity);
		current_mana = Mathf.Min(Mathf.Max(0f, current_mana + amount), max_mana);
	}
	
	public void TempChangeMaxMana(float amount, float time)
	{
		StartCoroutine(TempChangeMaxManaIE(amount, time));
	}
	
	private IEnumerator TempChangeMaxManaIE(float amount, float time)
	{
		ChangeMaxMana(amount);
		current_mana += amount;
		yield return new WaitForSeconds(time);
		ChangeMaxMana(-amount);
	}
	
	public void TempChangeMaxHealthFix(float amount, float time)
	{
		TempChangeMaxHealth(amount, time);
	}
	
	public void TempChangeMaxManaFix(float amount, float time)
	{
		TempChangeMaxMana(amount, time);
	}
	
	public void TempChangeMaxHealthPercent(float percent, float time)
	{
		TempChangeMaxHealth(max_health * percent, time);
	}
	
	public void TempChangeMaxManaPercent(float percent, float time)
	{
		TempChangeMaxMana(max_mana * percent, time);
	}
	
	public void ChangeAttackDamage(float amount)
	{
		attack_damage = Mathf.Min(Mathf.Max(0f, attack_damage + amount), Mathf.Infinity);
	}
	
	public void TempChangeAttackDamageFix(float amount, float time)
	{
		StartCoroutine(TempChangeAttackDamageFixIE(amount, time));
	}
	
	private IEnumerator TempChangeAttackDamageFixIE(float amount, float time)
	{
		ChangeAttackDamage(amount);
		yield return new WaitForSeconds(time);
		ChangeAttackDamage(-amount);
	}
	
	public void TempChangeAttackDamagePercent(float percent, float time)
	{
		StartCoroutine(TempChangeAttackDamagePercentIE(percent, time));
	}
	
	private IEnumerator TempChangeAttackDamagePercentIE(float percent, float time)
	{
		float amount = attack_damage * percent;
		ChangeAttackDamage(amount);
		yield return new WaitForSeconds(time);
		ChangeAttackDamage(-amount);
	}
	
	public void ChangeSpellPower(float amount)
	{
		spell_power = Mathf.Min(Mathf.Max(0f, spell_power + amount), Mathf.Infinity);
	}
	
	public void TempChangeSpellPower(float amount, float time)
	{
		StartCoroutine(TempChangeSpellPowerIE(amount, time));
	}
	
	private IEnumerator TempChangeSpellPowerIE(float amount, float time)
	{
		ChangeSpellPower(amount);
		yield return new WaitForSeconds(time);
		ChangeSpellPower(-amount);
	}
	
	public void ChangeAttackSpeed(float percent)
	{
		float amount = attack_speed * percent;
		attack_speed = Mathf.Min(Mathf.Max(0f, attack_speed + amount), Mathf.Infinity);
	}
	
	public void TempChangeAttackSpeed(float percent, float time)
	{
		StartCoroutine(TempChangeAttackSpeedIE(percent, time));
	}
	
	private IEnumerator TempChangeAttackSpeedIE(float percent, float time)
	{
		ChangeAttackSpeed(attack_speed * percent);
		yield return new WaitForSeconds(time);
		ChangeAttackSpeed(-attack_speed * percent);
	}
	
	public void ChangeLifeSteal(float amount)
	{
		life_steal = Mathf.Min(Mathf.Max(0f, life_steal + amount), Mathf.Infinity); 
	}
	
	public void TempChangeLifeSteal(float amount, float time)
	{
		StartCoroutine(TempChangeLifeStealIE(amount, time));
	}
	
	private IEnumerator TempChangeLifeStealIE(float amount, float time)
	{ 	
		ChangeLifeSteal(amount);
		yield return new WaitForSeconds(time);
		ChangeLifeSteal(-amount);
	}
	
	public void ChangeSpellSteal(float amount)
	{
		spell_steal = Mathf.Min(Mathf.Max(0f, spell_steal + amount), Mathf.Infinity);  
	}
	
	public void TempChangeSpellSteal(float amount, float time)
	{
		StartCoroutine(TempChangeSpellStealIE(amount, time));
	}
	
	private IEnumerator TempChangeSpellStealIE(float amount, float time)
	{ 	
		ChangeSpellSteal(amount);
		yield return new WaitForSeconds(time);
		ChangeSpellSteal(-amount);
	}
	
	public void ChangeManaSteal(float amount)
	{
		mana_steal = Mathf.Min(Mathf.Max(0f, mana_steal + amount), Mathf.Infinity); 
	}
	
	public void TempChangeManaSteal(float amount, float time)
	{
		StartCoroutine(TempChangeManaStealIE(amount, time));
	}
	
	private IEnumerator TempChangeManaStealIE(float amount, float time)
	{ 	
		ChangeManaSteal(amount);
		yield return new WaitForSeconds(time);
		ChangeManaSteal(-amount);
	}
	
	public void ChangeDodgeChance(float amount)
	{
		dodge_chance = Mathf.Min(Mathf.Max(0f, dodge_chance + amount), 1.0f); 
	}
	
	public void TempChangeDodgeChance(float amount, float time)
	{
		StartCoroutine(TempChangeDodgeChanceIE(amount, time));
	}
	
	private IEnumerator TempChangeDodgeChanceIE(float amount, float time)
	{ 	
		ChangeDodgeChance(amount);
		yield return new WaitForSeconds(time);
		ChangeDodgeChance(-amount);
	}
	
	public void ChangeCritChance(float amount)
	{
		crit_chance = Mathf.Min(Mathf.Max(0f, crit_chance + amount), 1.0f); 
	}
	
	public void TempChangeCritChance(float amount, float time)
	{
		StartCoroutine(TempChangeCritChanceIE(amount, time));
	}
	
	private IEnumerator TempChangeCritChanceIE(float amount, float time)
	{ 	
		ChangeCritChance(amount);
		yield return new WaitForSeconds(time);
		ChangeCritChance(-amount);
	}
	
	public void ChangeCritDamage(float percent)
	{
		crit_damage = Mathf.Min(Mathf.Max(1.0f, crit_damage + percent), Mathf.Infinity); 
	}
	
	public void TempChangeCritDamage(float percent, float time)
	{
		StartCoroutine(TempChangeCritDamageIE(percent, time));
	}
	
	private IEnumerator TempChangeCritDamageIE(float percent, float time)
	{
		ChangeCritDamage(percent);
		yield return new WaitForSeconds(time);
		ChangeCritDamage(-percent);
	}
	
	public void ChangeHealthRegen(float amount)
	{
		health_regen = Mathf.Min(Mathf.Max(0f, health_regen + amount), Mathf.Infinity);
	}
	
	public void TempChangeHealthRegen(float amount, float time)
	{
		StartCoroutine(TempChangeHealthRegenIE(amount, time));
	}
	
	private IEnumerator TempChangeHealthRegenIE(float amount, float time)
	{
		ChangeHealthRegen(amount);
		yield return new WaitForSeconds(time);
		ChangeHealthRegen(-amount);
	}
	
	public void ChangeManaRegen(float amount)
	{
		mana_regen = Mathf.Min(Mathf.Max(0f, mana_regen + amount), Mathf.Infinity);
	}
	
	public void TempChangeManaRegen(float amount, float time)
	{
		StartCoroutine(TempChangeManaRegenIE(amount, time));
	}
	
	private IEnumerator TempChangeManaRegenIE(float amount, float time)
	{
		ChangeManaRegen(amount);
		yield return new WaitForSeconds(time);
		ChangeManaRegen(-amount);
	}
	
	public void ChangeCDR(float amount)
	{
		cdr = Mathf.Min(Mathf.Max(0f, cdr + amount), 1.0f);
	}
	
	public void TempChangeCDR(float amount, float time)
	{
		StartCoroutine(TempChangeCDRIE(amount, time));
	}
	
	private IEnumerator TempChangeCDRIE(float amount, float time)
	{
		ChangeCDR(amount);
		yield return new WaitForSeconds(time);
		ChangeCDR(-amount);
	}
	
	public void ChangeStunChance(float amount)
	{
		stun_chance = Mathf.Min(Mathf.Max(0f, stun_chance + amount), 1.0f);
	}
	
	public void TempChangeStunChance(float amount, float time)
	{
		StartCoroutine(TempChangeStunChanceIE(amount, time));
	}
	
	private IEnumerator TempChangeStunChanceIE(float amount, float time)
	{
		ChangeStunChance(amount);
		yield return new WaitForSeconds(time);
		ChangeStunChance(-amount);
	}
	
	public void ChangeStunDur(float amount)
	{
		stun_dur = (amount > stun_dur) ? amount : stun_dur; //get the larger stun duration
		stun_dur = Mathf.Min(Mathf.Max(0f, stun_dur), Mathf.Infinity);
	}
	
	public void TempChangeStunDur(float amount, float time)
	{
		StartCoroutine(TempChangeStunDurIE(amount, time));
	}
	
	private IEnumerator TempChangeStunDurIE(float amount, float time)
	{
		float temp = stun_dur; //store the old value to restore later
		ChangeStunDur(amount);
		yield return new WaitForSeconds(time);
		ChangeStunDur(temp);
	}
	
	public void ChangePosionDamage(float amount)
	{
		posion_damage = Mathf.Min(Mathf.Max(0f, posion_damage + amount), Mathf.Infinity);
	}
	
	public void TempChangePosionDamage(float amount, float time)
	{
		StartCoroutine(TempChangePosionDamageIE(amount, time));
	}
	
	private IEnumerator TempChangePosionDamageIE(float amount, float time)
	{
		ChangePosionDamage(amount);
		yield return new WaitForSeconds(time);
		ChangePosionDamage(-amount);
	}
	
	public void ChangePosionAttackSlow(float amount)
	{
		posion_attack_slow = Mathf.Min(Mathf.Max(0f, posion_attack_slow + amount), 1.0f);
	}
	
	public void TempChangePosionAttackSlow(float amount, float time)
	{
		StartCoroutine(TempChangePosionAttackSlowIE(amount, time));
	}
	
	private IEnumerator TempChangePosionAttackSlowIE(float amount, float time)
	{
		ChangePosionAttackSlow(amount);
		yield return new WaitForSeconds(time);
		ChangePosionAttackSlow(-amount);
	}
	
	public void ChangePosionMoveSlow(float amount)
	{
		posion_move_slow = Mathf.Min(Mathf.Max(0f, posion_move_slow + amount), 1.0f);
	}
	
	public void TempChangePosionMoveSlow(float amount, float time)
	{
		StartCoroutine(TempChangePosionMoveSlowIE(amount, time));
	}
	
	private IEnumerator TempChangePosionMoveSlowIE(float amount, float time)
	{
		ChangePosionMoveSlow(amount);
		yield return new WaitForSeconds(time);
		ChangePosionMoveSlow(-amount);
	}
	
	public void ChangePosionDur(float amount)
	{
		posion_dur = Mathf.Min(Mathf.Max(0f, posion_dur + amount), Mathf.Infinity);
	}
	
	public void TempChangePosionDur(float amount, float time)
	{
		StartCoroutine(TempChangePosionDurIE(amount, time));
	}
	
	private IEnumerator TempChangePosionDurIE(float amount, float time)
	{
		ChangePosionDur(amount);
		yield return new WaitForSeconds(time);
		ChangePosionDur(-amount);
	}
	
	public void ChangePhysicalResist(float amount)
	{
		physics_resist = Mathf.Min(Mathf.Max(0f, physics_resist + amount), 1.0f);
	}
	
	public void TempChangePhysicalResist(float amount, float time)
	{
		StartCoroutine(TempChangePhysicalResistIE(amount, time));
	}
	
	private IEnumerator TempChangePhysicalResistIE(float amount, float time)
	{
		ChangePhysicalResist(amount);
		yield return new WaitForSeconds(time);
		ChangePhysicalResist(-amount);
	}
	
	public void ChangeMagicalResist(float amount)
	{
		magic_resist = Mathf.Min(Mathf.Max(0f, magic_resist + amount), 1.0f);
	}
	
	public void TempChangeMagicalResist(float amount, float time)
	{
		StartCoroutine(TempChangeMagicalResistIE(amount, time));
	}
	
	private IEnumerator TempChangeMagicalResistIE(float amount, float time)
	{
		ChangeMagicalResist(amount);
		yield return new WaitForSeconds(time);
		ChangeMagicalResist(-amount);
	}
	
	public void ChangeTenacity(float amount)
	{
		tenacity = Mathf.Min(Mathf.Max(0f, tenacity + amount), 1.0f);
	}
	
	public void TempChangeTenacity(float amount, float time)
	{
		StartCoroutine(TempChangeTenacityIE(amount, time));
	}
	
	private IEnumerator TempChangeTenacityIE(float amount, float time)
	{
		ChangeTenacity(amount);
		yield return new WaitForSeconds(time);
		ChangeTenacity(-amount);
	}
	
	public void ChangeBlockDamage(float amount)
	{
		block_damage = Mathf.Min(Mathf.Max(0f, block_damage + amount), Mathf.Infinity);
	}
	
	public void TempChangeBlockDamage(float amount, float time)
	{
		StartCoroutine(TempChangeBlockDamageIE(amount, time));
	}
	
	private IEnumerator TempChangeBlockDamageIE(float amount, float time)
	{
		ChangeBlockDamage(amount);
		yield return new WaitForSeconds(time);
		ChangeBlockDamage(-amount);
	}
	
	
	
	//this function is called whenever the character is healed
	private void ApplyHeal(float amount)
	{
		if(!is_dead)
		{	
			text_generator.ShowHeal(amount);
			current_health = Mathf.Min(Mathf.Max(0, current_health + (amount * heal_emp)), max_health);
		}
	}
	
	public void HealFix(float amount)
	{
		ApplyHeal(amount);
	}
	
	public void HealMaxPercent(float percent)
	{
		ApplyHeal(max_health * percent);
		
	}
	
	public void HealCurrentPercent(float percent)
	{
		ApplyHeal(current_health * percent);
	}
	
	//this function is called whenever the character is taken damage
	private void ApplyDamage(float amount)
	{
		if(!is_dead && !invulnerable && amount >= 0f)
		{
			current_health = Mathf.Min(Mathf.Max(0f, current_health - (amount * damage_emp)), max_health);
			if(current_health <= 0.0f)
			{
				Kill();	
			}
		}
	}
	
	//this function is called whenever the character take physical damage
	private void ApplyPhysicalDamage(float amount, StatusManager enemy_sm)
	{
		amount *= (1 - physics_resist);
		if(enemy_sm != null)
		{
			enemy_sm.HealFix(amount * enemy_sm.life_steal); //lifesteal
			enemy_sm.ChangeManaFix(amount * enemy_sm.mana_steal); //manasteal
		}
		text_generator.ShowPhysicalDamage(amount);
		ApplyDamage(amount);
	}
	
	//Instance of physical damage can be blocked and trigger passive skill
	
	public void ApplyInstancePhysicalAttackDamageFix(float amount, StatusManager enemy_sm)
	{
		foreach(EffectApplier s in get_physical_dam_passives.Keys)
		{
			for(int i = get_physical_dam_passives[s]; i > 0; ++i)
			{
				s.ApplyEffectEnemy(enemy_sm);
			}
		}
		if(target == null)
		{
			target = enemy_sm.gameObject;
		}
		amount -= block_damage;
		ApplyPhysicalDamage(amount, enemy_sm);
	}
	
	public void ApplyInstancePhysicalAttackDamageCurrentPercent(float percent, StatusManager enemy_sm)
	{
		ApplyInstancePhysicalAttackDamageFix(percent * current_health, enemy_sm);
	}
	
	public void ApplyInstanceAttackDamageMaxPercent(float percent, StatusManager enemy_sm)
	{
		ApplyInstancePhysicalAttackDamageFix(percent * max_health, enemy_sm);
	}
	
	private void ApplyPhysicalDamageFix(float amount, StatusManager enemy_sm)
	{
		ApplyPhysicalDamage(amount, enemy_sm);
	}
	
	private void ApplyPhysicalDamageMaxPercent(float percent, StatusManager enemy_sm)
	{
		ApplyPhysicalDamage(max_health * percent, enemy_sm);
	}
	
	private void ApplyPhysicalDamageCurrentPercent(float percent, StatusManager enemy_sm)
	{
		ApplyPhysicalDamage(current_health * percent, enemy_sm);
	}
	
	//this function is called whenever the character take magical damage
	private void ApplyMagicalDamage(float amount, StatusManager enemy_sm)
	{
		amount *= (1 - magic_resist);
		if(enemy_sm != null)
		{
			enemy_sm.HealFix(amount * enemy_sm.spell_steal); //spellsteal
		}
		text_generator.ShowMagicDamage(amount);
		ApplyDamage(amount);
	}
	
	//Instance magical damage can be blocked by spell sheield and trigger passive skill
	
	public void ApplyInstanceMagicalDamageFix(float amount, StatusManager enemy_sm)
	{
		foreach(EffectApplier s in get_magical_dam_passives.Keys)
		{
			for(int i = get_magical_dam_passives[s]; i > 0; ++i)
			{
				s.ApplyEffectEnemy(enemy_sm);
			}
		}
		if(target == null)
		{
			target = enemy_sm.gameObject;
		}
		ApplyMagicalDamageFix(amount, enemy_sm);
	}
	
	public void ApplyInstanceMagicalDamageCurrentPercent(float percent, StatusManager enemy_sm)
	{
		ApplyInstanceMagicalDamageFix(percent * current_health, enemy_sm);
	}
	
	public void ApplyInstanceMagicalDamageMaxPercent(float percent, StatusManager enemy_sm)
	{
		ApplyInstanceMagicalDamageFix(percent * max_health, enemy_sm);
	}
	
	private void ApplyMagicalDamageFix(float amount, StatusManager enemy_sm)
	{
		ApplyMagicalDamage(amount, enemy_sm);
	}
	
	private void ApplyMagicalDamageMaxPercent(float percent, StatusManager enemy_sm)
	{
		ApplyMagicalDamage(max_health * percent, enemy_sm);
	}
	
	private void ApplyMagicalDamageCurrentPercent(float percent, StatusManager enemy_sm)
	{
		ApplyMagicalDamage(current_health * percent, enemy_sm);
	}
	
	//this function is called whenever the character take pure damage
	private void ApplyPureDamage(float amount)
	{
		text_generator.ShowPureDamage(amount);
		ApplyDamage(amount);
	}
	
	public void ApplyInstancePureDamageFix(float amount, StatusManager enemy_sm)
	{
		if(target == null)
		{
			target = enemy_sm.gameObject;
		}
		ApplyPureDamage(amount);
	}
	
	public void ApplyInstancePureDamageMaxPercent(float percent, StatusManager enemy_sm)
	{
		ApplyInstancePureDamageFix(percent * max_health, enemy_sm);
	}
	
	public void ApplyInstancePureDamageCurrentPercent(float percent, StatusManager enemy_sm)
	{
		ApplyInstancePureDamageFix(percent * current_health, enemy_sm);
	}
	
	public void ApplyPureDamageFix(float amount)
	{
		ApplyPureDamage(amount);
	}
	
	public void ApplyPureDamageMaxPercent(float percent)
	{
		ApplyPureDamage(max_health * percent);
	}
	
	public void ApplyPureDamageCurrentPercent(float percent)
	{
		ApplyPureDamage(current_health * percent);
	}
	
	private void ChangeMana(float amount)
	{
		current_mana = Mathf.Min(Mathf.Max(0f, current_mana + amount), max_mana);
	}
	
	public void ChangeManaFix(float amount)
	{
		ChangeMana(amount);
	}
	
	public void ChangeManaMaxPercent(float percent)
	{
		ChangeMana(max_mana * percent);
	}
	
	public void ChangeManaCurrentPercent(float percent)
	{
		ChangeMana(current_mana * percent);
	}
	
	public void ChangeManaMaxPercentPerSec(float percent, int time)
	{
		StartCoroutine(ChangeManaMaxPercentPerSecIE(percent, time));
	}
	
	private IEnumerator ChangeManaMaxPercentPerSecIE(float percent, int time)
	{
		while(time-- >= 0)
		{
			ChangeMaxMana(percent);
			yield return new WaitForSeconds(1.0f);
		}
	}
	
	public void SetSpellBlock()
	{
		spell_block = true;
	}
	
	public void UseSpellBlock()
	{
		text_generator.ShowStatus("Spell Blocked");
		spell_block = false;
	}
	
	public void Relieve()
	{
		target = null;
		is_dead = false;
		animator.SetBool("dead", false);
		HealFix(max_health);
		ChangeManaFix(max_mana);
		SetInvulnerable(5.0f);
		animator.SetBool("relieve", true);
		InvokeRepeating("Regen", 2.0f, 1.0f);
		InvokeRepeating("HealPerSec", 2.0f, 1.0f);
		InvokeRepeating("ManaChangePerSec", 2.0f, 1.0f);
		InvokeRepeating("DamagePerSec", 2.0f, 1.0f);
	}
	
	public void PlayWalkSound()
	{
		if(walk_sounds.GetLength(0) != 0)
		{
			AudioManager.PlaySound(walk_sounds[UnityEngine.Random.Range(0, walk_sounds.GetLength(0))], transform.position);	
		}
	}
}
