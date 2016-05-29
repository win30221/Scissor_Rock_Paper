using UnityEngine;
using System.Collections;
using Parse;

public class Preload : MonoBehaviour {

	public MyParse myParse;
	public TestNetwork testNetwork;
	private bool isCallCreateAccount;

	public AudioSource normalMusic;
	public AudioSource playMusic;


	// Use this for initialization
	void Start () {
		DontDestroyOnLoad(this);
		//PlayerPrefs.SetInt ("isSetData", 0);
		//PlayerPrefs.DeleteAll ();
		if (PlayerPrefs.GetInt ("isSetData") != 1) {
			PlayerPrefs.SetString("account", "");
			PlayerPrefs.SetFloat("musicVolume", 1.0f);
			PlayerPrefs.SetFloat("soundVolume", 1.0f);
			PlayerPrefs.SetInt("language", 0);
			PlayerPrefs.SetInt("isSetData", 1);
			PlayerPrefsX.SetIntArray("scores", new int[]{-1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1});
			PlayerPrefsX.SetStringArray("names", new string[11]);
		}

		isCallCreateAccount = false;

		normalMusic.volume = PlayerPrefs.GetFloat("musicVolume");
		playMusic.volume = PlayerPrefs.GetFloat("musicVolume");

		Application.LoadLevel("Start");
	}

	void Update() {
		if (testNetwork.isConnect && PlayerPrefs.GetString("account") == "") {
			if (!isCallCreateAccount) {
				isCallCreateAccount = true;
				if (ParseUser.CurrentUser != null) {
					ParseUser.LogOut();
				}
				myParse.CreateAccount();
			}
		}
	}

	public void FirstInStartScene() {
		if (!normalMusic.isPlaying) {
			normalMusic.Play();
		}
		playMusic.Stop();
	}

	public void FirstInGameScene() {
		normalMusic.Pause();
		if (!playMusic.isPlaying) {
			playMusic.Play();
		}
	}
}
