using UnityEngine;
using System.Collections;
using System;
using Pathfinding;
using System.Collections.Generic;

public class AI : MonoBehaviour {
	
	
	//type of the ai: pet of player, normal: attack closest, challenger: attack main character in priority
	//coward: attack pets in priority
	public enum Type
	{pet, normal, challenger, coward} 

	public StatusManager status_manager;
	public GameObject health_bar;
	
	public AIStatus detection;
	public AIStatus chasing;
	public AIStatus attack;
	public Type ai_type;
	
	public float detection_range;
	public float chasing_range;
	public float attact_range;
	
	private bool prepare_falling;
	private float falling_buff_time = 1.0f;
	public Skill current_skill;
	
	public GameObject[] drop_items; //the fronter element in the array is priority to be dropped (can only drop an item at a time)
	public float[] drop_item_chances;
	public GameObject drop_health_potion;
	public float drop_health_potion_chance;
	public GameObject drop_mana_potion;
	public float drop_mana_potion_chance;
	public float exp;
	
	public float gravity;
	
	public AudioClip detect_sound;
	
	private bool dead = false;
	
	void SetUpSkills(AIStatus status)
	{
		foreach(Skill s in status.skills)
		{
			if(s != null)
			{
				s.ownership = ((ai_type == Type.pet) ? Skill.Ownership.petAi : Skill.Ownership.enemyAi);
				s.SetUp(status_manager);
			}
		}
	}
	
	public bool CannotMove()
	{
		return status_manager.is_knocked || status_manager.is_stand_casting || status_manager.is_stun || 
			status_manager.is_rooted || status_manager.is_dead || status_manager.is_falling;
	}
	
	private void DropItems()
	{
		bool dropped = false; //only one item can be dropped
		int item_num = drop_items.GetLength(0);
		for(int i = 0; i < item_num && !dropped; ++i)
		{
			 dropped = DropItem(drop_items[i], drop_item_chances[i]);	
		}
		
		DropItem(drop_health_potion, drop_health_potion_chance);
		DropItem(drop_mana_potion, drop_mana_potion_chance);
		
	}
	
	private bool DropItem(GameObject item, float chance)
	{
		if(item != null && UnityEngine.Random.value <= chance && chance != 0f)
		{
			(Instantiate(item, transform.position, Quaternion.identity) as GameObject).SetActive(true);
			return true;
		}
		return false;
	}
	
	void OnCollisionExit(Collision coll)
	{
		prepare_falling = true;
		status_manager.is_falling = true;
	}
	
	void OnTriggerStay(Collider coll)
	{
		falling_buff_time = 0f;
		status_manager.is_falling = false;
		prepare_falling = false;
	}
	
	public void CastSkill()
	{
		if(status_manager.target != null)
		{
			current_skill.StartEffect();
		}
	}
	
	//****************The following functions are from the AIPath in the example of A * Pathfinding Project (some of them get edited) **********************//
	
	/** Determines how often it will search for new paths. 
	 * If you have fast moving targets or AIs, you might want to set it to a lower value.
	 * The value is in seconds between path requests.
	 */
	public float repathRate = 0.5F;
	
	/** Enables or disables searching for paths.
	 * Setting this to false does not stop any active path requests from being calculated or stop it from continuing to follow the current path.
	 * \see #canMove
	 */
	public bool canSearch = true;
	
	/** Enables or disables movement.
	  * \see #canSearch */
	public bool canMove = true;
	
	/** Maximum velocity.
	 * This is the maximum speed in world units per second.
	 */
	public float speed;
	
	/** Rotation speed.
	 * Rotation is calculated using Quaternion.SLerp. This variable represents the damping, the higher, the faster it will be able to rotate.
	 */
	public float turningSpeed = 10f;
	
	/** Distance from the target point where the AI will start to slow down.
	 * Note that this doesn't only affect the end point of the path
 	 * but also any intermediate points, so be sure to set #forwardLook and #pickNextWaypointDist to a higher value than this
 	 */
	public float slowdownDistance = 0.3F;
	
	/** Determines within what range it will switch to target the next waypoint in the path */
	public float pickNextWaypointDist = 2;
	
	/** Target point is Interpolated on the current segment in the path so that it has a distance of #forwardLook from the AI.
	  * See the detailed description of AIPath for an illustrative image */
	public float forwardLook = 1;
	
	/** Distance to the end point to consider the end of path to be reached.
	 * When this has been reached, the AI will not move anymore until the target changes and OnTargetReached will be called.
	 */
	public float endReachedDistance = 0.2F;
	
	/** Do a closest point on path check when receiving path callback.
	 * Usually the AI has moved a bit between requesting the path, and getting it back, and there is usually a small gap between the AI
	 * and the closest node.
	 * If this option is enabled, it will simulate, when the path callback is received, movement between the closest node and the current
	 * AI position. This helps to reduce the moments when the AI just get a new path back, and thinks it ought to move backwards to the start of the new path
	 * even though it really should just proceed forward.
	 */
	public bool closestOnPathCheck = true;
	
	protected float minMoveScale = 0.05F;
	
	/** Cached Seeker component */
	protected Seeker seeker;
	
	/** Cached Transform component */
	protected Transform tr;
	
	/** Time when the last path request was sent */
	private float lastRepath = -9999;
	
	/** Current path which is followed */
	protected Path path;
	
	/** Cached CharacterController component */
	protected CharacterController controller;
	
	/** Cached NavmeshController component */
	protected NavmeshController navController;
	
	
	/** Cached Rigidbody component */
	protected Rigidbody rigid;
	
	/** Current index in the path which is current target */
	protected int currentWaypointIndex = 0;
	
	/** Holds if the end-of-path is reached
	 * \see TargetReached */
	protected bool targetReached = false;
	
	/** Only when the previous path has been returned should be search for a new path */
	protected bool canSearchAgain = true;
	
	/** Returns if the end-of-path has been reached
	 * \see targetReached */
	public bool TargetReached {
		get {
			return targetReached;
		}
	}
	
	/** Holds if the Start function has been run.
	 * Used to test if coroutines should be started in OnEnable to prevent calculating paths
	 * in the awake stage (or rather before start on frame 0).
	 */
	public bool startHasRun = false;
	
	/** Initializes reference variables.
	 * If you override this function you should in most cases call base.Awake () at the start of it.
	  * */
	protected virtual void Awake () {
		seeker = GetComponent<Seeker>();
		
		//This is a simple optimization, cache the transform component lookup
		tr = transform;
		
		//Make sure we receive callbacks when paths complete
		seeker.pathCallback += OnPathComplete;
		
		//Cache some other components (not all are necessarily there)
		controller = GetComponent<CharacterController>();
		navController = GetComponent<NavmeshController>();
		rigid = rigidbody;
	}
	
	/** Starts searching for paths.
	 * If you override this function you should in most cases call base.Start () at the start of it.
	 * \see OnEnable
	 * \see RepeatTrySearchPath
	 */
	protected virtual void Start () 
	{
		speed = status_manager.move_speed;
		StartCoroutine(detection.StartStatus(this));
		SetUpSkills(detection);
		SetUpSkills(chasing);
		SetUpSkills(attack);
		startHasRun = true;
	}
	
	/** Run at start and when reenabled.
	 * Starts RepeatTrySearchPath.
	 * 
	 * \see Start
	 */
	public virtual void OnEnable () {
		if (startHasRun) 
		{
			StartCoroutine (RepeatTrySearchPath ());
			startHasRun = false;
		}
	}
	
	/** Tries to search for a path every #repathRate seconds.
	  * \see TrySearchPath
	  */
	public IEnumerator RepeatTrySearchPath () {
		while (true) {
			TrySearchPath ();
			yield return new WaitForSeconds (repathRate);
		}
	}
	
	/** Tries to search for a path.
	 * Will search for a new path if there was a sufficient time since the last repath and both
	 * #canSearchAgain and #canSearch are true.
	 * Otherwise will start WaitForPath function.
	 */
	public void TrySearchPath () {
		if (Time.time - lastRepath >= repathRate && canSearchAgain && canSearch) {
			SearchPath ();
		} else {
			StartCoroutine (WaitForRepath ());
		}
	}
	
	/** Is WaitForRepath running */
	private bool waitingForRepath = false;
	
	/** Wait a short time til Time.time-lastRepath >= repathRate.
	  * Then call TrySearchPath
	  * 
	  * \see TrySearchPath
	  */
	protected IEnumerator WaitForRepath () {
		if (waitingForRepath) yield break; //A coroutine is already running
		
		waitingForRepath = true;
		//Wait until it is predicted that the AI should search for a path again
		yield return new WaitForSeconds (repathRate - (Time.time-lastRepath));
		
		waitingForRepath = false;
		//Try to search for a path again
		TrySearchPath ();
	}
	
	/** Requests a path to the target */
	public virtual void SearchPath () {
		
		if (status_manager.target == null || (status_manager.target.GetComponent("StatusManager") as StatusManager).is_dead) { startHasRun = true; return; }
		
		lastRepath = Time.time;
		//This is where we should search to
		Vector3 targetPosition = status_manager.target.transform.position;
		
		canSearchAgain = false;
		
		//Alternative way of requesting the path
		//Path p = PathPool<Path>.GetPath().Setup(GetFeetPosition(),targetPoint,null);
		//seeker.StartPath (p);
		
		//We should search from the current position
		seeker.StartPath (GetFeetPosition(), targetPosition);
	}
	
	public virtual void OnTargetReached () {
		//End of path has been reached
		//If you want custom logic for when the AI has reached it's destination
		//add it here
		//You can also create a new script which inherits from this one
		//and override the function in that script
	}
	
	public void OnDestroy () {
		if (path != null) path.Release (this);
	}
	
	/** Called when a requested path has finished calculation.
	  * A path is first requested by #SearchPath, it is then calculated, probably in the same or the next frame.
	  * Finally it is returned to the seeker which forwards it to this function.\n
	  */
	public virtual void OnPathComplete (Path _p) {
		ABPath p = _p as ABPath;
		if (p == null) throw new System.Exception ("This function only handles ABPaths, do not use special path types");
		
		//Release the previous path
		if (path != null) path.Release (this);
		
		//Claim the new path
		p.Claim (this);
		
		//Replace the old path
		path = p;
		
		//Reset some variables
		currentWaypointIndex = 0;
		targetReached = false;
		canSearchAgain = true;
		
		//The next row can be used to find out if the path could be found or not
		//If it couldn't (error == true), then a message has probably been logged to the console
		//however it can also be got using p.errorLog
		//if (p.error)
		
		if (closestOnPathCheck) {
			Vector3 p1 = p.startPoint;
			Vector3 p2 = GetFeetPosition ();
			float magn = Vector3.Distance (p1,p2);
			Vector3 dir = p2-p1;
			dir /= magn;
			int steps = (int)(magn/pickNextWaypointDist);
			for (int i=0;i<steps;i++) {
				CalculateVelocity (p1);
				p1 += dir;
			}
		}
	}
	
	public virtual Vector3 GetFeetPosition () {
		if (controller != null) {
			return tr.position - Vector3.up*controller.height*0.5F;
		}

		return tr.position;
	}
	
	public virtual void Update () {
		
		health_bar.SendMessage("SetValueF", (status_manager.current_health / status_manager.max_health));
		
		if(status_manager.is_dead && !dead) //dead is used to prevent dropping the items multiple times
		{
			dead = true;
			DropItems();
			gameObject.tag = "Untagged";
			LevelManager.player_status.AddExp(exp);
			StartCoroutine(SetActiveFalse());
			Destroy(gameObject, 20f);
		}
		
		if (status_manager.target == null || (status_manager.target.GetComponent("StatusManager") as StatusManager).is_dead) { startHasRun = true; return; }
		
		if(prepare_falling)
		{
			falling_buff_time += Time.deltaTime;
		}
		
		status_manager.controller.Move(new Vector3(0f, gravity, 0f) * Time.deltaTime);
		
		if (!detection.move_to_target && !chasing.move_to_target) { return;}
		
		Vector3 dir = CalculateVelocity (GetFeetPosition());
		
		//Rotate towards targetDirection (filled in by CalculateVelocity)
		if (targetDirection != Vector3.zero) {
			RotateTowards (targetDirection);
		}
		
		if (navController != null) {
			navController.SimpleMove (GetFeetPosition(),dir);
		} else if (controller != null) {
			controller.SimpleMove (dir);
		} else if (rigid != null) {
			rigid.AddForce (dir);
		} else{
			transform.Translate (dir*Time.deltaTime, Space.World);
		}
	}
	
	/** Point to where the AI is heading.
	  * Filled in by #CalculateVelocity */
	protected Vector3 targetPoint;
	/** Relative direction to where the AI is heading.
	 * Filled in by #CalculateVelocity */
	protected Vector3 targetDirection;
	
	protected float XZSqrMagnitude (Vector3 a, Vector3 b) {
		float dx = b.x-a.x;
		float dz = b.z-a.z;
		return dx*dx + dz*dz;
	}
	
	/** Calculates desired velocity.
	 * Finds the target path segment and returns the forward direction, scaled with speed.
	 * A whole bunch of restrictions on the velocity is applied to make sure it doesn't overshoot, does not look too far ahead,
	 * and slows down when close to the target.
	 * /see speed
	 * /see endReachedDistance
	 * /see slowdownDistance
	 * /see CalculateTargetPoint
	 * /see targetPoint
	 * /see targetDirection
	 * /see currentWaypointIndex
	 */
	protected Vector3 CalculateVelocity (Vector3 currentPosition) {
		if (path == null || path.vectorPath == null || path.vectorPath.Count == 0) return Vector3.zero; 
		
		List<Vector3> vPath = path.vectorPath;
		//Vector3 currentPosition = GetFeetPosition();
		
		if (vPath.Count == 1) {
			vPath.Insert (0,currentPosition);
		}
		
		if (currentWaypointIndex >= vPath.Count) { currentWaypointIndex = vPath.Count-1; }
		
		if (currentWaypointIndex <= 1) currentWaypointIndex = 1;
		
		while (true) {
			if (currentWaypointIndex < vPath.Count-1) {
				//There is a "next path segment"
				float dist = XZSqrMagnitude (vPath[currentWaypointIndex], currentPosition);
					//Mathfx.DistancePointSegmentStrict (vPath[currentWaypointIndex+1],vPath[currentWaypointIndex+2],currentPosition);
				if (dist < pickNextWaypointDist*pickNextWaypointDist) {
					currentWaypointIndex++;
				} else {
					break;
				}
			} else {
				break;
			}
		}
		
		Vector3 dir = vPath[currentWaypointIndex] - vPath[currentWaypointIndex-1];
		Vector3 targetPosition = CalculateTargetPoint (currentPosition,vPath[currentWaypointIndex-1] , vPath[currentWaypointIndex]);
			//vPath[currentWaypointIndex] + Vector3.ClampMagnitude (dir,forwardLook);
		
		
		
		dir = targetPosition-currentPosition;
		dir.y = 0;
		float targetDist = dir.magnitude;
		
		float slowdown = Mathf.Clamp01 (targetDist / slowdownDistance);
		
		this.targetDirection = dir;
		this.targetPoint = targetPosition;
		
		if (currentWaypointIndex == vPath.Count-1 && targetDist <= endReachedDistance) {
			if (!targetReached) { targetReached = true; OnTargetReached (); }
			
			//Send a move request, this ensures gravity is applied
			return Vector3.zero;
		}
		
		Vector3 forward = tr.forward;
		float dot = Vector3.Dot (dir.normalized,forward);
		float sp = speed * Mathf.Max (dot,minMoveScale) * slowdown;
		
		
		if (Time.deltaTime	> 0) {
			sp = Mathf.Clamp (sp,0,targetDist/(Time.deltaTime*2));
		}
		return forward*sp;
	}
	
	/** Rotates in the specified direction.
	 * Rotates around the Y-axis.
	 * \see turningSpeed
	 */
	protected virtual void RotateTowards (Vector3 dir) {
		Quaternion rot = tr.rotation;
		Quaternion toTarget = Quaternion.LookRotation (dir);
		
		rot = Quaternion.Slerp (rot,toTarget,turningSpeed*Time.fixedDeltaTime);
		Vector3 euler = rot.eulerAngles;
		euler.z = 0;
		euler.x = 0;
		rot = Quaternion.Euler (euler);
		
		tr.rotation = rot;
	}
	
	/** Calculates target point from the current line segment.
	 * \param p Current position
	 * \param a Line segment start
	 * \param b Line segment end
	 * The returned point will lie somewhere on the line segment.
	 * \see #forwardLook
	 * \todo This function uses .magnitude quite a lot, can it be optimized?
	 */
	protected Vector3 CalculateTargetPoint (Vector3 p, Vector3 a, Vector3 b) {
		a.y = p.y;
		b.y = p.y;
		
		float magn = (a-b).magnitude;
		if (magn == 0) return a;
		
		float closest = Mathfx.Clamp01 (Mathfx.NearestPointFactor (a, b, p));
		Vector3 point = (b-a)*closest + a;
		float distance = (point-p).magnitude;
		
		float lookAhead = Mathf.Clamp (forwardLook - distance, 0.0F, forwardLook);
		
		float offset = lookAhead / magn;
		offset = Mathf.Clamp (offset+closest,0.0F,1.0F);
		return (b-a)*offset + a;
	}
	
	private IEnumerator SetActiveFalse()
	{
		yield return new WaitForSeconds(5f);
		gameObject.SetActive(false);
	}
	
}
