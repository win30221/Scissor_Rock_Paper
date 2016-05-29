using UnityEngine;
using System.Collections;

public class ChangeLanguage : MonoBehaviour {

	private string[] language;
	private int languageIndex;

	void Start() {
		language = new string[] {"English", "Chinese"};
		languageIndex = PlayerPrefs.GetInt("language");
	}

	private void OnClick() {
		languageIndex = (languageIndex + 1) % language.Length;
		PlayerPrefs.SetInt("language", languageIndex);
		Localization.language = language [languageIndex];
	}
}
