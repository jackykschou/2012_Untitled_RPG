using UnityEngine;
using System.Collections;

//control input and animations of the main character
public class CharacterActionController : MonoBehaviour 
{
	public PlayerStatusManager status_manager; //the status manager of the character
	
	public AudioClip[] dodge_sounds;
	
	//varibles help to deal with the falling animation
	public float leaveGroundTimer; //used to delay falling action
	public bool onGround;
	public bool falling;
	public bool prepareToFall;
	
	private float jump_backward_timer = 0f;
	private float dodge_left_timer = 0f;
	private float dodge_right_timer = 0f;
	
	public float gravity = -9.8f;
	
	void Start () 
	{
		status_manager.animator.SetLayerWeight(0, 1.0f);
		
		leaveGroundTimer = 0f;
		onGround = true;
		falling = false;
		prepareToFall = false;
	}
	
	void Update () 
	{	
		//---------------falling movement---------------
		
		if(!status_manager.controller.isGrounded)
		{
			prepareToFall = true;
		}
		else
		{
			leaveGroundTimer = 0f;
			status_manager.animator.SetBool("falling", false);
			status_manager.animator.SetBool("onGround", true);
		}
		
		if(prepareToFall)
		{
			leaveGroundTimer += Time.deltaTime;
		}
		
		if(leaveGroundTimer > 0.5f)
		{
			status_manager.animator.SetBool("falling", true);
			status_manager.animator.SetBool("onGround", false);
		}
			
		status_manager.is_falling = status_manager.animator.GetBool(Animator.StringToHash("falling"));
		
		if(dodge_left_timer > 0f)
		{
			dodge_left_timer -= Time.deltaTime;
		}
		if(dodge_right_timer > 0f)
		{
			dodge_right_timer -= Time.deltaTime;
		}
		if(jump_backward_timer > 0f)
		{
			jump_backward_timer -= Time.deltaTime;
		}
		
		//get the current states of the animators in different layers
		AnimatorStateInfo stateInfo_base = status_manager.animator.GetCurrentAnimatorStateInfo(0);
		
		//------------------------------------------------------------
		
		//character status
		bool is_disabled = status_manager.is_stun || status_manager.is_dead || status_manager.is_knocked;
		bool is_dodging = stateInfo_base.IsName("BasicMovement.LeftDodge") || stateInfo_base.IsName("BasicMovement.RightDodge") || stateInfo_base.IsName("BasicMovement.BackJump")
						|| stateInfo_base.IsName("BasicMovement.RunJump") || stateInfo_base.IsName("BasicMovement.WalkJump");
		
		//disable mouse skill selection while being disabled or while falling
		if(is_disabled ||  status_manager.is_falling || status_manager.is_slienced)
		{
			status_manager.menu_selector.cancel_selection = true;
			status_manager.menu_selector.selected = true;
		}
		
		//only can perform actions or cast skill if not casting non-movable spell and not disabled and not falling and not in any menu selection (can't move whle rooted)
		if(!is_disabled && !status_manager.is_falling && !status_manager.is_rooted && !status_manager.menu_selector.skill_selecting 
				&& !CameraManager.menu_selecting && !status_manager.is_stand_casting)
		{
			if(!status_manager.is_move_casting && !status_manager.is_stand_casting) //cant dodge while performing any attack
			{
				status_manager.animator.SetBool("dodgeRight", Input.GetKeyDown(KeyCode.D) && dodge_right_timer > 0f);
				status_manager.animator.SetBool("dodgeLeft", Input.GetKeyDown(KeyCode.A) && dodge_left_timer > 0f);
				status_manager.animator.SetBool("jumpBackward", Input.GetKeyDown(KeyCode.S) && jump_backward_timer > 0f);
			}
			
			//---------------basic movements---------------
			
			if(Input.GetKeyDown(KeyCode.D) && dodge_right_timer <= 0f)
			{
				dodge_right_timer = 0.25f;
			}
			else if(Input.GetKeyDown(KeyCode.A) && dodge_left_timer <= 0f)
			{
				dodge_left_timer = 0.25f;
			}
			else if(Input.GetKeyDown(KeyCode.S) && jump_backward_timer <= 0f)
			{
				jump_backward_timer = 0.25f;
			}
			
			//check for input and update animator 
			status_manager.animator.SetBool("run", Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.LeftShift));
			status_manager.animator.SetBool("walk", Input.GetKey(KeyCode.W));
			status_manager.animator.SetBool("strifeLeft", Input.GetKey(KeyCode.A) && dodge_left_timer <= 0f);
			status_manager.animator.SetBool("strifeRight", Input.GetKey(KeyCode.D) && dodge_right_timer <= 0f);
			status_manager.animator.SetBool("walkBack", Input.GetKey(KeyCode.S) && jump_backward_timer <= 0f);
			
			//---------------change the position of the character while playing the movement animation---------------
			
			if(stateInfo_base.IsName("BasicMovement.Walk"))
			{
				status_manager.controller.Move(transform.forward * status_manager.move_speed * Time.deltaTime);
			}
			else if(stateInfo_base.IsName("BasicMovement.Run"))
			{
				status_manager.controller.Move(1.3f * transform.forward * status_manager.move_speed * Time.deltaTime);
			}
			else if(stateInfo_base.IsName("BasicMovement.WalkBack"))
			{
				status_manager.controller.Move(-0.8f * transform.forward * status_manager.move_speed * Time.deltaTime);
			}
			else if(stateInfo_base.IsName("BasicMovement.WalkJump"))
			{
				status_manager.controller.Move(transform.forward * status_manager.dodge_speed * Time.deltaTime);
			}
			else if(stateInfo_base.IsName("BasicMovement.LeftDodge"))
			{	
				status_manager.controller.Move(-transform.right * status_manager.dodge_speed * Time.deltaTime);
			}
			else if(stateInfo_base.IsName("BasicMovement.RightDodge"))
			{
				status_manager.controller.Move(transform.right * status_manager.dodge_speed * Time.deltaTime);
			}
			else if(stateInfo_base.IsName("BasicMovement.BackJump"))
			{
				status_manager.controller.Move(-transform.forward * status_manager.dodge_speed * Time.deltaTime);
			}
			else if(stateInfo_base.IsName("BasicMovement.StrifeLeft"))
			{
				status_manager.controller.Move(-transform.right * status_manager.move_speed * Time.deltaTime);
			}
			else if(stateInfo_base.IsName("BasicMovement.StrifeRight"))
			{
				status_manager.controller.Move(transform.right * status_manager.move_speed * Time.deltaTime);
			}
			
			
			//------------------------------------------------------------------------------------------
		}
		
		//---------------attacks and skills---------------
		
		//for mouse selection skills
		if((Input.GetKeyDown(KeyCode.Mouse0) || Input.GetKeyDown(KeyCode.Mouse1)) && status_manager.menu_selector.skill_selecting && !CameraManager.menu_selecting)
		{	
			status_manager.menu_selector.selected = true;
		}
		
		//only can perform actions or cast skill if not in any menu selection and not dodging
		if(!is_dodging && !CameraManager.menu_selecting && !status_manager.menu_selector.skill_selecting)
		{
			if(Input.GetKeyDown(KeyCode.Mouse0))
			{	
				SkillManager.InputActiviateSkill(0);
			}
			else if(Input.GetKeyDown(KeyCode.Mouse1))
			{
				SkillManager.InputActiviateSkill(1);
			}
			else if(Input.GetKeyDown(KeyCode.Alpha1) && !status_manager.menu_selector.skill_selecting)
			{
				SkillManager.InputActiviateSkill(2);
			}
			else if(Input.GetKeyDown(KeyCode.Alpha2) && !status_manager.menu_selector.skill_selecting)
			{
				
				SkillManager.InputActiviateSkill(3);
			}
			else if(Input.GetKeyDown(KeyCode.Alpha3) && !status_manager.menu_selector.skill_selecting)
			{
				SkillManager.InputActiviateSkill(4);
			}
			else if(Input.GetKeyDown(KeyCode.Alpha4) && !status_manager.menu_selector.skill_selecting)
			{
				SkillManager.InputActiviateSkill(5);
			}
		}
		
		//can use item if character is not disabled or not in menu or not in skill selection
		if(!is_disabled && !CameraManager.menu_selecting && !status_manager.menu_selector.skill_selecting)
		{
				
		}
		
		//switch to different menus
		if(!status_manager.menu_selector.skill_selecting)
		{
			//menu
			if(Input.GetKeyDown(KeyCode.K))
			{
				status_manager.menu_selector.TriggerSkillMenu();
			}
			else if(Input.GetKeyDown(KeyCode.L))
			{
				status_manager.menu_selector.TriggerStatusMenu();
			}
			else if(Input.GetKeyDown(KeyCode.H))
			{
				status_manager.menu_selector.TriggerHelpMenu();
			}
			else if(Input.GetKeyDown(KeyCode.I))
			{
				status_manager.menu_selector.TriggerItemMenu();
			}
		}
		
		//apply gravity
		status_manager.controller.Move(new Vector3(0f, gravity, 0f) * Time.deltaTime);
		
	}
	
	public void CastSkill()
	{
		SkillManager.CastSkill();
	}
	
	public void PlayDodgeSound()
	{
		if(dodge_sounds.GetLength(0) != 0)
		{
			AudioManager.PlaySound(dodge_sounds[UnityEngine.Random.Range(0, dodge_sounds.GetLength(0))], transform.position);	
		}
	}
}

