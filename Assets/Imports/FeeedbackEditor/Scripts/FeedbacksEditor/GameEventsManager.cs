using UnityEngine;

namespace FeedbacksEditor
{
    /// <summary>
    /// The Game Events Manager, listing all the different events, and playing them from anywhere thanks to the "PlayEvent" method
    /// </summary>
    public class GameEventsManager : MonoBehaviour
    {
        private static GameEventsManager _instance;

        private void Awake()
        {
            _instance = this;
        }

        /// <summary>
        /// Plays an event
        /// </summary>
        /// <param name="eventName">The name of the event scriptable object</param>
        /// <param name="target">The gameObject that is playing the event</param>
        public static void PlayEvent(GameEvent gameEvent, GameObject target)
        {
            _instance.StartCoroutine(gameEvent.Execute(target));
        }
    }
}