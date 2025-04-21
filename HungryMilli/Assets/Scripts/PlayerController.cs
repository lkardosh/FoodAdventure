using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    public float attackDamage = 7.0f;
    public float attackDistance = 15.0f;

    public float wiggleDistance = 18.0f;

    private float defaultMoveSpeed;
    public float eatDistance = 6.0f;
    private bool isWiggling = false;
    private bool isZooming = false;
    private float zoomDuration = 6f;
    private float wiggleDuration = 5f;
    private float zoomSpeedMultiplier = 2f;
    private float zoomEnergyCost = 5f;
    public float loseHairDistance = 15.0f;
    public CharacterController controller;
    private PlayerStatus status;

    private Vector3 originalPosition;

    private AIStatus aistatus;
    private bool isAttacking = false;

    private bool loseHairUsed = false;
    private float gravity = -9.8f;
    public float moveSpeed = 20;
    public float sleepySpeed = 10;
    public float rotateSpeed = 60;
    private float jumpHeight = 1;
    Vector3 playerVelocity;
    Vector3 rotateDirection;
    float yVelocity = 0;
    private Animation anim;
    private bool isControllable = true;
    private GameObject[] enemies;


    void Start()
    {
        controller = GetComponent<CharacterController>();
        status = GetComponent<PlayerStatus>();
        anim = GetComponent<Animation>();
        originalPosition = transform.position;
        defaultMoveSpeed = moveSpeed; 
        enemies = GameObject.FindGameObjectsWithTag("Enemy");
        Debug.Log(enemies.Length);
    }

    public bool IsControllable
    {
        get { return isControllable; }
        set { isControllable = value; }
    }

    void OnGUI()
    {
        GUI.Box(new Rect(Screen.width - 150, 0, 160, 44), "Current Energy: " + status.GetHealth().ToString()+ " /100");;
        GUI.Box(new Rect(Screen.width - 150, 40, 160, 44), "Food Eaten: " + status.GetFoodCount().ToString() + " /15");
        GUI.Box(new Rect(Screen.width - 150, 80, 160, 44), "Lives Left: " + status.GetNumLives().ToString());
        GUI.Box(new Rect(Screen.width - 150, 120, 160, 44), "Current Level: " + status.GetNumLevel().ToString());
        
    }

    void Update() {
        if (!isControllable) return; 
        
        if (Input.GetKeyDown(KeyCode.L) && !isAttacking) {
            StartCoroutine(AttackEnemiesCoroutine());
            return;
        }

        if (Input.GetKeyDown(KeyCode.E) && !isAttacking) {
            StartCoroutine(EatFoodCoroutine());
            return;
        }

        if (Input.GetKeyDown(KeyCode.Q) && !isWiggling) {
            StartCoroutine(WiggleCoroutine());
            return;
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) && !isZooming && status.GetHealth() > 20) {
            StartCoroutine(ZoomCoroutine());
            return;
        }

        if (!isAttacking && !isWiggling) {
            HandleMovement();
        }
    }

    void HandleMovement()
    {
        playerVelocity = new Vector3(0, 0, Input.GetAxis("Vertical"));

        if (playerVelocity.magnitude == 0){
            anim.CrossFade("cur_Breathing", 0.1f);
        }
        else{
            if (isZooming)
                anim.CrossFade("cur_Running", 0.1f);
            else
                anim.CrossFade("cur_Walking02", 0.1f);
        }

        playerVelocity = transform.TransformDirection(playerVelocity);
        if (status.GetHealth() < 20) {
            playerVelocity *= sleepySpeed;
        } else {
            playerVelocity *= moveSpeed;
        }

        // Jumping
        if (controller.isGrounded && Input.GetButtonDown("Jump"))
        {
            yVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        yVelocity += gravity * Time.deltaTime;
        playerVelocity.y = yVelocity;

        float moveHorz = Input.GetAxis("Horizontal");
        if (moveHorz > 0)
            rotateDirection = new Vector3(0, 1, 0);
        else if (moveHorz < 0)
            rotateDirection = new Vector3(0, -1, 0);
        else
            rotateDirection = Vector3.zero;

        controller.transform.Rotate(rotateDirection, rotateSpeed * Time.deltaTime);
        controller.Move(playerVelocity * Time.deltaTime);
    }

    private IEnumerator AttackCoroutine() {
        print("hi");
        isAttacking = true; 
        anim.CrossFade("cur_EatingCycle", 0.1f);
        anim["cur_EatingCycle"].speed = 1.0f;
        yield return new WaitForSeconds(anim["cur_EatingCycle"].length * 0.5f); 

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, wiggleDistance);
        foreach (Collider hitCollider in hitColliders) {
            FoodController food = hitCollider.GetComponent<FoodController>();
            if (food != null) {
                food.EatFood(attackDamage, status);
                break;
            }
        }

        yield return new WaitForSeconds(anim["cur_EatingCycle"].length * 0.5f);
        isAttacking = false; 
    }
    private IEnumerator ZoomCoroutine() {
        isZooming = true;
        status.ApplyDamage((int)zoomEnergyCost); // take away energy
        float originalSpeed = moveSpeed;

        moveSpeed *= zoomSpeedMultiplier;
        anim.CrossFade("cur_Running", 0.1f); // Use run animation if desired

        yield return new WaitForSeconds(zoomDuration);

        moveSpeed = originalSpeed;
        isZooming = false;
    }
    private IEnumerator WiggleCoroutine() {
        isWiggling = true;
        anim.CrossFade("cur_WigglingTail", 0.1f); 

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, wiggleDistance);
        foreach (Collider hitCollider in hitColliders) {
            AIStatus enemyStatus = hitCollider.GetComponent<AIStatus>();
            if (enemyStatus != null && enemyStatus.isAlive()) {
                enemyStatus.Stun(wiggleDuration);
            }
        }

        yield return new WaitForSeconds(1);
        isWiggling = false;
    }

    private IEnumerator AttackEnemiesCoroutine() {
        isAttacking = true; 
        if(loseHairUsed == false){
            anim.CrossFade("cur_AngryCycle", 0.1f);
            anim["cur_AngryCycle"].speed = 1.0f;

            yield return new WaitForSeconds(anim["cur_AngryCycle"].length * 0.5f);

            Collider[] hitColliders = Physics.OverlapSphere(transform.position, loseHairDistance);
            foreach (Collider hitCollider in hitColliders) {
                AIStatus enemyStatus = hitCollider.GetComponent<AIStatus>();
                if (enemyStatus != null && enemyStatus.isAlive()) {
                    enemyStatus.ApplyDamage(1000);
                }
            }

        yield return new WaitForSeconds(anim["cur_AngryCycle"].length * 0.5f);
        loseHairUsed = true;
        }
        isAttacking = false; 
    }
    private IEnumerator EatFoodCoroutine() {
        isAttacking = true; 
        anim.CrossFade("cur_EatingCycle", 0.1f);
        anim["cur_EatingCycle"].speed = 1.0f;
        
        yield return new WaitForSeconds(anim["cur_EatingCycle"].length * 0.5f);

        //makes sure garbage is detencted so it dissapears before food
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, eatDistance);
        foreach (Collider hitCollider in hitColliders) {
            GarbageController garbage = hitCollider.GetComponent<GarbageController>();
            if (garbage != null) {
                garbage.garbageFound(attackDamage, status);
                break;
            }

            HollyController holly = hitCollider.GetComponent<HollyController>();
            if (holly != null) {
                holly.HollyFound(attackDamage, status);
                break;
            }

            EnergyBarController energyBar = hitCollider.GetComponent<EnergyBarController>();
            if (energyBar != null) {
                energyBar.energyFound(attackDamage, status);
                break;
            }

            FoodController food = hitCollider.GetComponent<FoodController>();
            if (food != null) {
                food.EatFood(attackDamage, status);
                break;
            }
        }

        yield return new WaitForSeconds(anim["cur_EatingCycle"].length * 0.5f);
        isAttacking = false; 
    }

    //ends all actions incase one was being performed upon death
    public void ForceStopActions(){
        StopAllCoroutines(); 
        isAttacking = false;
        isWiggling = false;
        isZooming = false;
        moveSpeed = defaultMoveSpeed;
        yVelocity = 0;
        playerVelocity = Vector3.zero;
        anim.CrossFade("cur_Breathing", 0.1f); 
    }
}