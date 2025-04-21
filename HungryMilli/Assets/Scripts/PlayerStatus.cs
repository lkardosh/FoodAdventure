using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class PlayerStatus : MonoBehaviour {
	
	//declarations
	private bool gameOver = false;
	public Texture2D backgroundImage;
	private int	energy = 100;
	private int maxEnergy = 100;

	private bool hasStarted = false;
	private float energyTickTimer = 0f;
	private float tickInterval = 1f; 

	public int numLevel = 1;
	private Vector3 originalPosition;
	private int numLives = 3;
	private bool dead = false;
	public int foodCount = 0;

	private PlayerController playerController;
	private Animation anim; 
	
	public void AddFoodCount(){
		foodCount++;
	}
	
	public float GetHealth(){
		return energy;
	}
	public float GetNumLives(){
		return numLives;
	}

    void Start()
    {
        playerController = GetComponent<PlayerController>();
		anim = GetComponent<Animation>();
		Time.timeScale = 0f;
		originalPosition = transform.position; 
    }

    public bool isAlive() {
		return !dead;
	}
	//increases energy, used when milli eats, or interacts with holly
	public void IncreaseEnergy(int amount) {
		energy += amount;
		if (energy > maxEnergy) energy = maxEnergy;
	}
	public void ApplyDamage(int damage){
		if (dead) return; 
		energy -= damage;
		// Debug.Log("Ouch! " + health);
		if (energy <= 0){
			energy = 0;
			StartCoroutine(Die());
		}
	}
    public void AddLife(){
		numLives++;
	}
	IEnumerator Die() {
	if (dead) yield break;

	dead = true;
	numLives--;
	playerController.ForceStopActions(); 
	playerController.controller.enabled = false;

	Debug.Log("Oh no, you died :(");
	HideCharacter();
	anim.CrossFade("cur_Breathing", 0.1f);

	if (numLives <= 0) {
		yield return new WaitForSeconds(2f);
		anim.CrossFade("cur_Breathing", 0.1f); 
		EndGame(false);
		yield break;
	}

	yield return new WaitForSeconds(3f); // wait before respawn

	Debug.Log("You've returned from the dead!");
	Debug.Log("Original position: " + originalPosition);

	// Move Gmilli back to the original spot
	transform.position = originalPosition;
	playerController.controller.enabled = true;

	//sets up energt again
	energy = maxEnergy;
	dead = false;
	anim.CrossFade("cur_Breathing", 0.1f);

	 // re-enable playercontrols 
	ShowCharacter();  
}


	
	void HideCharacter(){	
		playerController.IsControllable = false;
	}
	
	void ShowCharacter(){
		playerController.IsControllable = true;
	}

	public int GetFoodCount() {
		return foodCount;
	}
	public int GetNumLevel(){
		int myFood = GetFoodCount();
		if(myFood<5){
			return 1;
		}
		else if(myFood>=10){
			return 3;
		}
		else{
			return 2;
		}
	}
	void Update() {
		if (!hasStarted || dead || gameOver) return;

		energyTickTimer += Time.deltaTime;

		if (energyTickTimer >= tickInterval) {
			energyTickTimer = 0f;

			// lose 1 energy per second
			energy -= 1; 

			if (energy <= 0) {
				energy = 0;
				StartCoroutine(Die());
			}
		}

		// win condition check
		if (foodCount >= 15) {
			EndGame(true);
		}
	}

	void EndGame(bool win) {
		gameOver = true;
		HideCharacter();
		anim.CrossFade("idle", 0.1f); // stop on idle animation

		if (win) {
			Debug.Log("You collected all the food! Milli is finally satified..... for now");
		} else {
			Debug.Log("Milli was caught too many times, the humans have hidden all the food :(");
		}

		Time.timeScale = 0; 
	}
	void OnGUI() {
		if (!hasStarted) {
        // Draw the full-screen background image
        if (backgroundImage != null) {
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), backgroundImage, ScaleMode.StretchToFill);
        }

        // Big title
        GUIStyle titleStyle = new GUIStyle(GUI.skin.label);
        GUIStyle welcomeStyle = new GUIStyle(GUI.skin.label);
        welcomeStyle.fontSize = 44;
        welcomeStyle.alignment = TextAnchor.MiddleCenter;
        welcomeStyle.normal.textColor = Color.black;
		welcomeStyle.fontStyle = FontStyle.Bold;
        titleStyle.fontSize = 50;
        titleStyle.alignment = TextAnchor.MiddleCenter;
        titleStyle.normal.textColor = Color.black;
		titleStyle.fontStyle = FontStyle.Bold;

		//removes white when hovering over
		Color textColor = Color.black;
		titleStyle.normal.textColor = textColor;
		titleStyle.hover.textColor = textColor;
		titleStyle.active.textColor = textColor;
		titleStyle.focused.textColor = textColor;

		welcomeStyle.normal.textColor = textColor;
		welcomeStyle.hover.textColor = textColor;
		welcomeStyle.active.textColor = textColor;
		welcomeStyle.focused.textColor = textColor;

        GUI.Label(new Rect(Screen.width / 2 - 300, Screen.height / 2 - 160, 600, 60), "Welcome to", welcomeStyle);
        GUI.Label(new Rect(Screen.width / 2 - 300, Screen.height / 2 - 100, 600, 80), "Milli’s Food Adventure!", titleStyle);

		GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
		buttonStyle.fontSize = 18;
		buttonStyle.fontStyle = FontStyle.Bold;
		buttonStyle.normal.textColor = Color.black;
		buttonStyle.hover.textColor = Color.black;
		buttonStyle.active.textColor = Color.black;
		buttonStyle.focused.textColor = Color.black;
        // Start button
        if (GUI.Button(new Rect(Screen.width / 2 - 75, Screen.height / 2, 150, 60), "Start Game", buttonStyle)) {
            hasStarted = true;
            Time.timeScale = 1f;
            playerController.IsControllable = true;
        }

        return; // prevent game HUD from drawing
    }
		if (gameOver) {
			string message = foodCount >= 15 ? "Yay you Won!" : "Oh no you lost :(";
			
			GUIStyle style = new GUIStyle(GUI.skin.box);
			style.fontSize = 24;
			style.alignment = TextAnchor.MiddleCenter;

			GUI.Box(new Rect(Screen.width / 2 - 100, Screen.height / 2 - 60, 200, 50), message, style);

			if (GUI.Button(new Rect(Screen.width / 2 - 50, Screen.height / 2, 100, 40), "Return Home")) {
				Time.timeScale = 1;
				UnityEngine.SceneManagement.SceneManager.LoadScene(
					UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex
				);
			}
		}
	}
}
