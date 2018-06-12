using UnityEngine;
using System.Collections;

public class VolumeMute : MonoBehaviour {

	// Lets us control the actual volume of the attached listener.
    public float volume = 1.0f;

	void Start () {
		AudioListener.volume = volume;
	}
}
