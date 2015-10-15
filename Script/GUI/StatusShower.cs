using UnityEngine;
using System.Collections;

public class StatusShower : MonoBehaviour {

	public PlayerStatusManager status_manager;
	public UILabel label;
	
	void Update()
	{
		label.text = "level: " + status_manager.level + "\n"
			+ "health: " + status_manager.current_health + "/" + status_manager.max_health + "\n"
			+ "mana: " + status_manager.current_mana + "/" + status_manager.max_mana + "\n"
			+ "health regen: " + status_manager.health_regen + "\n"
			+ "mana regen: " + status_manager.mana_regen + "\n"
			+ "movement speed: " + status_manager.move_speed + "\n"
			+ "attack damage: " + status_manager.attack_damage + "\n"
			+ "attack speed: " + status_manager.attack_speed + "\n"
			+ "spell power: " + status_manager.spell_power * 100 + "%\n"
			+ "life steal: " + status_manager.life_steal * 100 + "%\n"
			+ "spell steal: " + status_manager.spell_steal * 100 + "%\n"
			+ "mana steal: " + status_manager.mana_steal * 100 + "%\n"
			+ "spell steal: " + status_manager.mana_steal * 100 + "%\n"
			+ "dodge chance: " + status_manager.dodge_chance * 100 + "%\n"
			+ "critical chance: " + status_manager.crit_chance * 100 + "%\n"
			+ "critical damage: " + status_manager.crit_damage * 100 + "%\n"
			+ "stun chance: " + status_manager.stun_chance * 100 + "%\n"
			+ "CDR: " + status_manager.cdr * 100 + "%\n"
			+ "damage block: " + status_manager.block_damage  + "\n"
			+ "physical resist: " + status_manager.physics_resist + "%\n"
			+ "magical resist: " + status_manager.magic_resist + "%\n"
			+ "tenacity: " + status_manager.tenacity * 100 + "%\n";
	}
}
