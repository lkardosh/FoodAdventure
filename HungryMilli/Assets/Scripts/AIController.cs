using UnityEngine;
using System.Collections;
public class AIController : MonoBehaviour {

// all my declarations

	public float			attackDistance = 10.0f;
	private float 			gravity = 50.0f;
	private int			attackValue = 20;

    // private float runAwayRadius = 25.0f;
    public float walkSpeed = 6.0f;

    public float fastWalkSpeed = 7.0f;

    public float runSpeed = 9.0f;
    public float runAwaySpeed = 3.0f;
    public float sightRadiusIdle = 30.0f;

    public float sightRadiusPatrol = 40.0f;

    public float sightRadiusPatrolFast = 50.0f;

    private float currentSightRadius;
    
    public float fieldOfView = 120.0f;

    public float fieldOfViewPatrol = 130.0f;
    public float fieldOfViewPatrolFast = 140.0f;
	
	private CharacterController controller;
	private PlayerStatus	playerStatus;
	private Transform		target;
	private Vector3			moveDirection = new Vector3(0,0,0);
    private bool isAttacking = false;
	private State			currentState;
	private Animation		anim;
    private AIStatus aiStatus;  
    private bool isRotating = false;

	private bool			isControllable = true;
	private bool			isDead = false;

	//This is a hack for legacy animation
	private bool			deathStarted = false; 
    private float patrolTimer = 0f;

	public bool 	IsControllable {
		get {return isControllable;}
		set {isControllable = value;}
	}
    
    public bool IsDead {
        get { return isDead; }
        set { isDead = value; }
    }

    public float GetGetHealth() {
        float myHealth = aiStatus.GetHealth(); 
        return myHealth;
    }


	// Use this for initialization
	void Start () {
    controller = GetComponent<CharacterController>();
    anim = GetComponent<Animation>();
    aiStatus = GetComponent<AIStatus>();
	GameObject tmp = GameObject.FindWithTag("Player");
	if (tmp != null){
		target=tmp.transform;
		playerStatus = tmp.GetComponent< PlayerStatus>();
	}

	ChangeState(new StateIdle());
}
	//changes the state
	public void ChangeState(State newState){
		currentState = newState;
	}

    public void BeDead(){
		//This is a hack for legacy animation
		if (!deathStarted)
		{
			anim.CrossFade("die", 0.1f);
			deathStarted = true;
			CharacterController controller = GetComponent<CharacterController>();
			controller.enabled = false;

		}
		moveDirection = new Vector3(0,0,0);

		if (!anim.isPlaying)
        {
			StartCoroutine(timedDeatcivation(4f));
		}
	}
    IEnumerator timedDeatcivation(float seconds){		

		//Wait for several seconds
		yield return new WaitForSeconds(seconds);
		gameObject.SetActive(false);
		this.IsControllable = false;
	}

    public void BeIdle(){
        //animates movement
		anim.CrossFade("idle", 0.2f);
		anim["idle"].speed = 1.0f;
		moveDirection = new Vector3(0,0,0);
        currentSightRadius = sightRadiusIdle;
	}
	
	void OnDisable()
	{
		/*
		 * If you uncomment this, you need to somehow tell the PlayerController to update
		 * the enemies array by calling GameObject.FindGameObjectsWithTag("Enemy").
		 * Otherwise the reference to a desgtroyed GameObejct will still be in the enemies 
		 * array and you will get null pointer exceptions if you try to access it
		 */

		//Destroy(gameObject);

	}
    void Update() {
   
        if (!isControllable)
			return;
		
		currentState.Execute(this);	
    }

    //get functions, used in state funcions to get key values
    public float getDistanceToPlayer(){
        return Vector3.Distance(transform.position, target.position);
    } 
    public float getAttackDistance(){
        return attackDistance;
    } 

    public float getSightRadius(){
        return currentSightRadius;
    }


    //checks if player is within view
    public bool IsPlayerSeen() {
        if (!target) return false;

        Vector3 direction = (target.position - transform.position).normalized;
        float angle = Vector3.Angle(transform.forward, direction);
        return angle < fieldOfView / 2 && Vector3.Distance(transform.position, target.position) < getSightRadius();
    }
    //attacks player
    public void BeAttacking() {
        if (!isAttacking) {
            StartCoroutine(AttackCoroutine());
        }
        Vector3 directionToPlayer = (target.position - transform.position).normalized;
        float targetAngle = Mathf.Atan2(directionToPlayer.x, directionToPlayer.z) * Mathf.Rad2Deg;
        transform.eulerAngles = new Vector3(0, targetAngle, 0); 
    }

    private IEnumerator AttackCoroutine() {
        isAttacking = true;
        //animates movement
        anim.CrossFade("attack", 0.1f);
        anim["attack"].speed = 1.0f;

        //ensures animation plays
        yield return new WaitForSeconds(anim["attack"].length * 0.5f); 
        if (target != null && playerStatus != null) {
            playerStatus.ApplyDamage(attackValue);
        }
        yield return new WaitForSeconds(anim["attack"].length * 0.5f);  

        isAttacking = false;
    }

    public void bePatrolling() {
        anim.CrossFade("run", 0.1f);
        anim["run"].speed = 0.7f;
        moveDirection = transform.forward * walkSpeed;
        currentSightRadius = sightRadiusPatrol;

        if (controller.Move(moveDirection * Time.deltaTime) == CollisionFlags.Sides) {
            TurnAround();
        }

        patrolTimer += Time.deltaTime;
        if (patrolTimer >= 6.0f && !isRotating) {
            StartCoroutine(Rotate360());
            patrolTimer = 0;
        }
    }

    public void bePatrollingFast() {
        anim.CrossFade("run", 0.1f);
        anim["run"].speed = 1.0f;
        moveDirection = transform.forward * fastWalkSpeed;
        currentSightRadius = sightRadiusPatrolFast;

        if (controller.Move(moveDirection * Time.deltaTime) == CollisionFlags.Sides) {
            TurnAround();
        }

        patrolTimer += Time.deltaTime;
        if (patrolTimer >= 6.0f && !isRotating) {
            StartCoroutine(Rotate360());
            patrolTimer = 0;
        }
    }

    private IEnumerator Rotate360() {
        isRotating = true;
        float rotationAmount = 0;
        while (rotationAmount < 360f) {
            float rotationStep = 120f * Time.deltaTime;
            transform.Rotate(0, rotationStep, 0);
            rotationAmount += rotationStep;
            yield return null;
        }
        isRotating = false;
    }

    private void TurnAround() {
        transform.Rotate(0, 180f, 0);
    }
public void BeRunning() {
    //animates movement
    anim.CrossFade("run", 0.1f);
    anim["run"].speed = 1.0f;

    Vector3 directionToPlayer = (target.position - transform.position).normalized;

    float targetAngle = Mathf.Atan2(directionToPlayer.x, directionToPlayer.z) * Mathf.Rad2Deg;
    transform.eulerAngles = new Vector3(0, targetAngle, 0); // Face player

    moveDirection = transform.forward * runSpeed; 
    moveDirection.y = -gravity * Time.deltaTime;

    controller.Move(moveDirection * Time.deltaTime); 
}
    public void BeDancing() {
        //animates movement
        anim.CrossFade("dance", 0.2f);
        anim["dance"].speed = 1.0f;
    }
    public int GetPlayerFoodCount() {
        if (playerStatus != null) {
            return playerStatus.GetFoodCount();
        }
        return 0;
    }
    //checks if milli is alive
    public bool IsPlayerAlive() {
        return playerStatus != null && playerStatus.isAlive();
    }
}