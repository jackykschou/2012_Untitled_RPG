//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2012 Tasharen Entertainment (edited by Ka Seng Chou)
//----------------------------------------------

using UnityEngine;

public class DragDropSkill: MonoBehaviour
{
	/// <summary>
	/// Prefab object that will be instantiated on the DragDropSurface if it receives the OnDrop event.
	/// </summary>

	public Skill skill;
	public UITexture icon;
	
	public AudioClip unlock_normal_sound;
	public AudioClip unlock_rare_sound;
	public AudioClip unlock_legendary_sound;
	public AudioClip already_unlock_sound;
	public AudioClip drag_sound;
	
	public bool unlocked = false;

	Transform mTrans;
	bool mIsDragging = false;
	bool mSticky = false;
	Transform mParent;
	
	public void UnlockSkill()
	{
		if(!unlocked)
		{
			MetaDataManager.skills_learnt.Add(skill.id);
			if(skill.quality == Skill.Quality.normal)
			{
				AudioManager.PlaySound(unlock_normal_sound, transform.position);
				Messenger.DisplayBigMessage("Skill Unlocked:\n" + skill.name);
			}
			else if(skill.quality == Skill.Quality.rare)
			{
				AudioManager.PlaySound(unlock_rare_sound, transform.position);
				Messenger.DisplayBigMessage("Rare Skill Unlocked!:\n" + skill.name);
			}
			else
			{
				AudioManager.PlaySound(unlock_legendary_sound, transform.position);
				Messenger.DisplayBigMessage("Legendary Skill Unlocked!:\n" + skill.name);
			}
			unlocked = true;
			icon.mainTexture = skill.icon;
			MetaDataManager.AddLearntSkill(skill.id);
		}
		else
		{
			AudioManager.PlaySound(already_unlock_sound, transform.position);
			Messenger.DisplaySmallMessage("Skill already learnt\n");
		}
	}
	
	public void UnlockSkillSlience()
	{
		if(!unlocked)
		{
			unlocked = true;
			icon.mainTexture = skill.icon;
		}
	}
	
	/// <summary>
	/// Update the table, if there is one.
	/// </summary>

	void UpdateTable ()
	{
		if(unlocked)
		{
			UITable table = NGUITools.FindInParents<UITable>(gameObject);
			if (table != null) table.repositionNow = true;
		}
	}

	/// <summary>
	/// Drop the dragged object.
	/// </summary>

	void Drop ()
	{
		if(unlocked)
		{
			// Is there a droppable container?
			Collider col = UICamera.lastHit.collider;
			DragDropContainer container = (col != null) ? col.gameObject.GetComponent<DragDropContainer>() : null;
	
			mTrans.parent = mParent;
	
			// Notify the table of this change
			UpdateTable();
	
			// Make all widgets update their parents
			NGUITools.MarkParentAsChanged(gameObject);
		}
	}

	/// <summary>
	/// Cache the transform.
	/// </summary>

	void Awake () { mTrans = transform; }

	/// <summary>
	/// Start the drag event and perform the dragging.
	/// </summary>

	void OnDrag (Vector2 delta)
	{
		if(unlocked)
		{
			if (enabled && UICamera.currentTouchID > -2)
			{
				if (!mIsDragging)
				{
					mIsDragging = true;
					mParent = mTrans.parent;
					mTrans.parent = DragDropRoot.root;
					
					Vector3 pos = mTrans.localPosition;
					pos.z = 0f;
					mTrans.localPosition = pos;
	
					NGUITools.MarkParentAsChanged(gameObject);
				}
				else
				{
					mTrans.localPosition += (Vector3)delta;
				}
			}
		}
	}

	/// <summary>
	/// Start or stop the drag operation.
	/// </summary>

	void OnPress (bool isPressed)
	{
		if (enabled && unlocked)
		{
			AudioManager.PlaySound(drag_sound, transform.position);
			if (isPressed)
			{
				if (!UICamera.current.stickyPress)
				{
					mSticky = true;
					UICamera.current.stickyPress = true;
				}
			}
			else if (mSticky)
			{
				mSticky = false;
				UICamera.current.stickyPress = false;
			}

			mIsDragging = false;
			Collider col = collider;
			if (col != null) col.enabled = !isPressed;
			if (!isPressed) Drop();
		}
	}
	
	void OnTooltip (bool show)
    {
		if(show && unlocked)
		{
			UITooltip.ShowText(skill.GetDescription());
		}
		else
		{
			UITooltip.ShowText(null);
		}
    }
}
