//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2012 Tasharen Entertainment (edited by Ka Seng Chou)
//----------------------------------------------

using UnityEngine;

/// <summary>
/// Simple example of an OnDrop event accepting a game object. In this case we check to see if there is a DragDropObject present,
/// and if so -- create its prefab on the surface, then destroy the object.
/// </summary>

[AddComponentMenu("NGUI/Examples/Drag and Drop Surface")]
public class SkillSlot : MonoBehaviour
{
	public bool rotatePlacedObject = false;
	public Skill current_skill;
	public int skill_slot_id;
	public UITexture icon;
	
	public AudioClip drop_sound;
	public AudioClip drop_deny_sound;

	void OnDrop (GameObject go)
	{
		DragDropSkill slot = go.GetComponent<DragDropSkill>();
		if (slot != null && slot.unlocked && slot.skill != null)
		{
			Skill skill = slot.skill;
			if(SkillManager.ChangeSkill(skill, skill_slot_id))
			{
				current_skill = skill;
				AudioManager.PlaySound(drop_sound, transform.position);
			}
			else
			{
				AudioManager.PlaySound(drop_deny_sound, transform.position);
			}
		}
	}
	
	void OnTooltip (bool show)
    {
		if(show && current_skill != null)
		{
			UITooltip.ShowText(current_skill.GetDescription());
		}
		else
		{
			UITooltip.ShowText(null);
		}
    }
	
}