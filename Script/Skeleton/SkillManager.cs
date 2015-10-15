	using UnityEngine;
using System.Collections;
using System;

/**
 * this script is used to manage the skills owned by an object
 **/
public class SkillManager : MonoBehaviour {
	
	public static PlayerStatusManager status_manager; //access the status of the character
	public static Skill[] skills = new Skill[6] ;
	public static Skill skill_to_cast; //the skill that is going to be casted
	
	//assign a new skill to the manager (put into the list) return whether the change is successful
	public static bool ChangeSkill(Skill skill, int index)
	{
		if((skill.attack_type != status_manager.weapon_type && skill.attack_type != Skill.AttackType.anytype) || skill.level_req > status_manager.level)
		{
			return false;	
		}
		
		//if skill is already there
		for(int i = 0; i < 6; ++i)
		{
			if(skills[i] != null && skills[i].id == skill.id)
			{
				skills[i].Remove();
				skills[i] = null;
				StatusGUIManager.EmptySkillIcon(i);
				SkillSlotManager.EmptySkill(i);
			}
		}
		
		if(skills[index] != null)
		{
			skills[index].Remove();
		}
		
		StatusGUIManager.ChangeSkillIcon(index, skill.icon);
		SkillSlotManager.AssignSkill(index, skill);
		skill.ownership = Skill.Ownership.player;
		skill.SetUp(status_manager);
		skills[index] = skill;
		return true;
	}
	
	public static void InputActiviateSkill(int index)
	{	
		if(skills[index] != null && !skills[index].passive)//cannot activate a passive skill by input
		{
			skill_to_cast = skills[index];
			skill_to_cast.Activiate();
		}
	}
	
	//start the actual effect of the action (usually called in the middle of the skill casting animation)
	public static void CastSkill()
	{
		skill_to_cast.StartEffect();	
	}
}
