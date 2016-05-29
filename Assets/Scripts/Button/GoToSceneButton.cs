using UnityEngine;
using System.Collections;

public class GoToSceneButton : MonoBehaviour {

	public string sceneName;

	private void OnClick() {
		Application.LoadLevel(sceneName);
	}
}
