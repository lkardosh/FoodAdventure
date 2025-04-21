using UnityEngine;
using System.Collections;

public class AIStatus : MonoBehaviour {
	
	//sets player start health
	public float	health = 100.0f;
	private bool isStunned = false;

	//Uncomment if we need to apply a power-up to the AI
	private bool dead = false;
	private AIController aiController;
	
	void Start(){
		aiController = GetComponent< AIController>();
	}
	
	public float GetHealth(){
		return health;
	}
	public bool isAlive() {return !dead;}	


	public void Stun(float duration) {
		if (!isStunned) {
			StartCoroutine(StunCoroutine(duration));
		}
	}

	private IEnumerator StunCoroutine(float duration) {
		isStunned = true;
		Debug.Log(gameObject.name + " is stunned!");

		aiController.IsControllable = false;
		aiController.BeIdle(); // Play idle animation during stun

		yield return new WaitForSeconds(duration);

		isStunned = false;
		aiController.IsControllable = true;
		Debug.Log(gameObject.name + " is no longer stunned!");
	}

	public bool IsStunned() {
		return isStunned;
	}

	public void ApplyDamage(float damage){
		health -= damage;
		// Debug.Log("Enemy NPC health " + health);

		if (health <= 0 && !aiController.IsDead)
		{
			dead = true;
			health = 0;
			aiController.BeDead();
			aiController.IsDead = true;
			GameObject player = GameObject.FindWithTag("Player");
		}
	}
	void Update()
    {
		//Debug.Log("health is " + health);
    }
}
