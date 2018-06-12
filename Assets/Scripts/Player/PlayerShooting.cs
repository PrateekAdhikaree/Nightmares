using UnityEngine;
using UnityEngine.UI;

public class PlayerShooting : MonoBehaviour {

    public Color[] bulletColors;
    public float bounceDuration = 10;
    public float pierceDuration = 10;
    // The damage inflicted by each bullet.
    public int damagePerShot = 20;
    public int numberOfBullets = 1;
    // The time between each shot.
    public float timeBetweenBullets = 0.15f;
    public float angleBetweenBullets = 10f;
    // The distance the gun can fire.
    public float range = 100f;
    // A layer mask so the raycast only hits things on the shootable layer.
    public LayerMask shootableMask;
    // Reference to the UI's green health bar.
    public Image bounceImage;
    // Reference to the UI's red health bar.
    public Image pierceImage;
    public GameObject bullet;
    public Transform bulletSpawnAnchor;
	public GameObject pierceTimerObj;
	public GameObject bounceTimerObj;

    // A timer to determine when to fire.
    float timer;
    // A ray from the gun end forwards.
    Ray shootRay;
    // A raycast hit to get information about what was hit.
    RaycastHit shootHit;
    // Reference to the particle system.
    ParticleSystem gunParticles;
    // Reference to the line renderer.
    LineRenderer gunLine;
    // Reference to the audio source.
    AudioSource gunAudio;
    // Reference to the light component.
    Light gunLight;
    // The proportion of the timeBetweenBullets that the effects will display for.
    float effectsDisplayTime = 0.2f;
    float bounceTimer;
    float pierceTimer;
    bool bounce;
    bool piercing;
    Color bulletColor;

    public float BounceTimer {
        get { return bounceTimer; }
        set { bounceTimer = value; }
    }

    public float PierceTimer {
        get { return pierceTimer; }
        set { pierceTimer = value; }
    }

    void Awake() {
        // Set up the references.
        gunParticles = GetComponent<ParticleSystem>();
        gunAudio = GetComponent<AudioSource>();
        gunLight = GetComponentInChildren<Light>();

        bounceTimer = bounceDuration;
        pierceTimer = pierceDuration;
    }

    void Update() {
		// Disabling the timer labels for bounce and pierce
		bounceTimerObj.SetActive(false);
		pierceTimerObj.SetActive(false);

        if (bounceTimer < bounceDuration) {
            bounce = true;
        }
        else {
            bounce = false;
        }

        if (pierceTimer < pierceDuration) {
            piercing = true;
        }
        else {
            piercing = false;
        }

        bulletColor = bulletColors[0];
        if (bounce) {
			// setting and enabling label
			bounceTimerObj.SetActive(true);
			Text bounceTime = bounceTimerObj.GetComponent<Text> ();
			float floatVal = bounceDuration - bounceTimer;
			int val = Mathf.CeilToInt(floatVal);
			bounceTime.text = val.ToString ();

            bulletColor = bulletColors[1];
            bounceImage.color = bulletColors[1];
        }
        bounceImage.gameObject.SetActive(bounce);

        if (piercing) {
			// setting and enabling label
			pierceTimerObj.SetActive(true);
			Text pierceTime = pierceTimerObj.GetComponent<Text> ();
			float floatVal = pierceDuration - pierceTimer;
			int val = Mathf.CeilToInt(floatVal);
			pierceTime.text = val.ToString ();

            bulletColor = bulletColors[2];
            pierceImage.color = bulletColors[2];
        }
        pierceImage.gameObject.SetActive(piercing);

        if (piercing & bounce) {
            bulletColor = bulletColors[3];
            bounceImage.color = bulletColors[3];
            pierceImage.color = bulletColors[3];
        }

		var main = gunParticles.main;
		main.startColor = bulletColor;
        // For some reason the color I had selected originally looked extremely
        // reddish after I switched to deferred rendering and linear mode so 
        // I'm hardcoding in a lighter, more yellow light color if you have
        // both the pierce and bounce powerup active.
        gunLight.color = (piercing & bounce) ? new Color(1, 140f / 255f, 30f / 255f, 1) : bulletColor;

        // Add the time since Update was last called to the timer.
        bounceTimer += Time.deltaTime;
        pierceTimer += Time.deltaTime;
        timer += Time.deltaTime;

        // If the Fire1 button is being press and it's time to fire...
        if (Input.GetButton("Fire1") && timer >= timeBetweenBullets && Time.timeScale != 0) {
            // ... shoot the gun.
            Shoot();
        }

        // If the timer has exceeded the proportion of timeBetweenBullets that the effects should be displayed for...
        if (timer >= timeBetweenBullets * effectsDisplayTime) {
            // ... disable the effects.
            DisableEffects();
        }
    }

    public void DisableEffects() {
        // Disable the line renderer and the light.
        gunLight.enabled = false;
    }

    void Shoot() {
        // Reset the timer.
        timer = 0f;

        // Play the gun shot audioclip.
        gunAudio.pitch = Random.Range(1.2f, 1.3f);
        if (bounce) {
            gunAudio.pitch = Random.Range(1.1f, 1.2f);
        }
        if (piercing) {
            gunAudio.pitch = Random.Range(1.0f, 1.1f);
        }
        if (piercing & bounce) {
            gunAudio.pitch = Random.Range(0.9f, 1.0f);
        }
        gunAudio.Play();

        // Enable the light.
        gunLight.intensity = 2 + (0.25f * (numberOfBullets - 1));
        gunLight.enabled = true;

        // Stop the particles from playing if they were, then start the particles.
        gunParticles.Stop();
		var main = gunParticles.main;
        main.startSize = 1 + (0.1f * (numberOfBullets - 1));
        gunParticles.Play();

        // Set the shootRay so that it starts at the end ofres the gun and points forward from the barrel.
        shootRay.origin = transform.position;
        shootRay.direction = transform.forward;

        for (int i = 0; i < numberOfBullets; i++) {
            // Make sure our bullets spread out in an even pattern.
            float angle = i * angleBetweenBullets - ((angleBetweenBullets / 2) * (numberOfBullets - 1));
            Quaternion rot = transform.rotation * Quaternion.AngleAxis(angle, Vector3.up);
            GameObject instantiatedBullet = Instantiate(bullet, bulletSpawnAnchor.transform.position, rot) as GameObject;
            instantiatedBullet.GetComponent<Bullet>().piercing = piercing;
            instantiatedBullet.GetComponent<Bullet>().bounce = bounce;
            instantiatedBullet.GetComponent<Bullet>().bulletColor = bulletColor;
        }
    }
}
