using UnityEngine;
using System.Collections;

public class PickupManager : MonoBehaviour {
	
	public int scoreNeededForExtraBullet = 1500;
	public int extraScoreNeededAfterEachPickup = 1500;

	public Pickup healthPickup;
	public Pickup bouncePickup;
	public Pickup piercePickup;
	public Pickup bulletPickup;
}
