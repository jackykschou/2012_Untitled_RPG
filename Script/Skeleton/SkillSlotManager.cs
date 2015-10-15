using UnityEngine;
using System.Collections;

public class SkillSlotManager : MonoBehaviour {

	public SkillSlot[] slots = new SkillSlot[6];
	
	private static GameObject manager;
	
	void Start()
	{
		manager = GameObject.Find("Menues");
	}
	
	public static void AssignSkill(int index, Skill skill)
	{
		skill_to_assign = skill;
		manager.SendMessage("AssignSkillHelper", index);
	}
	
	private static Skill skill_to_assign;
	
	public void AssignSkillHelper(int index)
	{
		slots[index].icon.enabled = true;
		slots[index].current_skill = skill_to_assign;
		slots[index].icon.mainTexture = skill_to_assign.icon;
	}
	
	public static void EmptySkill(int index)
	{
		manager.SendMessage("EmptySkillHelper", index);
	}
	
	public void EmptySkillHelper(int index)
	{
		slots[index].current_skill = null;
		slots[index].icon.mainTexture = null;
		slots[index].icon.enabled = false;
	}
}
