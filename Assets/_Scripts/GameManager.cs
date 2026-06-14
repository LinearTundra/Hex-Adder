using System;
using UnityEngine;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using Unity.VisualScripting;

public class GameManager : MonoBehaviour
{

    public static GameManager Instance;
    public static event Action OnGameReset;
    public static event Action OnGameOver;

    private float _hexScale;
    private HashSet<int> _unlockedNumbers;


    private void Awake()
    {
        Instance = this;
        _unlockedNumbers = new HashSet<int> { 1, 2, 3 };

    }

    private void OnEnable()
    {
        OnGameReset -= ResetUnlockedNumbers;
        OnGameReset += ResetUnlockedNumbers;
    }

    private void OnDisable()
    {
        OnGameReset -= ResetUnlockedNumbers;
    }

    public void SetHexScale(float hexScale)
    {
        _hexScale = hexScale;
    }

    public float GetHexScale()
    {
        return _hexScale;
    }

    public void UnlockNumber(int num)
    {
        _unlockedNumbers.Add(num);
    }

    public void ResetUnlockedNumbers()
    {
        _unlockedNumbers = new HashSet<int> { 1, 2, 3 };
    }

    public int GetRandomUnlockedNumber()
    {
        List<int> list = new List<int>(_unlockedNumbers);
        return list[Random.Range(0, list.Count)];
    }

    /// <summary>
    /// Returns a random valid (a, b) pair from the unlocked pool that can be placed on the board.
    /// Returns (-1, -1) if no valid pair exists — game over condition.
    /// </summary>
    public (int a, int b) GetValidPiece()
    {
        List<int> numbers = new List<int>(_unlockedNumbers);
        List<(int, int)> validPairs = new List<(int, int)>();

        for (int i = 0; i < numbers.Count; i++)
            for (int j = i; j < numbers.Count; j++)
                if (Grid.Instance.Placable(numbers[i], numbers[j]))
                    validPairs.Add((numbers[i], numbers[j]));

        if (validPairs.Count == 0) return (-1, -1);

        return validPairs[Random.Range(0, validPairs.Count)];
    }

    public void ResetGame()
    {
        OnGameReset?.Invoke();
    }

    public void GameOver()
    {
        OnGameOver?.Invoke();
    }

}
