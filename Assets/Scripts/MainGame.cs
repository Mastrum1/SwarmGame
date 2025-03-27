using System;
using Garbage;
using UnityEngine;

/// <summary>
/// Singleton that references all other managers of the game
/// </summary>
class MainGame : MonoBehaviour
{
    public static MainGame Instance { get; private set; }
    [field:SerializeField] public PlayerController Player { get; private set; }
    [field:SerializeField] public SphereCollider Planet { get; private set; }
    [field:SerializeField] public TotalCleaningManager CleaningManager { get; private set; }
    private void Awake()
    {
        Instance = this;
    }
    
    public Action<Garbage.Garbage> OnGarbageCleaned;

}