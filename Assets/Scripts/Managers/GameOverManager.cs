using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour {

    // Reference to the player's health.
    public PlayerHealth playerHealth;
    // Time to wait before restarting the level
    public float restartDelay = 5f;

    // Reference to the animator component.
    Animator anim;
    // Timer to count up to restarting the level
    float restartTimer;

    void Awake() {
        // Set up the reference.
        anim = GameObject.Find("HUDCanvas").GetComponent<Animator>();
    }

    void Update() {
        // If the player has run out of health...
        if (playerHealth.currentHealth <= 0) {
			// saving the player's preferences
			PlayerPrefs.Save ();

            // ... tell the animator the game is over.
            anim.SetTrigger("GameOver");

            // .. increment a timer to count up to restarting.
            restartTimer += Time.deltaTime;

            // .. if it reaches the restart delay...
            if (restartTimer >= restartDelay) {
                // .. then reload the currently loaded level.
				SceneManager.LoadScene("Level 01");
            }
        }
    }
}
