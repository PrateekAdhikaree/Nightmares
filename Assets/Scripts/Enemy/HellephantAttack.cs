using UnityEngine;
using System.Collections;

public class HellephantAttack : MonoBehaviour {

	// The time in seconds between each attack.
	public float timeBetweenAttacks = 3f;  
	public int bulletsPerVolley = 5;
	public float timeBetweenBullets = 0.1f;
	public int numberOfBullets = 36;
	public float angleBetweenBullets = 10f;
	public GameObject bullet;
	public int attacksPerSpecialAttack = 3;
	public AudioClip shootClip;
	public AudioClip specialClip;
	   
	// Reference to the player GameObject.
	GameObject player;           
	// Reference to this enemy's health.
	EnemyHealth enemyHealth; 
	// Whether player is within the trigger collider and can be attacked.
	bool playerInRange;   
	// Reference to the animator.
	Animator anim;
	// Timer for counting up to the next attack.
	float attackTimer;
	float bulletTimer;
	int attackCount;
	int bulletCount;
	HellephantMovement helleMovement;
	float floatHeight = 3f;
	float landingTime;
	bool usedSpecial = false;
	bool landed = false;
	// Reference to the audio source.
	AudioSource enemyAudio;  
	
	void Awake() {
		// Setting up the references.
		player = GameObject.FindGameObjectWithTag("Player");
		enemyHealth = GetComponent<EnemyHealth>();
		enemyAudio = GetComponent<AudioSource>();
		anim = GetComponent<Animator>();
		helleMovement = GetComponent<HellephantMovement>();
	}

	void Start() {
		// Make sure we start in the air when we're spawned from the wave manager.
		transform.position = new Vector3(transform.position.x, floatHeight, transform.position.z);
	}

	void Update() {
		// The time between our attacks.
		attackTimer += Time.deltaTime;
		// If the timer exceeds the time between attacks, the player is in range and this enemy is alive attack.
		if (attackTimer > timeBetweenAttacks && enemyHealth.currentHealth > 0) {
			Attack();
		}
	}
	
	void Attack() {
		// The time between each bullet in our normal attack.
		bulletTimer += Time.deltaTime;

		if (attackCount < attacksPerSpecialAttack) {
			if (bulletTimer > timeBetweenBullets && bulletCount < bulletsPerVolley) {
				Vector3 relativePos = player.transform.position - transform.position;
				Quaternion rotation = Quaternion.LookRotation(relativePos);
				Quaternion rot = rotation * Quaternion.AngleAxis(Random.Range(-5.0f, 5.0f), Vector3.up) * Quaternion.AngleAxis(Random.Range(-5.0f, 5.0f), Vector3.right);
				Instantiate(bullet, transform.position + new Vector3(0, 0.5f, 0), rot);

				// Play shooting sound.
				enemyAudio.clip = shootClip;
				enemyAudio.Play();

				// Reset the timer.
				bulletTimer = 0f;
				bulletCount++;

				if (bulletCount == (bulletsPerVolley)) {
					bulletCount = 0;
					attackTimer = 0;
					attackCount++;
				}
			}
		}
		else {
			SpecialAttack();
		}
	}

	void SpecialAttack() {
		// Start landing.
		if (!landed) {
			anim.SetBool("Landing", true);
			helleMovement.shouldMove = false;
			landingTime += Time.deltaTime * 5f;
			transform.position = new Vector3(transform.position.x, Mathf.Lerp(floatHeight, 0, landingTime), transform.position.z);
		}
		// Start lifting.
		else {
			anim.SetBool("Landing", false);
			helleMovement.shouldMove = true;
			landingTime += Time.deltaTime * 2f;
			transform.position = new Vector3(transform.position.x, Mathf.Lerp(0, floatHeight, landingTime), transform.position.z);
		}

		// When we've landed we fire bullets in all directions.
		if (transform.position.y == 0) {
			landed = true;
			if (!usedSpecial) {
				for (int i = 0; i < numberOfBullets; i++) {
					// Make sure our bullets spread out in an even pattern.
					float angle = i * angleBetweenBullets - ((angleBetweenBullets / 2) * (numberOfBullets - 1));
					Quaternion rot = transform.rotation * Quaternion.AngleAxis(angle, Vector3.up);
					Instantiate(bullet, transform.position + new Vector3(0, 0.5f, 0), rot);
				}

				// Play special sound.
				enemyAudio.clip = specialClip;
				enemyAudio.Play();

				usedSpecial = true;
				// Reset the attack timer so we stay on the ground for one whole cycle
				// before lifting back up.
				attackTimer = 0;
				landingTime = 0;
			}
		}

		if (transform.position.y == floatHeight) {
			attackCount = 0;
			landed = false;
			usedSpecial = false;
			landingTime = 0;
		}
	}
}