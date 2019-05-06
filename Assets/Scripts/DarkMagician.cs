using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DarkMagician : MonoBehaviour
{
    // Game control
    public static int HIGH_SCORE = -1;
    public static int SCORE = 0;        // updated when killing bugs and after surviving a round
    public const int SCORE_PER_ENEMY = 5;
    public const int SCORE_PER_ROUND = 100;
    public static int ROUND = 1;
    public static bool LOST = false;    // set to true when an enemy contacts the player satellite
    private int timer;                  // simple seconds timer for round time remaining
    private float laserCharge = 0;      // time since last laser fire; laser can be fired again when laserCharge == LASER_RECHARGE
    private bool laserReady = false;

    // Starting values for player abilities
    public static int SWEEP_SPEED = 20;
    public static float LASER_RECHARGE = 10;
    public static float VISIBILITY_SCALE = 0.2f;

    // Values for player upgrade amounts
    public const int SWEEP_SPEED_UPGRADE = 12;
    public const float LASER_RECHARGE_UPGRADE = -0.8f;
    public const float VISIBILITY_UPGRADE = 0.08f;

    // Values for enemies
    public static float ENEMY_SPAWN_RATE = 4.5f;
    public static float ENEMY_SPEED = 0.02f;
    private const float enemySpawnX = 7.5f;
    private const float enemySpawnY = 6f;

    // Values for enemy difficulty creep
    private const float ENEMY_SPAWN_RATE_UP = -0.15f;
    //private const float ENEMY_SPEED_UP;
    private const float ENEMY_SPAWN_RATE_CAP = 0.5f;

    // References set in the editor
    public GameObject player;
    public GameObject visibility;
    public GameObject enemyPrefab;

    // UI
    public GameObject roundStartMenu;
    public GameObject roundEndMenu;
    public GameObject gameOverMenu;
    public GameObject highScoreText;
    public Text roundText;
    public Text timerText;
    public Text scoreText;
    public Image laserChargeImage;

    // Audio
    public AudioSource bgm;
    public AudioSource laserReadySFX;
    public AudioSource laserSFX;


    // Displays the Round Start Menu, called every time the scene loads
    private void Start ()
    {
        Time.timeScale = 0;
        LOST = false;
        timer = 25 + (ROUND * 5);
        UpdateUI();
        roundText.text = "ROUND " + ROUND;
        roundStartMenu.SetActive(true);

        // Set the scale of the visibility cone based on current upgrade level
        visibility.transform.localScale = new Vector3(VISIBILITY_SCALE, 0.76f, 1);
    }

    // Starts a new round, called when the player clicks on the Round Start Menu
    public void StartRound()
    {
        Time.timeScale = 1;
        roundStartMenu.SetActive(false);
        visibility.SetActive(true);
        InvokeRepeating("ClockTickDown", 1, 1);
        InvokeRepeating("SpawnEnemyOffscreen", ENEMY_SPAWN_RATE, ENEMY_SPAWN_RATE);
        bgm.Play();
    }

    
    private void Update ()
    {
        // Check for game loss
        if (LOST)
            EndGame();
        
        // Ignore all input if a menu is open
        if (Time.timeScale == 0)
            return;

        // Player input: rotate player satellite to face cursor
        Vector2 cursorPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        cursorPosition = cursorPosition.normalized;
        float rotAngle = (Mathf.Atan2(cursorPosition.y, cursorPosition.x) * Mathf.Rad2Deg) - 90;
        player.transform.rotation = Quaternion.RotateTowards(player.transform.rotation, Quaternion.Euler(new Vector3(0, 0, rotAngle)), SWEEP_SPEED * Time.deltaTime);

        // Update laser charge UI
        if (laserCharge <= LASER_RECHARGE)
        {
            laserChargeImage.fillAmount = laserCharge / LASER_RECHARGE;
            laserCharge += Time.deltaTime;
        }

        // Play SFX when laser becomes fully-charged
        if(laserCharge >= LASER_RECHARGE && !laserReady)
        {
            laserReady = true;
            laserReadySFX.Play();
        }

        // Player input: shoot laser
        if (Input.GetMouseButtonDown(0) && laserCharge >= LASER_RECHARGE)
        {
            visibility.GetComponent<Collider2D>().enabled = true;
            Invoke("TurnOffLaser", 0.1f);
            laserCharge = 0;
            laserReady = false;
            laserSFX.Play();
        }

        // Update score UI
        UpdateUI();
    }

    // Turns off the laser after just enough time to OnTriggerEnter() the enemies
    private void TurnOffLaser()
    {
        visibility.GetComponent<Collider2D>().enabled = false;
    }

    // Spawns bug enemies just outside of the field of view
    private void SpawnEnemyOffscreen()
    {
        float xPos, yPos;
        int randXY = Random.Range(0, 2);
        int randDir = Random.Range(0, 2);

        if (randXY < 1)     // fixed x, random y spawn
        {
            if (randDir < 1)
            { xPos = -enemySpawnX; }
            else
            { xPos = enemySpawnX; }
            yPos = Random.Range(-enemySpawnY, enemySpawnY);
        }
        else                // fixed y, random x spawn
        {
            if (randDir < 1)
            { yPos = -enemySpawnY; }
            else
            { yPos = enemySpawnY; }
            xPos = Random.Range(-enemySpawnX, enemySpawnX);
        }
        Instantiate(enemyPrefab, new Vector3(xPos, yPos, 0), new Quaternion(0, 0, 0, 0));
    }

    // Updates the simple seconds counter and display, called every second by an InvokeRepeating() in StartRound()
    private void ClockTickDown()
    {
        timer -= 1;
        SCORE += 1;
        if (timer == 0)
            EndRound();
    }

    private void UpdateUI()
    {
        timerText.text = timer.ToString();
        scoreText.text = SCORE.ToString();
    }

    // Displays the Round End Menu, called when the clock hits zero
    private void EndRound()
    {
        Time.timeScale = 0;
        bgm.Stop();
        roundEndMenu.SetActive(true);

        // Update score for surviving the round
        SCORE += SCORE_PER_ROUND;
    }

    // Adjusts difficulty creep values and reloads the scene, called from the Round End Menu
    public static void AdvanceRound()
    {
        // Round end upgrades have already been applied

        ROUND++;
        if (ENEMY_SPAWN_RATE > ENEMY_SPAWN_RATE_CAP)
            ENEMY_SPAWN_RATE += ENEMY_SPAWN_RATE_UP;
        //ENEMY_SPEED += ENEMY_SPEED_UP;        // decided not to increase enemy speed, just spawn rate and round time 

        SceneManager.LoadScene("Main");
    }

    // Displays the Game Over Menu, called when the player loses (an enemy contacts the satellite)
    private void EndGame()
    {
        Time.timeScale = 0;
        bgm.Stop();

        // Check for high score
        if (SCORE > HIGH_SCORE)
        {
            HIGH_SCORE = SCORE;
            highScoreText.SetActive(true);
        }
        gameOverMenu.SetActive(true);
    }

    // Reloads the menu scene, called when the player clicks on the Game Over Menu
    public void BackToMenu()
    {
        SceneManager.LoadScene("Menu");
    }
}
