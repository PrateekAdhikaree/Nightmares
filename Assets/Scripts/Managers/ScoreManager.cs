using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScoreManager : MonoBehaviour {

	// The player's score.
	int score;       
	
	// Reference to the Text component.
	public Text number;

	void Awake() {
		// Reset the score.
		score = 0;
	}

	void Update() {
		// Set the displayed text to be the word "Score" followed by the score value.
		number.text = score.ToString();
	}

	public void AddScore(int toAdd) {
		score += toAdd;
		number.GetComponent<Animation>().Stop();
		number.GetComponent<Animation>().Play();
	}

	public int GetScore() {
		return score;
	}
}