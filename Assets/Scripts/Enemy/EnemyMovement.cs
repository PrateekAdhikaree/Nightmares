using UnityEngine;
using System.Collections;

public class EnemyMovement : MonoBehaviour {

	// A layer mask so the raycast only hits things on the shootable layer.
	public LayerMask shootableMask;  
	public float roamSpeed = 1.5f;
	public float attackSpeed = 4;

	// Reference to the player's position.
	Transform player;       
	// Reference to the player's health.
	PlayerHealth playerHealth;     
	// Reference to this enemy's health.
	EnemyHealth enemyHealth;     
	// Reference to the nav mesh agent.
	UnityEngine.AI.NavMeshAgent nav;   
	// Reference to the animator.
	Animator anim;
	// Reference the renderer.
	SkinnedMeshRenderer myRenderer;
	// A ray from the gun end forwards.
	Ray shootRay;          
	// A raycast hit to get information about what was hit.
	RaycastHit shootHit;    
	Vector3 position;
	bool hasValidTarget = false;
	bool foundPlayer = false;

	void Awake() {
		// Set up the references.
		player = GameObject.FindGameObjectWithTag ("Player").transform;
		playerHealth = player.GetComponent<PlayerHealth> ();
		enemyHealth = GetComponent <EnemyHealth> ();
		nav = GetComponent <UnityEngine.AI.NavMeshAgent> ();
		anim = GetComponent<Animator>();
		myRenderer = GetComponentInChildren<SkinnedMeshRenderer>();

		SetRandomNavTarget();
	}
	
	
	void Update() {
		// If this enemy is alive.
		if (enemyHealth.currentHealth > 0) {
			foundPlayer = false;

			// Control the speed of the walk animation with the velocity we're moving at.
			//print ("Speed: " + nav.speed + " Velocity: " + nav.velocity.magnitude);
			float currentSpeed = nav.velocity.magnitude;
            anim.speed = currentSpeed;

			Vector3 distanceFromTarget = position - transform.position;

			if (playerHealth.currentHealth > 0) {
				// Shoot a ray from our eye level towards the players eye level.
				Vector3 direction = (player.position + new Vector3(0, 1, 0)) - (transform.position + new Vector3(0, 1, 0));
				shootRay.origin = transform.position + new Vector3(0, 1, 0);
				shootRay.direction = direction;
				if (Physics.Raycast (shootRay, out shootHit, 25, shootableMask)) {
					if (shootHit.transform.tag == "Player") {
						position = player.position;
						hasValidTarget = true;
						foundPlayer = true;
						//myRenderer.materials[1].color = Color.Lerp(myRenderer.materials[1].color, new Color(1, 0, 0, 0), 2 * Time.deltaTime);
						myRenderer.materials[1].SetColor("_RimColor", Color.Lerp(myRenderer.materials[1].GetColor("_RimColor"), new Color(1, 0, 0, 1), 2 * Time.deltaTime));
						nav.speed = attackSpeed;
					}
					else {
						//myRenderer.materials[1].color = Color.Lerp(myRenderer.materials[1].color, new Color(0, 0, 0, 0), 2 * Time.deltaTime);
						myRenderer.materials[1].SetColor("_RimColor", Color.Lerp(myRenderer.materials[1].GetColor("_RimColor"), new Color(0, 0, 0, 1), 2 * Time.deltaTime));
					}
				}
			}

			if (!foundPlayer) {
				if (distanceFromTarget.magnitude < 1 || !hasValidTarget) {
					SetRandomNavTarget();
				}
				nav.speed = roamSpeed;
				//myRenderer.materials[1].color = Color.Lerp(myRenderer.materials[1].color, new Color(0, 0, 0, 0), 2 * Time.deltaTime);
				myRenderer.materials[1].SetColor("_RimColor", Color.Lerp(myRenderer.materials[1].GetColor("_RimColor"), new Color(0, 0, 0, 1), 2 * Time.deltaTime));
			}

			if (hasValidTarget) {
				nav.SetDestination(position);
			}
		}
		// Otherwise...
		else {
			anim.speed = 1;

			// ... disable the nav mesh agent.
			nav.enabled = false;

			//myRenderer.materials[1].color = Color.Lerp(myRenderer.materials[1].color, new Color(0, 0, 0, 0), 2 * Time.deltaTime);
			myRenderer.materials[1].SetColor("_RimColor", Color.Lerp(myRenderer.materials[1].GetColor("_RimColor"), new Color(0, 0, 0, 1), 2 * Time.deltaTime));
		}
	}

	void SetRandomNavTarget() {
		Vector3 randomPosition = Random.insideUnitSphere * 30;
		randomPosition.y = 0;
		randomPosition += transform.position;
		UnityEngine.AI.NavMeshHit hit;
		hasValidTarget = UnityEngine.AI.NavMesh.SamplePosition(randomPosition, out hit, 5, 1);
		Vector3 finalPosition = hit.position;
		position = finalPosition;
    }
}