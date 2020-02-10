using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuController : MonoBehaviour {

	Button playButton, settingsButton, helpButton, exitButton;

	GameManager gameManager;

	// Use this for initialization
	void Start () {


		playButton = GameObject.Find("PlayButton").GetComponent<Button>();
		settingsButton = GameObject.Find("SettingsButton").GetComponent<Button>();
		helpButton = GameObject.Find("HelpButton").GetComponent<Button>();
        exitButton = GameObject.Find("ExitButton").GetComponent<Button>();

		gameManager = GameManager.instance;

		playButton.onClick.AddListener(() => gameManager.Play(true));
		settingsButton.onClick.AddListener(() => gameManager.GotoSettings());
		helpButton.onClick.AddListener(() => gameManager.NeedHelp());
        exitButton.onClick.AddListener(() => Application.Quit());
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
