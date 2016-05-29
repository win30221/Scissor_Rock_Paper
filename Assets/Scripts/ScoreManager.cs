using UnityEngine;
using System.Collections;
using Parse;

public class ScoreManager : MonoBehaviour {

	private MyParse myParse;
	private TestNetwork testNetwork;

	public UILocalize localLoading;
	public GameObject localGameObject;
	public GameObject localScoreItem;
	public float localPosition;

	public UILocalize top100Loading;
	public GameObject top100GameObject;
	public GameObject top100ScoreItem;
	public float top100Position; 

	public bool isQuery;

	public enum LoadState {
		Local = 0,
		World = 1,
		Top100 = 2
	};
	public LoadState loadState;

	int sum = 0;

	// Use this for initialization
	void Start () {
		myParse = GameObject.Find("Preload").GetComponent<MyParse>();
		testNetwork = GameObject.Find("Preload").GetComponent<TestNetwork>();
		testNetwork.ReTestConnection();
		isQuery = false;

		localPosition = 275.98f;
		top100Position = 275.98f;
		loadState = LoadState.Local;
	}
	
	// Update is called once per frame
	void Update () {

		switch (loadState) {
		case LoadState.Local:
			/*if (!isQuery) {
				isQuery = true;
				myParse.QueryScore(ParseUser.CurrentUser.Username);
			} else {
				if (myParse.isGetScores) {
					myParse.isGetScores = false;
					int sum = 0;
					foreach (var score in myParse.results) {
						sum++;
						GameObject clone = NGUITools.AddChild (localGameObject, localScoreItem);
						clone.transform.localPosition = new Vector3(0, localPosition, 0);
						clone.GetComponent<SetScoreData>().SetData(sum, score.Get<string>("nickname"), score.Get<int>("score"));
						localPosition -= 60.0f;
					}
					if (sum > 0) {
						localLoading.gameObject.SetActive(false);
					} else {
						localLoading.key = "NoData";
					}
					isQuery = false;
					loadState = LoadState.World;
				}
			}*/
			int[] scores = PlayerPrefsX.GetIntArray("scores");
			string[] names = PlayerPrefsX.GetStringArray("names");
			sum = 0;
			for (int i = 0; i < 10; i++) {
				if (scores[i] >= 0) {
					sum++;
					GameObject clone = NGUITools.AddChild (localGameObject, localScoreItem);
					clone.transform.localPosition = new Vector3(0, localPosition, 0);
					clone.GetComponent<SetScoreData>().SetData(sum, names[i], scores[i]);
					localPosition -= 60.0f;
				}
			}
			if (sum > 0) {
				localLoading.gameObject.SetActive(false);
			} else {
				localLoading.key = "NoData";
			}
			loadState = LoadState.World;
			break;
		case LoadState.World:
			loadState = LoadState.Top100;
			break;
		case LoadState.Top100:
			if (testNetwork.isConnect) {
				if (!testNetwork.isReConnect) {
					if (!isQuery) {
						isQuery = true;
						myParse.QueryTop100Score();
					} else {
						if (myParse.isGetTop100Scores) {
							myParse.isGetTop100Scores = false;
							sum = 0;
							
							foreach (var score in myParse.results) {
								sum++;
								GameObject clone = NGUITools.AddChild (top100GameObject, top100ScoreItem);
								clone.transform.localPosition = new Vector3(0, top100Position, 0);
								clone.GetComponent<SetScoreData>().SetData(sum, score.Get<string>("nickname"), score.Get<int>("score"));
								top100Position -= 60.0f;
							}
							if (sum > 0) {
								top100Loading.gameObject.SetActive(false);
							} else {
								top100Loading.key = "NoData";
							}
						}
					}
				}
			} else {
				top100Loading.key = "PleaseConnectNetwork";
				testNetwork.ReTestConnection();
			}
			break;
		}
	}
}
