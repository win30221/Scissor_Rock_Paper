using UnityEngine;
using System.Collections;

public class TestNetwork : MonoBehaviour {

	public bool isConnect;   //check network is connected
	public bool isReConnect;   //recheck network is connected, if not set isConnect to false

	private void Start() {
		isConnect = false;
		isReConnect = false;
		//StartCoroutine(CheckConnectionToMasterServer(0));
		StartCoroutine(CheckConnectionServer());
	}

	public void ReTestConnection() {
		isReConnect = true;
		StartCoroutine(CheckConnectionServer());
	}

	/*private IEnumerator CheckConnectionToMasterServer(int testCount) {
		if (testCount <= 5) {
			Ping pingMasterServer = new Ping("74.125.23.94");
			float startTime = Time.time;
			while (!pingMasterServer.isDone && Time.time < startTime + 5.0f) {
				yield return new WaitForSeconds(0.1f);
			}
			if(pingMasterServer.isDone) {
				isConnect = true;
				Debug.Log("Connect Internet successful");
			} else {
				isConnect = false;
				StartCoroutine(CheckConnectionToMasterServer(testCount+1));
				Debug.Log("Connect Internet unsuccessful");
			}
		}
	}*/

	private IEnumerator CheckConnectionServer() {
		WWW www = new WWW ("https://www.google.com.tw/");
		yield return www;
		if (www.error == null) {
			isConnect = true;
			isReConnect = false;
			Debug.Log("Connect Internet successful");
		} else {
			isConnect = false;
			isReConnect = false;
			Debug.Log("Connect Internet unsuccessful");
		}
	}
}
