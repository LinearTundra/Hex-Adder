using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{

    private static float _hexScale;
    private static HashSet<int> _unlockedNumbers;


    private void Awake()
    {
        ResetUnlockedNumbers();
    }

    public static void SetHexScale(float hexScale)
    {
        _hexScale = hexScale;
    }

    public static float GetHexScale()
    {
        return _hexScale;
    }

    public static void UnlockNumber(int num)
    {
        _unlockedNumbers.Add(num);
    }

    public static void ResetUnlockedNumbers()
    {
        _unlockedNumbers = new HashSet<int> { 1, 2, 3 };
    }

    public static int GetRandomUnlockedNumber()
    {
        List<int> list = new List<int>(_unlockedNumbers);
        return list[Random.Range(0, list.Count)];
    }

}
