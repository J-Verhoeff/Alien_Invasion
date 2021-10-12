/**
 * Alien Invation
 * 
 * Joshua Verhoeff
 * 
 * 2021-04-18
 * 
 * Script to implement main menu and handle various UI button presses
 */

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEditor;
using TMPro;

public class MainMenu : MonoBehaviour {
    // List of levels (use scene names)
    [SerializeField] private string[] levelNames;
    [SerializeField] private Sprite[] levelImages;

    private GameObject[] itemsToHide;
    private GameObject levelSelectPage;
    private GameObject instructionsPage;
    private Image levelSelectImage;
    private TextMeshProUGUI levelSelectTitle;
    private int currentLevelSelected = 0;

    private void Awake() {
        itemsToHide = GameObject.FindGameObjectsWithTag("MainmenuButtons");
        levelSelectPage = GameObject.Find("LevelSelect");
        levelSelectImage = GameObject.FindGameObjectWithTag("LevelImage").GetComponent<Image>();
        levelSelectTitle = GameObject.Find("LevelTitle").GetComponent<TextMeshProUGUI>();
        instructionsPage = GameObject.Find("Instructions");
    }

    // Function to hide main menu items
    private void HideMenuItems() {
        foreach (GameObject item in itemsToHide) {
            item.SetActive(false);
        }
        levelSelectPage.SetActive(false);
    }

    // function to show main menu items
    private void ShowMenuItems() {
        foreach (GameObject item in itemsToHide) {
            item.SetActive(true);
        }
    }

    private void Start() {
        ShowMenuItems();
        levelSelectPage.SetActive(false);
        instructionsPage.SetActive(false);
    }

    // Load the level select to load a new game
    public void NewGame() {
        //Debug.Log("New Game");
        HideMenuItems();
        levelSelectPage.SetActive(true);
    }

    // Go back to menu
    public void Back() {
        levelSelectPage.SetActive(false);
        instructionsPage.SetActive(false);
        ShowMenuItems();
    }

    // Load currently selected level
    public void Play() {
        SceneManager.LoadScene(levelNames[currentLevelSelected]);
    }

    // Go to next level in list
    public void Next() {
        currentLevelSelected++;
        if (currentLevelSelected < levelNames.Length) {
            levelSelectImage.sprite = levelImages[currentLevelSelected];
            levelSelectTitle.text = "Level " + (currentLevelSelected + 1) + "\nThe " + levelNames[currentLevelSelected];
        } else {
            currentLevelSelected = 0;
            levelSelectImage.sprite = levelImages[currentLevelSelected];
            levelSelectTitle.text = "Level " + (currentLevelSelected + 1) + "\nThe " + levelNames[currentLevelSelected];
        }
    }

    // Go to previous level in list
    public void Prev() {
        if (currentLevelSelected > 0) {
            currentLevelSelected--;
            levelSelectImage.sprite = levelImages[currentLevelSelected];
            levelSelectTitle.text = "Level " + (currentLevelSelected + 1) + "\nThe " + levelNames[currentLevelSelected];
        } else {
            currentLevelSelected = levelNames.Length - 1;
            levelSelectImage.sprite = levelImages[currentLevelSelected];
            levelSelectTitle.text = "Level " + (currentLevelSelected + 1) + "\nThe " + levelNames[currentLevelSelected];
        }
    }

    // Load instructions page
    public void Instructions() {
        //Debug.Log("Instruction");
        HideMenuItems();
        instructionsPage.SetActive(true);
    }

    // Either exit the applicationm or stop running game if in editor
    public void Exit() {
        EditorApplication.isPlaying = false;
        Application.Quit();
    }
}
