using UnityEngine;

/// <summary>
/// Singleton that references all other managers of the game
/// </summary>
class MainGame : MonoBehaviour
{
    public static MainGame Instance { get; private set; }

    [field:SerializeField] public SmallEntitiesManager EntitiesManager { get; private set; }
    [field:SerializeField] public PlayerController Player { get; private set; }
    private void Awake()
    {
        Instance = this;
    }
    
}