using UnityEngine;

namespace Scripts
{
    public class CameraController : MonoBehaviour
    {
        public float Speed
        {
            get => speed;
            set => speed = value;
        }
        
        [Header("Target")]
        [SerializeField] private GameObject target;
        [SerializeField] private Transform anchorPosition;
    
        [Header("Movement")]
        [SerializeField] private float speed = 5f;
        [SerializeField] private float rotationSpeed = 5f;
        
        Vector3 _lookDirection = Vector3.zero;
    
        private void FixedUpdate()
        {
            if (!target) return;
        
            // Move the camera to the target local position
            transform.position = Vector3.Lerp(transform.position, anchorPosition.position, speed * Time.deltaTime);
            
            // use the target up and target right to rotate camera
            Vector3 targetUp = -target.transform.up;
            
            // Rotate the camera to the target rotation
            _lookDirection = target.transform.position - transform.position;
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(_lookDirection, targetUp), rotationSpeed * Time.deltaTime);
        }
    }
}


