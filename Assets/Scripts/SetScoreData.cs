using UnityEngine;
using System.Collections;

public class SetScoreData : MonoBehaviour {

	public UILabel NOLabel;
	public UILabel UserLabel;
	public UILabel ScoreLabel;

	private string[] NO = {"NO{0}", "第{0}名"};
	private string[] Score = {"{0} win", "{0}次"};

	public void SetData(int x, string y, int z) {
		NOLabel.text = string.Format(NO[PlayerPrefs.GetInt("language")], x);
		UserLabel.text = y;
		ScoreLabel.text = string.Format(Score[PlayerPrefs.GetInt("language")], z);
	}
}
