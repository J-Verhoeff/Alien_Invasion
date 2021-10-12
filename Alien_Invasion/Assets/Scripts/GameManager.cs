/**
 * Alien Invation
 * 
 * Joshua Verhoeff
 * 
 * 2021-04-18
 * 
 *  Game Manager Script
 *  Handles UI updates, score, and player values
 */
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour {
    [SerializeField] private int maxHealth = 100;

    private GameObject tempLights;
    private GameObject gameUI;
    private GameObject pauseMenu;
    private TextMeshProUGUI infoText;
    private TextMeshProUGUI ammoCount;
    private TextMeshProUGUI highScore;
    private Image weaponImage;

    // Bool to check for player death
    private bool isDead;
    public bool IsDead {
        get {
            return isDead;
        }
        set {
            isDead = value;
            // Debug.Log("You Died");
            Score += Cash; // Add cash to score at end
            PauseGame();
        }
    }

    // player current currency value
    private int cash;
    public int Cash {
        get {
            return cash;
        }
        set {
            cash = value;
            gameUI.transform.Find("currentCash").GetComponent<TextMeshProUGUI>().text = "$" + cash;
        }
    }

    // players current score
    private int score;
    public int Score {
        get {
            return score;
        }
        set {
            score = value;
        }
    }

    // Player Current Health
    private int currentHealth;
    public int Health {
        get {
            return currentHealth;
        }
        set {
            currentHealth = value;
            if (currentHealth <= 0) {
                IsDead = true;
                currentHealth = 0;
            } else if (currentHealth > maxHealth) {
                currentHealth = maxHealth;
            }
            gameUI.transform.Find("Health").GetComponent<TextMeshProUGUI>().text = "Health\n" + currentHealth;
        }
    }

    // Current Round number
    private int round;
    public int Round {
        get {
            return round;
        }
        set {
            round = value;
            gameUI.transform.Find("RoundNumber").GetComponent<TextMeshProUGUI>().text = "" + round;
        }
    }

    private void Awake() {
        tempLights = GameObject.Find("TempLights");
        gameUI = GameObject.Find("FPSUI");
        pauseMenu = GameObject.Find("PauseMenu");
        infoText = gameUI.transform.Find("InfoText").GetComponent<TextMeshProUGUI>();
        ammoCount = gameUI.transform.Find("Ammo").GetComponent<TextMeshProUGUI>();
        weaponImage = gameUI.transform.Find("WeaponImage").GetComponent<Image>();
        highScore = GameObject.Find("HighScore").GetComponent<TextMeshProUGUI>();

        infoText.text = "";
    }

    // Start is called before the first frame update
    private void Start() {
        tempLights.SetActive(false); // disable lights used for development 
        gameUI.SetActive(true);

        // check highscore
        if (PlayerPrefs.HasKey(SceneManager.GetActiveScene().name)) {
            highScore.text = "Highscore " + PlayerPrefs.GetInt(SceneManager.GetActiveScene().name);
        } else {
            highScore.text = "";
        }

        // Set initial values
        pauseMenu.SetActive(false);
        Cash = 100;
        Score = 0;
        Health = maxHealth;
        Round = 1;
    }

    private void Update() {
        // Pause the game with P or Esc
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P)) {
            PauseGame();
        }
    }

    // Pause the game and display the pause menu
    private void PauseGame() {
        Time.timeScale = 0;
        Cursor.lockState = CursorLockMode.None;
        pauseMenu.SetActive(true);
        gameUI.SetActive(false);
        pauseMenu.transform.Find("Score").GetComponent<TextMeshProUGUI>().text = "Score " + Score;
        if (IsDead) {
            pauseMenu.transform.Find("DisplayText").GetComponent<TextMeshProUGUI>().text = "You Are Dead";
            pauseMenu.transform.Find("Restart").gameObject.SetActive(true);
            pauseMenu.transform.Find("Continue").gameObject.SetActive(false);

            // Save score if high score
            if (NewHighScore(Score)) {
                highScore.text = "New High Score!";
            }
        } else {
            pauseMenu.transform.Find("Restart").gameObject.SetActive(false);
            pauseMenu.transform.Find("Continue").gameObject.SetActive(true);
        }
    }

    // Unpause the game
    public void Continue() {
        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.Locked;
        pauseMenu.SetActive(false);
        gameUI.SetActive(true);
    }

    // Eexit the level
    public void ExitToMenu() {
        SceneManager.LoadScene("MainMenu");
    }

    // Restart the level by reloading the scene
    public void Restart() {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // Set the info text value
    public void SetInfoText(string info) {
        infoText.text = info;
    }

    // Update the ammo count UI element
    public void SetAmmoCount(int currentAmmo, int TotalAmmo) {
        ammoCount.text = "" + currentAmmo + "/" + TotalAmmo;
    }

    // Update the weapon UI image
    public void SetWeaponImage(Sprite image) {
        weaponImage.sprite = image;
    }

    // Check if player is at max health
    public bool AtMaxHealth() {
        return currentHealth == maxHealth;
    }

    // Set high score if achieved
    // Use scene name as key
    private bool NewHighScore(int newScore) {
        string key = SceneManager.GetActiveScene().name;

        if (!PlayerPrefs.HasKey(key)) {
            PlayerPrefs.SetInt(key, newScore);
            PlayerPrefs.Save();
            return true;
        } else {
            if (PlayerPrefs.GetInt(key) < newScore) {
                PlayerPrefs.SetInt(key, newScore);
                PlayerPrefs.Save();
                return true;
            } else {
                return false;
            }
        }
    }
}
