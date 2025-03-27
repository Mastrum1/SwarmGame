using UnityEngine;

namespace Entities
{
    public class SmallEntityVisual : MonoBehaviour
    {
        [SerializeField] private GameObject _visual;
        [SerializeField] private SmallEntity _entity;
        [SerializeField] private float _lerp;

        private void Start()
        {
            //transform.SetParent(null);
        }

        private void FixedUpdate()
        {
            transform.position = _entity.transform.position;
            transform.forward = _entity.Direction;
        }
    }
}