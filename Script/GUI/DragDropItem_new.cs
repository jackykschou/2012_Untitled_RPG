using UnityEngine;
using System.Collections;

public class DragDropItem_new : MonoBehaviour {

	/// <summary>
	/// Prefab object that will be instantiated on the DragDropSurface if it receives the OnDrop event.
	/// </summary>

	public Item item;
	public UITexture icon;
	
	public AudioClip drag_sound;
	
	Transform mTrans;
	bool mIsDragging = false;
	bool mSticky = false;
	Transform mParent;
	
	public int id;
	
	public void AssignItem(Item i)
	{
		if(i == null)
		{
			i = null;
			MetaDataManager.RemoveItemFromBag(id);
			icon.enabled = false;
			item = null;
		}
		else
		{
			item = i;
			MetaDataManager.AddItemToBag(id, i.id_in_pool);
			icon.enabled = true;
			icon.mainTexture = i.icon;
		}
	}
	
	/// <summary>
	/// Update the table, if there is one.
	/// </summary>

	void UpdateTable ()
	{
		if(item != null)
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
		if(item != null)
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
		if(item != null)
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
		if (enabled && item != null)
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
		if(show && item != null)
		{
			UITooltip.ShowText(item.GetDescription());
		}
		else
		{
			UITooltip.ShowText(null);
		}
    }
	
	public AudioClip drop_sound;
	public AudioClip drop_deny_sound;
	
	void OnDrop (GameObject go)
	{
		DragDropItem_new slot = go.GetComponent<DragDropItem_new>();
		if (slot != null && slot.item != null)
		{
			if(item == null)
			{
				AssignItem(slot.item);
				slot.AssignItem(null);
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
			if(slot2 != null && slot2.current_item != null && item == null)
			{
				AssignItem(slot2.current_item);
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
