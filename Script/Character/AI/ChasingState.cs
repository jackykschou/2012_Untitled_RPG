using UnityEngine;
using System.Collections;
using System;

public class ChasingState : AIStatus {
	
	protected override sealed IEnumerator Execute()
	{	
		if(ai.startHasRun)
		{
			ai.SendMessage("OnEnable");	
		}
		ai.status_manager.animator.SetBool("run", true);
		yield return StartCoroutine(Move());
		yield return StartCoroutine(ExecuteSkills());
	}
	
	
	protected override sealed bool CheckPreviousState()
	{
		if(ai.status_manager.target == null || (ai.ai_type == AI.Type.challenger && ai.status_manager.target.tag != "Player"))
		{
			return true;
		}
		else if(ai.ai_type == AI.Type.coward && ai.status_manager.target.tag != "Pet")
		{
			GameObject[] pets = GameObject.FindGameObjectsWithTag("Pet");
			if(pets.GetLength(0) != 0)
			{
				return true;
			}
		}
		return false;
	}
	
	protected override sealed bool CheckNextState()
	{
		if(ai.status_manager.target != null)
		{
			return CheckDistance(gameObject, ai.status_manager.target, ai.attact_range);
		}
		
		return false;
	}
	
	protected override sealed void ToPreviousState()
	{
		ai.status_manager.animator.SetBool("run", false);
		StartCoroutine(ai.detection.StartStatus(ai));	
	}
	
	protected override sealed void ToNextState()
	{
		ai.status_manager.animator.SetBool("run", false);
		StartCoroutine(ai.attack.StartStatus(ai));	
	}
}
