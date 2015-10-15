//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2012 Tasharen Entertainment (edited by Ka Seng Chou)
//----------------------------------------------

using UnityEngine;

/// <summary>
/// Simple example of an OnDrop event accepting a game object. In this case we check to see if there is a DragDropObject present,
/// and if so -- create its prefab on the surface, then destroy the object.
/// </summary>

public class ItemSlot : MonoBehaviour
{
	public bool rotatePlacedObject = false;
	public Item current_item;
	public int item_slot_id;
	public UITexture icon;
	
	public AudioClip drop_sound;
	public AudioClip drop_deny_sound;

	void OnDrop (GameObject go)
	{
		DragDropItem_new slot = go.GetComponent<DragDropItem_new>();
		if (slot != null && slot.item != null)
		{
			if(slot.item.unique)
			{
				foreach(Item i in ItemManager.items)
				{
					if(i != null && (i.name == current_item.name))
					{
						return;	
					}
				}
			}
			
			if(ItemManager.ChangeItem(slot.item, item_slot_id))
			{
				slot.icon.mainTexture = null;
				slot.icon.enabled = false;
				current_item = slot.item;
				slot.item = null;
				AudioManager.PlaySound(drop_sound, transform.position);
			}
			else
			{
				AudioManager.PlaySound(drop_deny_sound, transform.position);
			}
		}
		else
		{
			ItemSlot slot2 = go.GetComponent<ItemSlot>();
			if(slot2 != null && slot2.current_item != null)
			{
				if(ItemManager.ChangeItem(slot2.current_item, item_slot_id))
				{
					ItemManager.ChangeItem(null, slot2.item_slot_id);
					AudioManager.PlaySound(drop_sound, transform.position);
				}
				else
				{
					AudioManager.PlaySound(drop_deny_sound, transform.position);
				}
			}
		}
	}
	
	void OnTooltip (bool show)
    {
		if(show && current_item != null)
		{
			UITooltip.ShowText(current_item.GetDescription());
		}
		else
		{
			UITooltip.ShowText(null);
		}
    }
	
	//---------------------------------------
	
	public AudioClip drag_sound;
	
	Transform mTrans;
	bool mIsDragging = false;
	bool mSticky = false;
	Transform mParent;
	
	/// <summary>
	/// Update the table, if there is one.
	/// </summary>

	void UpdateTable ()
	{
		if(current_item != null)
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
		if(current_item != null)
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
		if(current_item != null)
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
		if (enabled && current_item != null)
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
	
}