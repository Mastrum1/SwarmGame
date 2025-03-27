using UnityEngine;

namespace Garbage
{
    public class FloorManager : MonoBehaviour
    {
        [SerializeField] private Garbage _emptyGarbagePrefab;
        [SerializeField] private int _amount;
        private void Start()
        {
            var planet = MainGame.Instance.Planet;
            CreateAroundSphere(planet.radius * planet.transform.localScale.y, _amount);
        }

        private void CreateAroundSphere(float radiusMultiplier, int amount = 1000)
        {
            for (int i = 0; i < amount; i++)
            {
                var targetPosition = FibSphere(i, radiusMultiplier, amount);
                MainGame.Instance.CleaningManager.CreateGarbage(_emptyGarbagePrefab.gameObject, targetPosition);
            }
        }
        
        private Vector3 FibSphere(int i, float radius, int total) {
            var k = i + .5f;

            var phi = Mathf.Acos(1f - 2f * k / total);
            var theta = Mathf.PI * (1 + Mathf.Sqrt(5)) * k;

            var x = Mathf.Cos(theta) * Mathf.Sin(phi);
            var y = Mathf.Sin(theta) * Mathf.Sin(phi);
            var z = Mathf.Cos(phi);

            return new Vector3(x, y, z) * radius;
        }
    }
}