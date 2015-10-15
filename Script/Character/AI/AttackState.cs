using UnityEngine;
using System.Collections;

public class AttackState : AIStatus {

	protected override sealed IEnumerator Execute()
	{
		yield return StartCoroutine(LookAt());
		yield return StartCoroutine(ExecuteSkills());
	}
	
	
	protected override sealed bool CheckPreviousState()
	{
		if(ai.status_manager.target != null)
		{
			return !CheckDistance(gameObject, ai.status_manager.target, ai.attact_range);
		}
		return false;
	}
	
	protected override sealed bool CheckNextState()
	{
		return false;
	}
	
	protected override sealed void ToPreviousState()
	{
		StartCoroutine(ai.chasing.StartStatus(ai));	
	}
	
	protected override sealed void ToNextState(){}
	
}
