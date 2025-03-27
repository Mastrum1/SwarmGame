using UnityEngine;

public class OrientToPlanetStart : MonoBehaviour
{
    private void Start()
    {
        Vector3 direction = (MainGame.Instance.Planet.position - transform.position).normalized;
        Quaternion rotation = Quaternion.FromToRotation(-transform.up, direction) * transform.rotation;
        transform.rotation = rotation;
    }
}