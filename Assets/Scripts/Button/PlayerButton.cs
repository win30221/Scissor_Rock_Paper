using UnityEngine;
using System.Collections;

public class PlayerButton : MonoBehaviour {

	public GameManager gameManager;
	public int index;

	private void OnClick() {
		gameManager.playerPlay = index;
	}
}
