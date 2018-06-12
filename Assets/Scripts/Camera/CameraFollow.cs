using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour {

	// The position that that camera will be following.
	public Transform target; 
	// The speed with which the camera will be following.
	public float smoothing = 5f;     
	// reference to mini map camera object
	public GameObject minimap;

	// The initial offset from the target.
	Vector3 offset;

	void Start() {
		// Calculate the initial offset.
		offset = transform.position - target.position;
	}

	void FixedUpdate () {
		// Create a postion the camera is aiming for based on the offset from the target.
		Vector3 targetCamPos = target.position + offset;
		
		// Smoothly interpolate between the camera's current position and it's target position.
		transform.position = Vector3.Lerp (transform.position, targetCamPos, smoothing * Time.deltaTime);
	}

	void Update () {
		if (Input.GetButtonDown("MiniMap")) {
			// Toggle minimap
			minimap.SetActive(!minimap.activeInHierarchy);
		}
	}
}