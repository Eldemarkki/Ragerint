using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BallController : MonoBehaviour
{

    LineRenderer lineRenderer;
    Rigidbody2D rb;
    FollowPlayer cam;
    Texture2D map;

    public GameObject wallPrefab;
    public GameObject spikePrefab;
    public GameObject spikeDoorPrefab;
    public GameObject goalPrefab;

    public GameObject pausePanel;
    public GameObject settingsPanel;
    public GameObject gameOverPanel;

    public Button resumeButton;
    public Button settingsButton;
    public Button quitButton;

    public Button continueButton;
    public Button backToMenuButton;

    public Text scoreText;
    public Text inGameScoreText;

    public Slider volumeSlider;
    public Slider effectsSlider;

    public float maxLineLength;
    public float ballSpeed;

    public float spikeDoorChance = 20;
    public float wallReplacementWithSpikeChance = 15;

    public GameObject deathParticleSystem, winParticleSystem;

    public bool paused = false;
    public bool inSettings = false;

    public bool isDead = false;
    public bool isWon = false;

    public bool hallwaysEndWithSpikes = false;

    // For clean-code-purposes; reduces CanSpawn function arguments
    Vector2 currentEmptySpot;

    // Use this for initialization
    void Start()
    {
        Time.timeScale = 1;
        isDead = false;
        isWon = false;

        pausePanel = GameObject.Find("PausePanel");
        resumeButton = GameObject.Find("ResumeButton").GetComponent<Button>();
        settingsButton = GameObject.Find("SettingsButton").GetComponent<Button>();
        quitButton = GameObject.Find("QuitButton").GetComponent<Button>();
        pausePanel.SetActive(false);

        inGameScoreText = GameObject.Find("InGameScoreText").GetComponent<Text>();
        GameManager.instance.highscore = PlayerPrefs.GetInt("highscore", 0);
        GameManager.instance.score = PlayerPrefs.GetInt("score", 0);
        inGameScoreText.text = GameManager.instance.score.ToString() + "/" + GameManager.instance.highscore.ToString();

        resumeButton.onClick.AddListener(() => Unpause());
        settingsButton.onClick.AddListener(() => OpenSettings());
        quitButton.onClick.AddListener(() => Quit());

        settingsPanel = GameObject.Find("SettingsPanel");
        volumeSlider = GameObject.Find("VolumeSlider").GetComponent<Slider>();
        volumeSlider.value = PlayerPrefs.GetFloat("volume", 1f);

        settingsPanel.SetActive(false);



        gameOverPanel = GameObject.Find("GameOverPanel");
        continueButton = GameObject.Find("ContinueButton").GetComponent<Button>();
        backToMenuButton = GameObject.Find("BackToMenuButton").GetComponent<Button>();
        scoreText = GameObject.Find("ScoreAfterGameText").GetComponent<Text>();
        gameOverPanel.SetActive(false);



        continueButton.onClick.AddListener(() => GameManager.instance.Play(true));
        backToMenuButton.onClick.AddListener(() => Quit());


        volumeSlider.onValueChanged.AddListener(delegate { AudioListener.volume = volumeSlider.value; PlayerPrefs.SetFloat("volume", volumeSlider.value); });


        lineRenderer = GetComponent<LineRenderer>();
        rb = GetComponent<Rigidbody2D>();

        map = Resources.Load("Mazes/" + Random.Range(0, 40).ToString()) as Texture2D;
        cam = FollowPlayer.instance;

        CreateLevel(map);

        // I know, it's not floating point division.
        cam.mapMiddle = new Vector3(map.width / 2, map.height / 2, cam.gameObject.transform.position.z);
    }

    public void Pause()
    {
        lineRenderer.enabled = false;
        pausePanel.SetActive(true);
        paused = true;
        Time.timeScale = 0;
    }

    public void Unpause()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);


        lineRenderer.enabled = true;
        pausePanel.SetActive(false);
        paused = false;
        Time.timeScale = 1;

        lineRenderer.SetPositions(new Vector3[] { transform.position, transform.position });
    }

    public void OpenSettings()
    {
        inSettings = true;
        settingsPanel.SetActive(true);
        pausePanel.SetActive(false);
    }

    public void CloseSettings()
    {
        inSettings = false;
        settingsPanel.SetActive(false);
        pausePanel.SetActive(true);
    }

    public void Quit()
    {
        Debug.Log("lol");
        GameManager.instance.score = 0;
        PlayerPrefs.SetInt("score", GameManager.instance.score);
        SceneManager.LoadScene("Menu");
    }

    public void Win()
    {
        isWon = true;
        StartCoroutine(PlayWinParticles());
    }

    public void Die()
    {
        StartCoroutine(PlayDeathParticles());
    }

    IEnumerator PlayWinParticles()
    {


        rb.bodyType = RigidbodyType2D.Static;

        GameManager.instance.score++;
        PlayerPrefs.SetInt("score", GameManager.instance.score);

        

        inGameScoreText.text = GameManager.instance.score.ToString() + "/" + GameManager.instance.highscore.ToString();


        ParticleSystem ps = Instantiate(winParticleSystem, transform.position, Quaternion.identity).GetComponent<ParticleSystem>();
        while (!ps.isStopped)
            yield return null;
        yield return new WaitForSecondsRealtime(1f);
        Destroy(ps.gameObject);


        GameManager.instance.Play(false);
    }

    IEnumerator PlayDeathParticles()
    {
        isDead = true;
        GetComponent<SpriteRenderer>().enabled = false;
        ParticleSystem ps = Instantiate(deathParticleSystem, transform.position, Quaternion.identity).GetComponent<ParticleSystem>();
        rb.bodyType = RigidbodyType2D.Static;
        while (!ps.isStopped)
            yield return null;
        Destroy(ps.gameObject);
        yield return new WaitForSecondsRealtime(1f);

        gameOverPanel.SetActive(true);

        backToMenuButton = GameObject.Find("BackToMenuButton").GetComponent<Button>();
        backToMenuButton.onClick.AddListener(() => Quit());

        if (GameManager.instance.score > GameManager.instance.highscore)
        {
            scoreText.text = "New highscore! Score: " + GameManager.instance.score.ToString();
            GameManager.instance.highscore = GameManager.instance.score;
            PlayerPrefs.SetInt("highscore", GameManager.instance.highscore);
            PlayerPrefs.Save();
        }
        else
            scoreText.text = "Score: " + GameManager.instance.score.ToString();

        GameManager.instance.score = 0;
        PlayerPrefs.SetInt("score", GameManager.instance.score);

        Camera.main.GetComponent<FollowPlayer>().follow = false;
        Destroy(gameObject);


    }

    #region Level Creation

    bool IsEmptySpot(Texture2D map, Vector2 position)
    {
        return map.GetPixel((int)position.x, (int)position.y) == Color.white;
    }

    //Could use array but too much work (actually not even that much...)
    bool CanSpawn(bool topEmpty, bool downEmpty, bool rightEmpty, bool leftEmpty)
    {
        return (IsEmptySpot(map, currentEmptySpot + Vector2.up) == topEmpty)
            && (IsEmptySpot(map, currentEmptySpot + Vector2.down) == downEmpty)
            && (IsEmptySpot(map, currentEmptySpot + Vector2.right) == rightEmpty)
            && (IsEmptySpot(map, currentEmptySpot + Vector2.left) == leftEmpty);
    }

    void CreateLevel(Texture2D map)
    {

        List<Vector2> emptySpots = new List<Vector2>();

        for (int x = 0; x < map.width; x++)
        {
            for (int y = 0; y < map.height; y++)
            {

                Color p = map.GetPixel(x, y);

                if (p == Color.black)
                {
                    if (Random.value >= (100 - wallReplacementWithSpikeChance) / 100)
                    {
                        GameObject g = Instantiate(spikePrefab, new Vector2(x, y), Quaternion.identity);
                        g.GetComponentInChildren<SpriteRenderer>().sortingLayerName = "Spikes";
                        g.tag = "Spike";
                    }
                    else
                    {
                        GameObject g = Instantiate(wallPrefab, new Vector3(x, y), Quaternion.identity);
                        g.GetComponent<SpriteRenderer>().sortingLayerName = "Walls";
                        g.tag = "Wall";
                    }
                }
                else
                    emptySpots.Add(new Vector2(x, y));

            }
        }

        Vector2[] list = emptySpots.ToArray();
        foreach (Vector2 p in list)
        {

            currentEmptySpot = p;

            if (hallwaysEndWithSpikes)
            {
                // X O X
                // X O X
                // X X X
                if (CanSpawn(true, false, false, false))
                {
                    GameObject spike = Instantiate(spikePrefab, p, new Quaternion(0, 0, 90, 0));
                    spike.GetComponentInChildren<SpriteRenderer>().sortingLayerName = "Spikes";
                    emptySpots.Remove(p);
                    spike.tag = "Spike";
                }


                // X X X
                // X O X
                // X O X
                else if (CanSpawn(false, true, false, false))
                {
                    GameObject spike = Instantiate(spikePrefab, p, new Quaternion(0, 0, 90, 0));
                    spike.GetComponentInChildren<SpriteRenderer>().sortingLayerName = "Spikes";
                    emptySpots.Remove(p);
                    spike.tag = "Spike";
                }

                // X X X
                // X O O
                // X X X
                else if (CanSpawn(false, false, true, false))
                {
                    GameObject spike = Instantiate(spikePrefab, p, new Quaternion(0, 0, 90, 0));
                    spike.GetComponentInChildren<SpriteRenderer>().sortingLayerName = "Spikes";
                    emptySpots.Remove(p);
                    spike.tag = "Spike";
                }

                // X X X
                // O O X
                // X X X
                else if (CanSpawn(false, false, false, true))
                {
                    GameObject spike = Instantiate(spikePrefab, p, new Quaternion(0, 0, -90, 0));
                    spike.GetComponentInChildren<SpriteRenderer>().sortingLayerName = "Spikes";
                    emptySpots.Remove(p);
                    spike.tag = "Spike";
                }
            }

            // Replace random walls with spikes

            // X X X
            // O O O
            // X X X
            if (CanSpawn(false, false, true, true) && Random.value >= (100 - spikeDoorChance) / 100)
            {
                GameObject spike = Instantiate(spikeDoorPrefab, p - new Vector2(1f / 3f - 0.04f, -0.475f), new Quaternion(0, 0, 0, 0));
                spike.GetComponentInChildren<SpriteRenderer>().sortingLayerName = "Spikes";
                emptySpots.Remove(p);
                spike.tag = "Spike";
            }
        }

        GameObject goal = Instantiate(goalPrefab, emptySpots[Random.Range(0, emptySpots.Count)], Quaternion.identity);
        goal.tag = "Goal";
        emptySpots.Remove(goal.transform.position);

        transform.position = emptySpots[Random.Range(0, emptySpots.Count)];
        cam.transform.position = new Vector3(transform.position.x, transform.position.y, cam.transform.position.z);
    }

    #endregion

    bool IsBetween(float x, float a, float b)
    {
        return x >= a && x <= b;
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        switch (col.gameObject.tag)
        {
            case "Wall":
                BackgroundMusicPlayer.instance.PlayBallHitWall();
                break;
            case "Spike":
                BackgroundMusicPlayer.instance.PlayBallDie();
                break;

        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Goal")
            BackgroundMusicPlayer.instance.PlayBallWin();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isDead)
        {
            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Backspace))
            {

                if (!paused)
                    Pause();

                else
                {
                    if (inSettings)
                        CloseSettings();
                    else
                        Unpause();
                }


            }


            if (!paused)
            {
                // vT for velocityThreshold
                float vT = 0.2f;

                if (IsBetween(rb.velocity.x, -vT, vT) && IsBetween(rb.velocity.y, -vT, vT))
                {

                    Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                    if (Input.GetMouseButton(0) || Input.GetMouseButton(1))
                    {

                        Vector3 pos1 = new Vector3(transform.position.x, transform.position.y, 0);
                        Vector3 pos2 = new Vector3(mousePos.x, mousePos.y, 0);
                        if (Vector3.Distance(pos1, pos2) <= maxLineLength)
                        {
                            lineRenderer.SetPositions(new Vector3[] { pos1, pos2 });
                        }
                        else
                        {
                            lineRenderer.SetPositions(new Vector3[] { pos1, maxLineLength * Vector3.Normalize(pos2 - pos1) + pos1 });
                        }
                        lineRenderer.enabled = true;
                    }

                    if (Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1))
                    {


                        Vector3 direction = lineRenderer.GetPosition(0) - lineRenderer.GetPosition(1);

                        rb.AddForce(direction * ballSpeed);

                        lineRenderer.enabled = false;
                    }
                }
            }
        }
    }
}
