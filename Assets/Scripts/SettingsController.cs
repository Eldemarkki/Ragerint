using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SettingsController : MonoBehaviour {

    Slider volumeSlider;
    Button resetProgressButton, yesResetButton, noResetButton;
    GameObject resetPanel;

	// Use this for initialization
	void Start () {
        volumeSlider = GameObject.Find("VolumeSlider").GetComponent<Slider>();
        volumeSlider.value = PlayerPrefs.GetFloat("volume", 1f);
        volumeSlider.onValueChanged.AddListener(delegate { AudioListener.volume = volumeSlider.value; PlayerPrefs.SetFloat("volume", volumeSlider.value); });

        resetPanel = GameObject.Find("ResetPanel");

        yesResetButton = GameObject.Find("YesResetButton").GetComponent<Button>();
        noResetButton = GameObject.Find("NoResetButton").GetComponent<Button>();

        yesResetButton.onClick.AddListener(() => YesReset());
        noResetButton.onClick.AddListener(() => NoReset());



        resetPanel.SetActive(false);

        resetProgressButton = GameObject.Find("ResetProgressButton").GetComponent<Button>();
        resetProgressButton.onClick.AddListener(() => AskReset());
    }

    void AskReset()
    {
        resetPanel.SetActive(true);
    }

    void YesReset()
    {
        PlayerPrefs.SetInt("score", 0);
        PlayerPrefs.SetInt("highscore", 0);
        resetPanel.SetActive(false);
    }

    void NoReset()
    {
        resetPanel.SetActive(false);
    }

    // Update is called once per frame
    void Update () {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Backspace))
        {
            if (resetPanel.activeSelf)
                resetPanel.SetActive(false);
            else
                SceneManager.LoadScene("Menu");
        }

	}
}
