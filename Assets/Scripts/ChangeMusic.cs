using UnityEngine;
using System.Collections;

public class ChangeMusic : MonoBehaviour {

	private UISlider mSlider;
	public AudioSource music;

	// Use this for initialization
	void Start () {
		music = GameObject.Find("NormalMusic").GetComponent<AudioSource>();
		mSlider = GetComponent<UISlider>();
		mSlider.value = PlayerPrefs.GetFloat("musicVolume");
		EventDelegate.Add(mSlider.onChange, OnChange);
	}

	void OnChange () {
		music.volume = UISlider.current.value;
		PlayerPrefs.SetFloat ("musicVolume", UISlider.current.value);
	}
}
