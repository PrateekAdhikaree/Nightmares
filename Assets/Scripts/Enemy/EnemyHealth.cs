using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EnemyHealth : MonoBehaviour {

	// The amount of health the enemy starts the game with.
	public int startingHealth = 100;  
	// The current health the enemy has.
	[HideInInspector]
	public int currentHealth;  
	// The speed at which the enemy sinks through the floor when dead.
	public float sinkSpeed = 2.5f;   
	// The amount added to the player's score when the enemy dies.
	public int scoreValue = 10; 
	// The sound to play when the enemy dies.
	public AudioClip deathClip;    
	// The sound to play when the enemy burns up.
	public AudioClip burnClip;  
	// The particle system to play when the enemy is burning
	public ParticleSystem deathParticles;  
	// The health bar we display over our head.
	public Slider healthBarSlider;
	// We spawn two eyes when we die. Gibs, you know. :p
	public GameObject eye;
	
	// The health bar slider instance for this enemy.
	Slider sliderInstance;
	// Whether the enemy is dead.
	bool isDead;
	// Whether the enemy is burning.
	bool isBurning = false;
	// The rim color for our shader. We change this to simulate a red hit effect.
	Color rimColor;
    // Changing the rim power as well produces a better effect.
    float rimPower;
    // The cutoff value for our dissolve shader. We change this to dissolve our
    // body when we burn up.
    float cutoffValue = 0f;
	// Components and scripts we need references to.
	Animator anim;            
	AudioSource enemyAudio;        
	CapsuleCollider capsuleCollider;   
	SkinnedMeshRenderer myRenderer;
	GameObject enemyHealthbarManager;
	WaveManager waveManager;
	ScoreManager scoreManager;
	PickupManager pickupManager;

	void Awake() {
		// Setting up the references.
		anim = GetComponent<Animator>();
		enemyAudio = GetComponent<AudioSource>();
		capsuleCollider = GetComponent<CapsuleCollider>();
		myRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
		enemyHealthbarManager = GameObject.Find("EnemyHealthbarsCanvas");
		waveManager = GameObject.Find("WaveManager").GetComponent<WaveManager>();
		scoreManager = GameObject.Find("ScoreManager").GetComponent<ScoreManager>();
		pickupManager = GameObject.Find("PickupManager").GetComponent<PickupManager>();
	}

	void Start() {
		// Setting the current health when the enemy first spawns.
		currentHealth = startingHealth;

		// Instantiate our health bar GUI slider.
		sliderInstance = Instantiate(healthBarSlider, gameObject.transform.position, Quaternion.identity) as Slider;
		sliderInstance.gameObject.transform.SetParent(enemyHealthbarManager.transform, false);
		sliderInstance.GetComponent<Healthbar>().enemy = gameObject;
		sliderInstance.gameObject.SetActive(false);

		// Get the current rim color and rim power from our material.
		rimColor = myRenderer.materials[0].GetColor("_RimColor");
        rimPower = myRenderer.materials[0].GetFloat("_RimPower");
    }

	void Update() {
		// If we are burning we update the cutoff value for our materials so they
		// gradually dissolve over time.
		if (isBurning) {
			cutoffValue = Mathf.Lerp(cutoffValue, 1f, 1.3f * Time.deltaTime);
			myRenderer.materials[0].SetFloat("_Cutoff", cutoffValue);
			myRenderer.materials[1].SetFloat("_Cutoff", 1);
		}
	}

	public void TakeDamage(int amount, Vector3 hitPoint) {
        StopCoroutine("Ishit");
        StartCoroutine("Ishit");

		// If the enemy is dead there's no need to take damage so exit the function.
		if (isDead)
			return;

		GetComponent<Rigidbody>().AddForceAtPosition(transform.forward * -300, hitPoint);
		
		// Reduce the current health by the amount of damage sustained.
		currentHealth -= amount;

		// Set the health bar's value to the current health.
		if (currentHealth <= startingHealth) {
			sliderInstance.gameObject.SetActive(true);
		}
		int sliderValue = (int) Mathf.Round(((float)currentHealth / (float)startingHealth) * 100);
		sliderInstance.value = sliderValue;
		
		// If the current health is less than or equal to zero the enemy is dead.
		if (currentHealth <= 0) {
			Death();
		}
	}

	IEnumerator Ishit() {
		Color newColor = new Color(10, 0, 0, 0);
        float newPower = 0.5f;

		myRenderer.materials[0].SetColor("_RimColor", newColor);
        myRenderer.materials[0].SetFloat("_RimPower", newPower);

        float time = 0.25f;
		float elapsedTime = 0;
		while (elapsedTime < time) {
			newColor = Color.Lerp(newColor, rimColor, elapsedTime / time);
			myRenderer.materials[0].SetColor("_RimColor", newColor);
            newPower = Mathf.Lerp(newPower, rimPower, elapsedTime / time);
            myRenderer.materials[0].SetFloat("_RimPower", newPower);
            elapsedTime += Time.deltaTime;
			yield return null;
		}
        myRenderer.materials[0].SetColor("_RimColor", rimColor);
        myRenderer.materials[0].SetFloat("_RimPower", rimPower);
    }

	void Death() {
		// The enemy is dead.
		isDead = true;

		// Tell the animator that the enemy is dead.
		anim.SetTrigger("Dead");
		
		// Change the audio clip of the audio source to the death clip and play it (this will stop the hurt clip playing).
		enemyAudio.clip = deathClip;
		enemyAudio.Play();

		// Find and disable the Nav Mesh Agent.
		if (GetComponent<UnityEngine.AI.NavMeshAgent>()) {
			GetComponent<UnityEngine.AI.NavMeshAgent>().enabled = false;
		}
		
		// Find the rigidbody component and make it kinematic (since we use Translate to sink the enemy).
		GetComponent<Rigidbody>().isKinematic = true;

		// Increase the score by the enemy's score value.
		scoreManager.AddScore(scoreValue);

		waveManager.enemiesAlive--;

		// Turn the collider into a trigger so shots can pass through it.
		capsuleCollider.isTrigger = true;

		// Remove this object.
		StartCoroutine(StartSinking());
		waveManager.playEnemyTextAnimation ();

		Destroy(sliderInstance.gameObject);
	}

	IEnumerator StartSinking() {
		yield return new WaitForSeconds(2);

		isBurning = true;

		// And play the particles.
		deathParticles.Play();

		enemyAudio.clip = burnClip;
		enemyAudio.Play();

		// Spawn two eyes.
		for (int i = 0; i < 2; i++) {
			GameObject instantiatedEye = Instantiate(eye, transform.position + new Vector3(0, 0.3f, 0), transform.rotation) as GameObject;
			instantiatedEye.GetComponent<Rigidbody>().velocity = transform.TransformDirection(new Vector3 (Random.Range(-5.0f, 5.0f), Random.Range(-5.0f, 5.0f), Random.Range(-5.0f, 5.0f)));
		}

		SpawnPickup();

		// After 2 seconds destory the enemy.
		Destroy(gameObject, 3f);
	}

	/** 
	 * Chance to spawn a pickup on death.
	 */
	void SpawnPickup() {
		// We spawn our pickups slightly above the ground.
		Vector3 spawnPosition = transform.position + new Vector3(0, 0.3f, 0);

		// Spawn an extra bullet pickup when we reach a certain score.
		if (scoreManager.GetScore () >= pickupManager.scoreNeededForExtraBullet) {
			Instantiate (pickupManager.bulletPickup, spawnPosition, transform.rotation);

			// Increase the score needed to spawn a pickup.
			pickupManager.scoreNeededForExtraBullet += pickupManager.extraScoreNeededAfterEachPickup;
		} else {
			// Spawns one of our 3 powerup pickups randomly.
			// It's set up to spawn a pickup 20% of the time.
			// And the pickups are selected accordingly:
			// - 30% of the time it will be a bounce pickup
			// - 20% of the time it will be a pierce pickup
			// - 50% of the time it will be a health pickup
			float rand = Random.value;
			if (rand <= 0.2f) {
				// Bounce.
				if (rand <= 0.06f) {
					Instantiate (pickupManager.bouncePickup, spawnPosition, transform.rotation);
				}
			// Pierce.
			else if (rand > 0.06f && rand <= 0.1f) {
					Instantiate (pickupManager.piercePickup, spawnPosition, transform.rotation);
				}
			// Health.
			else {
					Instantiate (pickupManager.healthPickup, spawnPosition, transform.rotation);
				}
			}
		}
	}
}
