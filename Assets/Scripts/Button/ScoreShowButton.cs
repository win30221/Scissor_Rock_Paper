using UnityEngine;
using System.Collections;

public class ScoreShowButton : MonoBehaviour {

	public GameObject targetUI;
	public GameObject[] otherUI;

	private void OnClick() {
		targetUI.SetActive(true);

		otherUI[0].SetActive(false);
		//otherUI[1].SetActive(false);
	}
}
