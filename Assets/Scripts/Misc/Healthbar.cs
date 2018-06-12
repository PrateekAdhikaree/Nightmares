using UnityEngine;

public class Healthbar : MonoBehaviour {

	public GameObject enemy;

	void Update() {
		if (enemy == null) {
			return;
		}

		Vector3 screenPos = Camera.main.WorldToScreenPoint(enemy.transform.position);
		gameObject.transform.position = screenPos + new Vector3(0, 60, 0);
	}
}
