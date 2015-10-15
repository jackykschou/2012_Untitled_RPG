using UnityEngine;
using System.Collections;
using System; //Copy

public class SkillPool : MonoBehaviour{

	public DragDropSkill[] skill_pool = new DragDropSkill[30];
	
	public void UnlockSkill(int index)
	{
		skill_pool[index].UnlockSkill();
	}
	
	public void UnlockSkillSlience(int index)
	{
		skill_pool[index].UnlockSkillSlience();
	}
	
}
