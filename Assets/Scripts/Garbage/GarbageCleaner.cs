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
            MainGame.Instance.OnGarbageCleaned(garbage);
            Destroy(garbage.gameObject);
        }
    }
}