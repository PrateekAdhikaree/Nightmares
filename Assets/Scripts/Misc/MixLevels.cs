using UnityEngine;
using System.Collections;
using UnityEngine.Audio;

public class MixLevels : MonoBehaviour {

    public AudioMixer masterMixer;

    void Start() {
		if (PlayerPrefs.HasKey ("EffectsValue"))
			masterMixer.SetFloat("sfxVol", PlayerPrefs.GetFloat ("EffectsValue"));
		else
        	masterMixer.SetFloat("sfxVol", 0);

		if (PlayerPrefs.HasKey ("MusicValue"))
			masterMixer.SetFloat("musicVol", PlayerPrefs.GetFloat ("MusicValue"));
		else
			masterMixer.SetFloat("musicVol", -15);
    }

    public void SetSfxLvl(float sfxLvl) {
        masterMixer.SetFloat("sfxVol", sfxLvl);
    }

    public void SetMusicLvl(float musicLvl) {
        masterMixer.SetFloat("musicVol", musicLvl);
    }
}
