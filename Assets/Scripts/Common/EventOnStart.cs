using UnityEngine;
using UnityEngine.Events;

public class EventOnStart : MonoBehaviour
{
    [SerializeField] private UnityEvent _event;

    void Start()
    {
        _event?.Invoke();
    }
}
