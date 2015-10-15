using UnityEngine;
using System.Collections;
using System;
using Pathfinding;

public abstract class AIStatus : MonoBehaviour {
	
	public Skill[] skills;
	public int skill_size;
	
	protected AI ai;
	
	public bool move_to_target = false;
	
	protected float min_follow_distance = 10.0f; //minimum distance that a folloer can get close to the player while following
	protected Vector3 move_direction;
	
	public IEnumerator StartStatus(AI a)
	{
		ai = a;
		bool in_state = !ai.status_manager.is_dead;
		while(in_state)
		{
			yield return StartCoroutine(Execute());
			
			if(CheckPreviousState())
			{
				in_state = false;
				ToPreviousState();
			}
			else if(CheckNextState())
			{
				in_state = false;
				ToNextState();
			}
			
			//start allover again if the current target does not exists or is dead
			if(ai.status_manager.target == null || ((ai.status_manager.target.GetComponent("StatusManager") as StatusManager).is_dead))
			{
				in_state = false;
				StartCoroutine(ai.detection.StartStatus(ai));
			}
			
			in_state = in_state && !ai.status_manager.is_dead;
			yield return new WaitForSeconds(Time.deltaTime);
		}
	}
	
	protected IEnumerator LookAt()
	{
		while(ai.CannotMove())
		{
			yield return new WaitForSeconds(Time.deltaTime);	
		}
		if(ai.status_manager.target != null)
		{
			transform.LookAt(new Vector3(ai.status_manager.target.transform.position.x, transform.position.y, ai.status_manager.target.transform.position.z));
		}
	}
	
	protected IEnumerator Follow()
	{
		if(!ai.CannotMove() && (ai.status_manager.target != null) && (ai.ai_type == AI.Type.pet) && (ai.status_manager.target.tag == "Player") && !CheckDistance(ai.status_manager.target, gameObject, min_follow_distance))
		{
			if(ai.startHasRun)
			{
				ai.SendMessage("OnEnable");	
			}
			float runtime = 3.0f;
			while(runtime > 0f)
			{
				if(ai.CannotMove() || CheckDistance(ai.status_manager.target, gameObject, min_follow_distance))
				{
					runtime = 0f;	
				}
				ai.status_manager.animator.SetBool("run", true);
				move_to_target = true;
				yield return new WaitForSeconds(Time.deltaTime);
				ai.status_manager.animator.SetBool("run", false);
				runtime -= Time.deltaTime;
			}
			move_to_target = false;
		}
	}
	
	protected IEnumerator Move()
	{
		if(!ai.CannotMove())
		{
			float runtime = 3.0f;
			while(runtime > 0f)
			{
				if(CheckNextState() || CheckPreviousState() || ai.CannotMove())
				{
					runtime = 0f;	
				}
				move_to_target = true;
				yield return new WaitForSeconds(Time.deltaTime);
				runtime -= Time.deltaTime;
			}
			move_to_target = false;
		}
	}
	
	protected bool CheckDistance(GameObject x, GameObject y, float distance)
	{
		return Vector3.Distance(x.transform.position, y.transform.position) <= distance;
	}
	
	protected void StartSkill(Skill s)
	{
		ai.current_skill = s;
		s.Activiate();
	}
	
	protected IEnumerator ExecuteSkills()
	{
		if(skill_size != 0 && ai.status_manager.target != null && !ai.status_manager.is_stand_casting && !ai.status_manager.is_move_casting)
		{
			yield return StartCoroutine(LookAt());
			int index = UnityEngine.Random.Range(0, skill_size); //randomly cast a skill
			StartSkill(skills[index]);
			yield return new WaitForSeconds(1f);
		}
	}
	
	protected abstract IEnumerator Execute();
	protected abstract bool CheckPreviousState();
	protected abstract bool CheckNextState();
	protected abstract void ToPreviousState();
	protected abstract void ToNextState();
}
