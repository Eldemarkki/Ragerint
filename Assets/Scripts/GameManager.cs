using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

	public static GameManager instance;

	public int score = 0;
    public int highscore = 0;


    public bool needsFade = true;
	public bool paused = false;

	void Awake(){
		instance = this;

		DontDestroyOnLoad(gameObject);

		if(FindObjectsOfType(GetType()).Length>1)
			DestroyImmediate(gameObject);
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void Play(bool resetScore){

        
        if (resetScore)
            score = 0;

		SceneManager.LoadScene("Game");
	}

	public void GotoSettings(){
		SceneManager.LoadScene("Settings");
	}

	public void NeedHelp(){
		SceneManager.LoadScene("Help");
	}
}
