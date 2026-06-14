using TMPro;
using UnityEngine;


public class GameOverUI : MonoBehaviour
{

    [SerializeField]
    private TMP_Text scoreTextBox;
    [SerializeField]
    private TMP_Text highScoreTextBox;
    [SerializeField]
    private GameObject gameOverCanvas;

    private int _highScore;


    private void Awake()
    {
        gameOverCanvas.SetActive(false);
    }

    private void OnEnable()
    {
        GameManager.OnGameOver += GameOver;
        GameManager.OnGameReset += GameReset;
    }

    private void OnDisable()
    {
        GameManager.OnGameOver -= GameOver;
        GameManager.OnGameReset -= GameReset;
    }

    private void GameOver()
    {
        int score = Grid.Instance.GetGridSum();

        scoreTextBox.text = $"Score: {score}";
        highScoreTextBox.text = $"High Score: {(_highScore = Mathf.Max(_highScore, score))}";

        gameOverCanvas.SetActive(true);
    }

    private void GameReset()
    {
        gameOverCanvas.SetActive(false);
    }

}