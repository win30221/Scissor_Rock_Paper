using UnityEngine;
using System.Collections;

public class SubmitScore : MonoBehaviour {

	public GameManager gameManager;
	public UILabel name;

	private void OnClick() {
		gameManager.End(name.text);
	}
}
