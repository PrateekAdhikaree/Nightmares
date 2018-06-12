using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class WaveManager : MonoBehaviour {
	
	// Reference to the player's heatlh.
	public PlayerHealth playerHealth;   
	// The distance from our Camera View Frustrum we want to spawn enemies
	// to make sure they are not visisble when they spawn. I'm too lazy to
	// do any proper checks.
	public float bufferDistance = 200;
	// The time in seconds between each wave.
	public float timeBetweenWaves = 5f;
    // The time in seconds between each spawn in a wave.
    public float spawnTime = 3f;
    // The wave to start on.
	public int startingWave = 1;
    // The difficulty to start on.
	public int startingDifficulty = 1;
	// Reference to the waves Text component.
	public Text number;
	// Reference to the enemies Text component.
	public Text numberEnemies; 
    // The number of enemies left alive for the current wave.
	[HideInInspector]
	public int enemiesAlive = 0;

    // A class depicting one wave with x number of entries.
    [System.Serializable]
    public class Wave {
        public Entry[] entries;

        // A class depicting one wave entry.
        [System.Serializable]
        public class Entry {
            // The enemy type to spawn.
            public GameObject enemy;
            // The number of enemies to spawn.
            public int count;
            // A counter telling us how many have been spawned so far.
            [System.NonSerialized]
            public int spawned;
        }
    }

    // All our waves.
    public Wave[] waves;

    // Misc private variables needed to make everything work.
    Vector3 spawnPosition = Vector3.zero;
	int waveNumber;
	float timer; 
	Wave currentWave;
	int spawnedThisWave = 0;
	int totalToSpawnForWave;
	bool shouldSpawn = false;
	int difficulty;
	int enemiesInLastFrame;
	float timeSinceNoEnemiesKilled;

	void Start() {
		// Let us start on a higher wave and difficulty if we wish.
		waveNumber = startingWave > 0 ? startingWave - 1 : 0;
		difficulty = startingDifficulty;

		// Start the next, ie. the first wave.
		StartCoroutine("StartNextWave");
	}
	
	void Update() {
		// This is false while we're setting up the next wave.
		if (!shouldSpawn) {
			return;
        }

		// Start the next wave when we've spawned all our enemies and the player
		// has killed them all or that there were no enemies killed in last 20 secs
		// so we can assume that enemy could not be found and next wave is started.
		if (spawnedThisWave == totalToSpawnForWave && (enemiesAlive == 0 || (enemiesAlive == enemiesInLastFrame && timeSinceNoEnemiesKilled > 20))) {
			StartCoroutine("StartNextWave");
			return;
		}

        // Add the time since Update was last called to the timer.
		timer += Time.deltaTime;

        // If the timer exceeds the time between spawns, we check if we need to spawn an enemy and then spawn it.
		if (timer >= spawnTime) {
			// Spawn one enemy from each of the entries in this wave.
            // The difficulty multiplies the number of spawned enemies for each
            // "loop", that is each full run through all the waves.
			foreach (Wave.Entry entry in currentWave.entries) {
				if (entry.spawned < (entry.count * difficulty)) {
					Spawn(entry);
				}
			}
		}

		numberEnemies.text = enemiesAlive.ToString();

		// Updating time since last enemy was killed
		if (enemiesInLastFrame == enemiesAlive) {
			timeSinceNoEnemiesKilled += Time.deltaTime;
		} else {
			enemiesInLastFrame = enemiesAlive;
			timeSinceNoEnemiesKilled = 0;
		}
	}

	public void playEnemyTextAnimation(){
		numberEnemies.GetComponent<Animation>().Play();
	}

	/**
	 * Sets the new wave and its requirements
	 */
	IEnumerator StartNextWave() {
		shouldSpawn = false;

		yield return new WaitForSeconds(timeBetweenWaves);

		if (waveNumber < waves.Length) {
			currentWave = waves[waveNumber];
		} else {
			difficulty++;

			// resetting the spawned variable in the last defined wave
			foreach (Wave.Entry entry in waves [waves.Length - 1].entries) {
				entry.spawned = 0;
			}

			currentWave = waves [waves.Length - 1];
		}

        // The difficulty multiplies the number of spawned enemies for each
        // "loop", that is each full run through all the waves.
        totalToSpawnForWave = 0;
		foreach (Wave.Entry entry in currentWave.entries) {
			totalToSpawnForWave += (entry.count * difficulty);
		}

		spawnedThisWave = 0;
		shouldSpawn = true;

		waveNumber++;

		number.text = waveNumber.ToString ();//actualWaveNumber == 0 ? ((waveNumber + ((difficulty - 1) * waves.Length)).ToString()) : actualWaveNumber.ToString();
		number.GetComponent<Animation>().Play();
	}

	/**
	 * Spawn enemies.
 	 * 
	 * This method is called at regular intervals, but all the ways this function 
	 * can end up not spawning an enemy means it could be many intervals between each 
	 * actual spawn and our enemies will spawn very irregularly. That just 
	 * makes it seem more random.
	 */
	void Spawn(Wave.Entry entry) {
		// Reset the timer.
		timer = 0f;
		
		// If the player has no health left, stop spawning.
		if (playerHealth.currentHealth <= 0f) {
			return;
		}
		
		// Find a random position roughly on the level.
		Vector3 randomPosition = Random.insideUnitSphere * 35;
		randomPosition.y = 0;
		
		// Find the closest position on the nav mesh to our random position.
		// If we can't find a valid position return and try again.
		UnityEngine.AI.NavMeshHit hit;
		if (!UnityEngine.AI.NavMesh.SamplePosition(randomPosition, out hit, 5, 1)) {
			return;
		}
		
		// We have a valid spawn position on the nav mesh.
		spawnPosition = hit.position;
		
		// Check if this position is visible on the screen, if it is we
		// return and try again.
		Vector3 screenPos = Camera.main.WorldToScreenPoint(spawnPosition);
		if ((screenPos.x > -bufferDistance && screenPos.x < (Screen.width + bufferDistance)) && 
		    (screenPos.y > -bufferDistance && screenPos.y < (Screen.height + bufferDistance))) 
		{
			return;
		}

		// We passed all the checks, spawn our enemy.
		GameObject enemy =  Instantiate(entry.enemy, spawnPosition, Quaternion.identity) as GameObject;
		// Multiply health and score value by the current difficulty.
		enemy.GetComponent<EnemyHealth>().startingHealth *= difficulty;
		enemy.GetComponent<EnemyHealth>().scoreValue *= difficulty;
		
		entry.spawned++;
		spawnedThisWave++;
		enemiesAlive++;
		numberEnemies.text = enemiesAlive.ToString();
	}
}
