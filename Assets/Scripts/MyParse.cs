using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Parse;
using System.Threading.Tasks;

public class MyParse : MonoBehaviour {

	public bool isLogin;   //use it to check is user logined
	//private bool isCheckLogin;
	
	public IEnumerable<ParseObject> results;   //return for user's query results
	public bool isGetQuest;   //check user's query results is done

	public bool isGetScores;
	public bool isGetTop100Scores;
	
	// Use this for initialization
	void Start () {
		isLogin = false;
		if (ParseUser.CurrentUser != null) {
			isLogin = true;
			//isCheckLogin = true;
		} else {
			Login(PlayerPrefs.GetString("account"), PlayerPrefs.GetString("account"));
		}
	}

	void Update () {
		/*if (isCheckLogin) {
			if (ParseUser.CurrentUser != null) {

			} else {
				Debug.Log("Not logined");
			}
		}*/
	}
	
	void OnApplicationQuit() {
		//ParseUser.LogOut();
	}

	public void CreateAccount() {
		/* 1.count users and give new user next ID(ex:0000001)
		 * 2.login
		 * */
		var query = ParseUser.Query;
		query.CountAsync().ContinueWith (t => {
			
			string tmp = string.Format("{0:0000000}", (t.Result + 1));
			PlayerPrefs.SetString("account", tmp);
			var user = new ParseUser()
			{
				Username = tmp,
				Password = tmp
			};
			Task signUpTask = user.SignUpAsync();
			Debug.Log("Create account");
			
			Login(tmp, tmp);
		});
	}
	
	public void Login(string username, string password) {
		/* 1.use checkLogin to check is it want to login
		 * 2.use Parse API to login
		 * 3.if login successful, then set isLogin to true
		 * 4.return checkLogin done
		 * */
		Debug.Log("Loginning");
		isLogin = false;
		//isCheckLogin = false;
		ParseUser.LogInAsync(username, password).ContinueWith(t => {
			if (t.IsFaulted || t.IsCanceled) {
				Debug.Log("Not Logined");
				Login(PlayerPrefs.GetString("account"), PlayerPrefs.GetString("account"));
			} else {
				isLogin = true;
				Debug.Log("Logined");
			}
			//isCheckLogin = true;
		});
	}
	
	public void ShowStageScore(string name, string stage) {
		/* 1.set results to null
		 * 2.Query StageScore by name, stage isn't necessary
		 * 3.if done, then return result
		 * */
		results = null;

		var	query = ParseObject.GetQuery("StageScore")
				.WhereEqualTo("username", name);
		if (stage != "") {
			query = query.WhereEqualTo("stage", stage);
		}

		query.FindAsync().ContinueWith(t => {
			results = t.Result;
			isGetQuest = true;
		});
	}

	public void QueryScore(string name = "") {
		/* 1.set results to null
		 * 2.Query whole score or by name
		 * 3.if done, then return result
		 * */
		results = null;

		var	query = ParseObject.GetQuery("Score").OrderByDescending("score");
		if (name != "") {
			query = query.WhereEqualTo("username", name);
		}
		
		query.FindAsync().ContinueWith(t => {
			results = t.Result;
			isGetScores = true;
		});
	}

	public void QueryTop100Score() {
		/* 1.Query whole score order by score
		 * 2.if done, then return result
		 * */
		var	query = ParseObject.GetQuery("Score").OrderByDescending("score").Limit(100);
		
		query.FindAsync().ContinueWith(t => {
			results = t.Result;
			isGetTop100Scores = true;
		});
	}
	
	public void InsertOrUpdateStageScore(string stage, int score) {
		/* 1.Query StageScore by username and stage
		 * 2.if storage score is less than new score, then replace it
		 * 3.else do nothing
		 * 4.if no record, then insert a new object
		 * */
		var query = ParseObject.GetQuery("Score")
				.WhereEqualTo("username", ParseUser.CurrentUser.Username)
				.WhereEqualTo("stage", stage);
		
		query.FindAsync().ContinueWith(t => {
			int count = 0;
			foreach (var s in t.Result) {
				count++;
				s.SaveAsync().ContinueWith(tt => {
					if (s.Get<int>("score") < score) {
						s["score"] = score;
						s.SaveAsync();
						Debug.Log("updated");
						ShowStageScore(ParseUser.CurrentUser.Username, stage);
					} else {
						Debug.Log("do nothing");
						ShowStageScore(ParseUser.CurrentUser.Username, stage);
					}
				});
			}
			if (count == 0) {
				ParseObject gameScore = new ParseObject("Score");
				gameScore["username"] = ParseUser.CurrentUser.Username;
				gameScore["stage"] = stage;
				gameScore["score"] = score;
				Task saveTask = gameScore.SaveAsync();
				Debug.Log("inserted");
				ShowStageScore(ParseUser.CurrentUser.Username, stage);
			}
		});
	}

	public void InsertScore(int score, string name) {
		ParseObject gameScore = new ParseObject("Score");
		gameScore["username"] = ParseUser.CurrentUser.Username;
		gameScore["nickname"] = name;
		gameScore["score"] = score;
		Task saveTask = gameScore.SaveAsync();
		Debug.Log("inserted");
	}

}












/*   Version1
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Parse;
using System.Threading.Tasks;

public class Login : MonoBehaviour {

	public bool checkLogin;

	public GameObject UILogin;
	public GameObject UILogined;

	IEnumerable<ParseObject> results;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		if (checkLogin) {
			if (ParseUser.CurrentUser != null) {
				//Debug.Log("Logined");
				UILogin.SetActive(false);
				UILogined.SetActive(true);
				//ShowStageScore("1-1", ParseUser.CurrentUser.Username);
			} else {
				Debug.Log("Not logined");
			}
		}
	}

	void OnApplicationQuit() {
		ParseUser.LogOut();
	}

	public void MyLogin(string username, string password){
		checkLogin = false;
		ParseUser.LogInAsync(username, password).ContinueWith(t => {
			if (t.IsFaulted || t.IsCanceled) {
				Debug.Log("Not Logined");
			} else {
				Debug.Log("Logined");
			}
			checkLogin = true;
		});
	}

	public void ShowStageScore(string stage, string name) {
		var query = ParseObject.GetQuery("StageScore")
			.WhereEqualTo("username", name)
				.WhereEqualTo("stage", stage);
		query.FindAsync().ContinueWith(t => {
			results = t.Result;
			foreach (var score in results) {
				Debug.Log(score.Get<string>("username") + " " + score.Get<int>("score"));
			}
		});
	}

	public void UpdateStageScore(string stage, int newScore) {
		var query = ParseObject.GetQuery("StageScore")
			.WhereEqualTo("username", ParseUser.CurrentUser.Username)
				.WhereEqualTo("stage", stage);

		query.FindAsync().ContinueWith(t => {
			foreach (var score in t.Result) {
				score.SaveAsync().ContinueWith(tt => {
					score["score"] = newScore;
					score.SaveAsync();
					Debug.Log("updated");
				});
			}
		});
	}
}
*/