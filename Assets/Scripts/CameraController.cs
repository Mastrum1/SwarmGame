using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private GameObject target;
    [SerializeField] private Transform anchorPosition;
    
    [Header("Movement")]
    [SerializeField] private float speed = 5f;
    [SerializeField] private float rotationSpeed = 5f;
    
    private void FixedUpdate()
    {
        if (!target) return;
        
        // Move the camera to the target local position
        transform.position = Vector3.Lerp(transform.position, anchorPosition.position, speed * Time.deltaTime);
        
        // Rotate the camera to look at the target
        Vector3 direction = target.transform.position - transform.position;
        Quaternion rotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Lerp(transform.rotation, rotation, rotationSpeed * Time.deltaTime);
    }
    
}
