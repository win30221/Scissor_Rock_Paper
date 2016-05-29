using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

	//======parse======
	private MyParse myParse;
	//=================

	private TestNetwork testNetwork;

	//======data======
	private string[][] data;
	private int dataCount;

	private float[][] timeData;
	//================
	

	//======state======
	private enum GameState {
		Starting,
		Playing,
		EndAnimation,
		Ending
	};
	private GameState gameState;

	//start in GameState.Playing
	private enum PlayState {
		NewRandom,
		Going,
		PlayingAnimation,
		Judgment
	};
	private PlayState playState;

	//start in PlayState.EndAnimation
	private enum EndAnimationState {
		MoveDown,
		Stop,

	};
	private EndAnimationState endAnimationState;

	//start in End
	private enum AnimationState {
		MoveForward,
		Stop,
		MoveBack
		
	};
	private AnimationState animationState;
	//=================
	

	//=====gameobject=====
	public UILabel CountDownLabel;
	public UIPlaySound CountDownSound;
	public UIPlayTween CountDownLabelPlayTween;
	public AudioClip CountDownSource;
	private float countDownTimeLeft;
	private int countDownTimeLeftTmp;

	public UISprite DefeatSprite;
	public TweenPosition DefeatSpriteTweenPosition;
	public UIPlayTween DefeatSpritePlayTween;

	public GameObject PlayUI;
	public UILabel PlayComboCountLabel;
	public UILocalize QuestionLabel;
	public UILabel TimeLeftCountDownLabel;
	private float timeLeft;

	public GameObject EndUI;
	public GameObject Warning;
	public UILabel EndComboCountLabel;
	public UIPlayTween[] player;
	public UIPlayTween[] computer;

	public GameObject ResultLabel;
	public UILocalize ResultLocalize;
	
	//=====================
	

	//=======other data=======
	public int playerPlay;   //0:scissor, 1:rock, 2:paper
	private int computerPlay;

	private int combo;

	private int randomQuestion;   //decide which question to use

	private bool isPlayed;   //check UIPlayTween is played.

	private float stopTime;   //stop when animation finished
	//========================
	
	void Start () {
		TextAsset binAsset = Resources.Load ("Rule", typeof(TextAsset)) as TextAsset;
		string [] lineArray = binAsset.text.Split ("\n"[0]);
		dataCount = lineArray.Length-1;
		data = new string [dataCount][];
		for (int i = 1; i < lineArray.Length; i++) {  
			data[i-1] = lineArray[i].Split (',');  
		}
		/*string tmp = "";
		for (int i = 0; i < dataCount; i++) {
			tmp = "";
			for (int j = 0; j < 4; j++) {
				tmp += data[i][j] + " ";
			}
			Debug.Log(tmp);
		}*/

		binAsset = Resources.Load ("Time", typeof(TextAsset)) as TextAsset;
		lineArray = binAsset.text.Split ("\n"[0]);
		timeData = new float[lineArray.Length-1][];
		string[] tmp;
		for (int i = 1; i < lineArray.Length; i++) {  
			tmp = lineArray[i].Split (',');
			timeData[i-1] = new float[tmp.Length];
			for (int j = 0; j < tmp.Length; j++) {
				timeData[i-1][j] = float.Parse(tmp[j]);
			}
		}
		/*string tmp1 = "";
		for (int i = 0; i < timeData.Length; i++) {
			tmp1 = "";
			for (int j = 0; j < 2; j++) {
				tmp1 += timeData[i][j] + " ";
			}
			Debug.Log(tmp1);
		}*/
		myParse = GameObject.Find("Preload").GetComponent<MyParse>();
		testNetwork = GameObject.Find("Preload").GetComponent<TestNetwork>();
		countDownTimeLeft = 3f;
		countDownTimeLeftTmp = 3;
		combo = 0;

		gameState = GameState.Starting;
	}

	void Update () {

		switch(gameState) {
		case GameState.Starting:  //ok
			if (countDownTimeLeft <= 0) {
				CountDownSound.audioClip = CountDownSource;
				CountDownSound.Play();
				gameState = GameState.Playing;
				playState = PlayState.NewRandom;
			} else if (Mathf.FloorToInt(countDownTimeLeft) < countDownTimeLeftTmp) {
				CountDownLabelPlayTween.resetOnPlay = true;
				if (countDownTimeLeftTmp < 3) {
					CountDownSound.Play();
				}
				CountDownLabel.text = countDownTimeLeftTmp.ToString();
				CountDownLabelPlayTween.Play(true);
				countDownTimeLeftTmp--;
			}
			countDownTimeLeft -= Time.deltaTime;
			break;
		case GameState.Playing:  //ok

			PlayComboCountLabel.text = combo.ToString();

			switch(playState) {
			case PlayState.NewRandom:  //ok

				randomQuestion = Random.Range(0, dataCount);
				computerPlay = NewComputerPlay(randomQuestion);
				playerPlay = -1;

				QuestionLabel.key = data[randomQuestion][0];
				timeLeft = SetTimeLeft(combo);

				PlayUI.SetActive(true);
				playState = PlayState.Going;
				break;
			case PlayState.Going:  //ok

				TimeLeftCountDownLabel.text = timeLeft.ToString("0.0");
				timeLeft -= Time.deltaTime;


				if (playerPlay != -1) {
					isPlayed = true;
					PlayUI.SetActive(false);
					playState = PlayState.PlayingAnimation;
					animationState = AnimationState.MoveForward;
				}
				if (timeLeft <= 0.0f) {
					PlayUI.SetActive(false);
					TimeLeftCountDownLabel.text = "0.0";
					DefeatSprite.spriteName = "TimesUp";
					gameState = GameState.EndAnimation;
				}
				break;
			case PlayState.PlayingAnimation:
				switch(animationState) {
				case AnimationState.MoveForward:
					if (isPlayed) {
						isPlayed = false;
						if (AwinB(playerPlay, computerPlay)) {
							ResultLocalize.key = "Win";
						} else if (AwinB(computerPlay, playerPlay)) {
							ResultLocalize.key = "Lose";
						} else {
							ResultLocalize.key = "Draw";
						}
						ResultLabel.gameObject.SetActive(true);
						player[playerPlay].Play(true);
						computer[computerPlay].Play (true);
					}
					if (!player[playerPlay].GetComponent<TweenPosition>().enabled) {
						stopTime = 1.0f;
						animationState = AnimationState.Stop;
					}
					break;
				case AnimationState.Stop:
					stopTime -= Time.deltaTime;
					if (stopTime <= 0.0f) {
						isPlayed = true;
						animationState = AnimationState.MoveBack;
					}
					break;
				case AnimationState.MoveBack:
					if (isPlayed) {
						isPlayed = false;
						player[playerPlay].Play(false);
						computer[computerPlay].Play (false);
					}
					if (!player[playerPlay].GetComponent<TweenPosition>().enabled) {
						ResultLabel.gameObject.SetActive(false);
						playState = PlayState.Judgment;
					}
					break;
				}
				break;
			case PlayState.Judgment:  //ok
				if (AwinB(playerPlay, computerPlay)) {  //player win
					combo++;
					playState = PlayState.NewRandom;
				} else if (AwinB(computerPlay, playerPlay)) {  //computer win
					DefeatSprite.spriteName = "Defeat";
					gameState = GameState.EndAnimation;

				} else {
					playState = PlayState.NewRandom;
				}
				break;
			}


			break;
		case GameState.EndAnimation:


			switch (endAnimationState) {
			case EndAnimationState.MoveDown:
				DefeatSpriteTweenPosition.from = new Vector3(0, 511, 0);
				DefeatSpriteTweenPosition.to = new Vector3(0, 0, 0);
				DefeatSpritePlayTween.Play(true);
				if (!DefeatSpriteTweenPosition.enabled) {
					stopTime = 1.0f;
					endAnimationState = EndAnimationState.Stop;
				}
				break;
			case EndAnimationState.Stop:
				if (stopTime > 0) {
					stopTime -= Time.deltaTime;
				} else {
					DefeatSpriteTweenPosition.gameObject.SetActive(false);
					testNetwork.ReTestConnection();
					gameState = GameState.Ending;
				}
				break;
			}


			break;
		case GameState.Ending:
			PlayUI.SetActive(false);
			EndUI.SetActive(true);
			EndComboCountLabel.text = combo.ToString();
			if (testNetwork.isConnect) {
				if (!testNetwork.isReConnect) {
					Warning.SetActive(false);
				}
			} else {
				Warning.SetActive(true);
				testNetwork.ReTestConnection();
			}
			break;
		}
	}

	public void End(string name) {
		if (testNetwork.isConnect) {
			myParse.InsertScore(combo, name);
			GameObject.Find("Preload").GetComponent<Preload>().FirstInStartScene();
			Application.LoadLevel("Start");
		}
		int[] scores = PlayerPrefsX.GetIntArray("scores");
		string[] names = PlayerPrefsX.GetStringArray("names");
		scores[10] = combo;
		names[10] = name;
		for(int i = 0; i < scores.Length; i++) {
			for(int j = i; j < scores.Length; j++) {
				if( scores[j] > scores[i] ) {
					int temp = scores[j];
					scores[j] = scores[i];
					scores[i] = temp;
					string temp1 = names[j];
					names[j] = names[i];
					names[i] = temp1;
				}
			}
		}
		PlayerPrefsX.SetIntArray ("scores", scores);
		PlayerPrefsX.SetStringArray ("names", names);
	}

	private int NewComputerPlay(int index) {
		int tmp = Random.Range (0, 101);
		int x = int.Parse (data [index] [1]);
		int y = int.Parse (data [index] [2]);
		int z = int.Parse (data [index] [3]);

		//Debug.Log (tmp + " " + x + " " + y + " " + z);
		if (x != 0 && tmp <= x) {
			return 0;
		} else if (y != 0 && tmp <= x + y) {
			return 1;
		} else if (z != 0 && tmp <= x + y + z) {
			return 2;
		}
		return 3;
	}

	private float SetTimeLeft(int combo) {
		for (int i = timeData.Length - 1; i >= 0; i--) {
			if (combo >= timeData[i][0]) {
				return timeData[i][1];
			}
		}
		return 0.0f;
	}

	private bool AwinB(int a, int b) {
		if (((a+2)%3) == b) {
			return true;
		}
		return false;
	}
}
