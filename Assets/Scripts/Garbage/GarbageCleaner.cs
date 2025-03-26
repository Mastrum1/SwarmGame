using UnityEngine;

namespace Garbage
{
    /// <summary>
    /// A GameObject that can clean garbage
    /// </summary>
    public class GarbageCleaner : MonoBehaviour
    {
        public void Clean(Garbage garbage)
        {
            MainGame.Instance.EntitiesManager.AddEntities(garbage.EntitiesToAdd);
            Destroy(garbage.gameObject);
        }
    }
}