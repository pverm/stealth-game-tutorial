using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameUI : MonoBehaviour
{

    public GameObject gameLoseUI;
    public GameObject gameWinUI;
    bool gameOver = false;

    // Start is called before the first frame update
    void Start()
    {
        Guard.OnPlayerSpotted += ShowLoseUI;
        PlayerController.OnGoalReached += ShowWinUI;
    }

    // Update is called once per frame
    void Update()
    {
        if (gameOver) {
            if (Input.GetKeyDown(KeyCode.Space)) {
                SceneManager.LoadScene(0);
            }
        }
    }

    void ShowLoseUI() {
        OnGameOver(gameLoseUI);
    }
    
    void ShowWinUI() {
        OnGameOver(gameWinUI);
    }

    void OnGameOver(GameObject ui) {
        ui.SetActive(true);
        gameOver = true;
        Guard.OnPlayerSpotted -= ShowLoseUI;
        PlayerController.OnGoalReached -= ShowWinUI;
    }
}
