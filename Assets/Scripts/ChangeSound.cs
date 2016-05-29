using UnityEngine;
using System.Collections;

public class ChangeSound : MonoBehaviour {

	private UISlider mSlider;
	
	// Use this for initialization
	void Start () {
		mSlider = GetComponent<UISlider>();
		mSlider.value = PlayerPrefs.GetFloat("soundVolume");
		EventDelegate.Add(mSlider.onChange, OnChange);
	}
	
	void OnChange () {
		PlayerPrefs.SetFloat ("soundVolume", UISlider.current.value);
	}
}
