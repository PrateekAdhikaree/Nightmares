using UnityEngine;
using System.Collections;

public class Eye : MonoBehaviour {

	// Reference to the attached particle system so we can turn it off.
	public ParticleSystem deathParticles;  

	// The cutoff value for our dissolve shader. We change this to dissolve our
	// eyes when the enemy is burning up.
	float cutoffValue = 0f;
	// A bool we set to true when we start destroying the game object.
	bool triggered = false;

	void Update () {
		// Update the cutoff value for our material so it gradually dissolves over time.
		cutoffValue = Mathf.Lerp(cutoffValue, 1f, 0.8f * Time.deltaTime);
		GetComponent<Renderer>().materials[0].SetFloat("_Cutoff", cutoffValue);

		// Nearing the end of the dissolve we start destroying the game object.
		if (cutoffValue >= 0.8f && !triggered) {
			deathParticles.Stop();
			Destroy(gameObject, 1.5f);
			triggered = true;
		}
	}
}
