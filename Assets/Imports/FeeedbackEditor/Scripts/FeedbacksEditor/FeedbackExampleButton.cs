using UnityEngine;
using UnityEngine.UI;

namespace FeedbacksEditor
{
    /// <summary>
    /// An example of how to call the GameEventsManager in code.
    /// </summary>
    [RequireComponent(typeof(Button))]
    public class FeedbackExampleButton : MonoBehaviour
    {
        [SerializeField] private Button _button;
        [SerializeField] private GameObject _targetGameObject;
        [SerializeField] private GameEvent _feedback;

        private void Start()
        {
            _button.onClick.AddListener(() => GameEventsManager.PlayEvent(_feedback, _targetGameObject));
        }

        private void OnDisable()
        {
            _button.onClick.RemoveListener(() => GameEventsManager.PlayEvent(_feedback, _targetGameObject));
        }
    }
}