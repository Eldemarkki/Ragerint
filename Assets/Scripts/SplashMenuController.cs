using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SplashMenuController : MonoBehaviour {

	// Use this for initialization
	void Start () {
		StartCoroutine(Fade());
	}

	IEnumerator Fade(){
		yield return new WaitForSecondsRealtime(3f);
		SceneManager.LoadScene("Menu");
	}
}
