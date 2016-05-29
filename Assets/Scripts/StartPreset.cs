using UnityEngine;
using System.Collections;

public class StartPreset : MonoBehaviour {

	void Start () {
		GameObject.Find("Preload").GetComponent<Preload>().FirstInStartScene();
	}

}
