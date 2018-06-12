using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HellephantBullet : MonoBehaviour {
	
	public float speed = 600.0f;
	public float life = 3;
	public ParticleSystem normalTrailParticles;
	public ParticleSystem ImpactParticles;
	public int damage = 20;
	public Color bulletColor;
	public AudioClip hitSound;

	Vector3 velocity;
    Vector3 force;
	Vector3 newPos;
	Vector3 oldPos;
	Vector3 direction;
	bool hasHit = false;
	RaycastHit lastHit;
	// Reference to the audio source.
	AudioSource bulletAudio;  
	float timer;

	void Awake() {
		bulletAudio = GetComponent<AudioSource> ();
	}

	void Start() {
		newPos = transform.position;
		oldPos = newPos;

		// Set our particle colors.
		var main = normalTrailParticles.main;
		main.startColor = bulletColor;
		main = ImpactParticles.main;
		main.startColor = bulletColor;
		normalTrailParticles.gameObject.SetActive(true);
	}

	void Update() {
		if (hasHit) {
			return;
		}

		// Add the time since Update was last called to the timer.
		timer += Time.deltaTime;

		// Schedule for destruction if bullet never hits anything.
		if (timer >= life) {
			Dissipate();
		}

        velocity = transform.forward;
		//velocity.y = 0;
		velocity = velocity.normalized * speed;

		// assume we move all the way
		newPos += velocity * Time.deltaTime;
	
		// Check if we hit anything on the way
		direction = newPos - oldPos;
		float distance = direction.magnitude;

		if (distance > 0) {
            RaycastHit[] hits = Physics.RaycastAll(oldPos, direction, distance);

		    // Find the first valid hit
		    for (int i = 0; i < hits.Length; i++) {
		        RaycastHit hit = hits[i];

				if (ShouldIgnoreHit(hit)) {
					continue;
				}

				// notify hit
				OnHit(hit);

				lastHit = hit;

				if (hasHit) {
					newPos = hit.point;
					break;
				}
		    }
		}

		oldPos = transform.position;
		transform.position = newPos;
	}

	/**
	 * So we don't hit the same enemy twice with the same raycast when we have
	 * piercing shots. The shot can still bounce on a wall, come back and hit
	 * the enemy again if we have both bouncing and piercing shots.
	 */
	bool ShouldIgnoreHit (RaycastHit hit) {
		if (lastHit.point == hit.point || lastHit.collider == hit.collider || hit.collider.tag == "Enemy")
			return true;
		
		return false;
	}

	/**
	 * Figure out what to do when we hit something.
	 */
	void OnHit(RaycastHit hit) {
        Quaternion rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);

        if (hit.transform.tag == "Environment") {
			newPos = hit.point;
			ImpactParticles.transform.position = hit.point;
			ImpactParticles.transform.rotation = rotation;
			ImpactParticles.Play();
			hasHit = true;
			bulletAudio.clip = hitSound;
			bulletAudio.volume = 0.5f;
			bulletAudio.pitch = Random.Range(0.6f, 0.8f);
			bulletAudio.Play();
			DelayedDestroy();
        }

        if (hit.transform.tag == "Player") {
			ImpactParticles.transform.position = hit.point;
			ImpactParticles.transform.rotation = rotation;
			ImpactParticles.Play();

			// Try and find an EnemyHealth script on the gameobject hit.
			PlayerHealth playerHealth = hit.collider.GetComponent<PlayerHealth>();
			
			// If the EnemyHealth component exist...
			if (playerHealth != null) {
				// ... the enemy should take damage.
				playerHealth.TakeDamage(damage);
			}
    		hasHit = true;
			DelayedDestroy();
			bulletAudio.clip = hitSound;
			bulletAudio.volume = 0.5f;
			bulletAudio.pitch = Random.Range(0.6f, 0.8f);
			bulletAudio.Play();
        }
	}

	// Just a method for destroying the game object, but which
	// first detaches the particle effect and leaves it for a
	// second. Called if the bullet end its life in midair
	// so we get an effect of the bullet fading out instead
	// of disappearing immediately.
	void Dissipate() {
		var normalTrailParticlesEmissions = normalTrailParticles.emission.enabled;
		normalTrailParticlesEmissions = false;
		normalTrailParticles.transform.parent = null;
		var main = normalTrailParticles.main;
		Destroy(normalTrailParticles.gameObject, main.duration);
		Destroy(gameObject);
	}

	void DelayedDestroy() {
		normalTrailParticles.gameObject.SetActive(false);
		Destroy(gameObject, hitSound.length);
	}
}