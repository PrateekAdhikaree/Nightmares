using UnityEngine;
using UnityEngine.UI;

public class PauseManager : MonoBehaviour {

    public string pausedTitle = "PAUSED";
    public string resumeButtonText = "RESUME";
	public string exitButtonText = "EXIT";

    public Text titleText;
    public Text resumeText;
	public Text exitText;

	public Slider musicSlider;
	public Slider effectsSlider;

    Canvas menuCanvas;
    Canvas hudCanvas;

    bool gameHasStarted = false;

	void Awake() {
		MixLevels mixLevels = (MixLevels)GameObject.FindObjectOfType (typeof(MixLevels));
		
		if (PlayerPrefs.HasKey ("MusicValue")) {
			float val = PlayerPrefs.GetFloat ("MusicValue");
			musicSlider.value = val;
			mixLevels.SetMusicLvl (val);
		}
		
		if (PlayerPrefs.HasKey ("EffectsValue")) {
			float val = PlayerPrefs.GetFloat ("EffectsValue");
			effectsSlider.value = val;
			mixLevels.SetSfxLvl (val);
		}
	}

    void Start() {
		menuCanvas = GetComponent<Canvas>();
        hudCanvas = GameObject.Find("HUDCanvas").GetComponent<Canvas>();
        hudCanvas.enabled = false;
		Time.timeScale = 0;

		musicSlider.onValueChanged.AddListener(delegate {MusicValueChangeCheck(); });
		effectsSlider.onValueChanged.AddListener(delegate {EffectsValueChangeCheck(); });
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
			gameHasStarted = true;
            Pause();
        }
    }

	// Invoked when the music value of the slider changes.
	public void MusicValueChangeCheck() {
		PlayerPrefs.SetFloat ("MusicValue", musicSlider.value);
	}

	// Invoked when the effects value of the slider changes.
	public void EffectsValueChangeCheck() {
		PlayerPrefs.SetFloat ("EffectsValue", effectsSlider.value);
	}

    public void Pause() {
        menuCanvas.enabled = !menuCanvas.enabled;
        hudCanvas.enabled = !hudCanvas.enabled;

        Time.timeScale = Time.timeScale == 0 ? 1 : 0;

        if (gameHasStarted) {
            titleText.text = pausedTitle;
			resumeText.text = resumeButtonText;
			exitText.text = exitButtonText;
        }
    }

	public void ExitGame(){
		#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
		#else
		Application.Quit();
		#endif
	}
}
