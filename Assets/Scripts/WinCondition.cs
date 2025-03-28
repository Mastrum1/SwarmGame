using Es.InkPainter;
using Garbage;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class WinCondition : MonoBehaviour
{
    private TotalCleaningManager _cleaningManager;
    public UnityEvent OnWin;
    public float WinPercentage = 0.7f;

    private void Start()
    {
        _cleaningManager = MainGame.Instance.CleaningManager;
        _cleaningManager.OnPercentageChanged += CheckWinCondition;
    }

    private void CheckWinCondition(TotalCleaningManager manager)
    {
        if (manager.CleanedPercentage < WinPercentage) return;
        Debug.Log("You win!");
        OnWin?.Invoke();
    }
    public void OnDestroy()
    {
        _cleaningManager.OnPercentageChanged -= CheckWinCondition;
    }

    public void RetartGame()
    {
        int index = (UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex + 1) % SceneManager.sceneCountInBuildSettings;

        UnityEngine.SceneManagement.SceneManager.LoadScene(index);
    }
}
