using System;
using UnityEngine;

namespace Scripts
{
    public class Beginning : MonoBehaviour
    {
        [SerializeField] private MonoBehaviour[] _components;

        private void Start()
        {
            foreach (var component in _components)
            {
                component.enabled = false;
            }
        }

        private void Update()
        {
            if (Input.anyKeyDown)
            {
                foreach (var component in _components)
                {
                    component.enabled = true;
                }

                Destroy(this);
            }
        }
    }
}