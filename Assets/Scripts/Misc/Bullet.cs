using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Bullet : MonoBehaviour {
	
	public float speed = 600.0f;
	public float life = 3;
	public ParticleSystem normalTrailParticles;
	public ParticleSystem bounceTrailParticles;
	public ParticleSystem pierceTrailParticles;
	public ParticleSystem ImpactParticles;
	public int damage = 20;
	public bool piercing = false;
	public bool bounce = false;
	public Color bulletColor;
	public AudioClip bounceSound;
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
		main = bounceTrailParticles.main;
		main.startColor = bulletColor;
		main = pierceTrailParticles.main;
		main.startColor = bulletColor;
		main = ImpactParticles.main;
		main.startColor = bulletColor;

		normalTrailParticles.gameObject.SetActive(true);
		if (bounce) {
			bounceTrailParticles.gameObject.SetActive(true);
			normalTrailParticles.gameObject.SetActive(false);
			life = 1;
			speed = 20;
		}
		if (piercing) {
			pierceTrailParticles.gameObject.SetActive(true);
			normalTrailParticles.gameObject.SetActive(false);
			speed = 40;
		}
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
		velocity.y = 0;
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
		if (lastHit.point == hit.point || lastHit.collider == hit.collider)
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
			if (bounce) {
				Vector3 reflect = Vector3.Reflect(direction, hit.normal);
				transform.forward = reflect;
				bulletAudio.clip = bounceSound;
				bulletAudio.pitch = Random.Range(0.8f, 1.2f);
				bulletAudio.Play();
			}
			else {
				hasHit = true;
				bulletAudio.clip = hitSound;
				bulletAudio.volume = 0.5f;
				bulletAudio.pitch = Random.Range(1.2f, 1.3f);
				bulletAudio.Play();
				DelayedDestroy();
			}
        }

        if (hit.transform.tag == "Enemy") {
			ImpactParticles.transform.position = hit.point;
			ImpactParticles.transform.rotation = rotation;
			ImpactParticles.Play();

			// Try and find an EnemyHealth script on the gameobject hit.
			EnemyHealth enemyHealth = hit.collider.GetComponent<EnemyHealth>();
			
			// If the EnemyHealth component exist...
			if (enemyHealth != null) {
				// ... the enemy should take damage.
				enemyHealth.TakeDamage(damage, hit.point);
			}
			if (!piercing) {
            	hasHit = true;
				DelayedDestroy();
			}
			bulletAudio.clip = hitSound;
			bulletAudio.volume = 0.5f;
			bulletAudio.pitch = Random.Range(1.2f, 1.3f);
			bulletAudio.Play();
        }
	}

	// Just a method for destroying the game object, but which
	// first detaches the particle effect and leaves it for a
	// second. Called if the bullet end its life in midair
	// so we get an effect of the bullet fading out instead
	// of disappearing immediately.
	void Dissipate() {
		var normalTrailParticlesEmission = normalTrailParticles.emission.enabled;
		normalTrailParticlesEmission = false;
		normalTrailParticles.transform.parent = null;
		var normalTrailParticlesMain = normalTrailParticles.main;
		Destroy(normalTrailParticles.gameObject, normalTrailParticlesMain.duration);

		if (bounce) {
			var bounceParticlesEmission = bounceTrailParticles.emission.enabled;
			bounceParticlesEmission = false;
			bounceTrailParticles.transform.parent = null;
			var bounceTrailParticlesMain = bounceTrailParticles.main;
			Destroy(bounceTrailParticles.gameObject, bounceTrailParticlesMain.duration);
		}
		if (piercing) {
			var pierceTrailParticlesEmission = pierceTrailParticles.emission.enabled;
			pierceTrailParticlesEmission = false;
			pierceTrailParticles.transform.parent = null;
			var pierceTrailParticlesMain = pierceTrailParticles.main;
			Destroy(pierceTrailParticles.gameObject, pierceTrailParticlesMain.duration);
		}

		Destroy(gameObject);
	}

	void DelayedDestroy() {
		normalTrailParticles.gameObject.SetActive(false);
		if (bounce) {
			bounceTrailParticles.gameObject.SetActive(false);
		}
		if (piercing) {
			pierceTrailParticles.gameObject.SetActive(false);
		}
		Destroy(gameObject, hitSound.length);
	}
}