using UnityEngine;
using System.Collections;

public class GamePreset : MonoBehaviour {

	void Start () {
		GameObject.Find("Preload").GetComponent<Preload>().FirstInGameScene();
	}

}