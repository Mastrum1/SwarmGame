using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace DefaultNamespace
{
    public class Drone : MonoBehaviour

    {
        [SerializeField] private int beesAmount;
        [SerializeField] private float speed;
        [SerializeField] private Rigidbody rb;
        [SerializeField] private float gravity = 9.8f;
        [SerializeField] private float gravityStrength = 1f;

        private Vector3 _targetPosition;

        private void Start()
        {
            ChooseRandomTargetAroundPlanet();
            StartCoroutine(ChangeTarget());
        }

        private void FixedUpdate()
        {
            ApplyGravity();
            RotateTowardsTarget();
            RotateToPlanet();
            Move();
        }

        private void ApplyGravity()
        {
            var planetPosition = MainGame.Instance.Planet.transform.position;
            var distance = Vector3.Distance(transform.position, planetPosition);

            Vector3 direction = (planetPosition - transform.position).normalized;
            rb.AddForce(direction * (gravity * gravityStrength * distance));
        }

        private void RotateTowardsTarget()
        {
            var direction = (_targetPosition - transform.position).normalized;
            var rotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 5f);
        }
        
        private void RotateToPlanet()
        {
            Vector3 direction = (MainGame.Instance.Planet.transform.position - transform.position).normalized;
            Quaternion rotation = Quaternion.FromToRotation(transform.up, direction) * transform.rotation;
            transform.rotation = rotation;
        }
        
        private void ChooseRandomTargetAroundPlanet()
        {
            var planet = MainGame.Instance.Planet;
            var randomPosition = Random.onUnitSphere * planet.transform.localScale.x;
            _targetPosition = planet.transform.position + randomPosition;
        }

        private void Move()
        {
            if (Vector3.Distance(transform.position, _targetPosition) < 1f) ChooseRandomTargetAroundPlanet();
            
            var direction = (_targetPosition - transform.position).normalized;
            rb.linearVelocity = direction * speed;
        }

        private IEnumerator ChangeTarget()
        {
            yield return new WaitForSeconds(10f);
            ChooseRandomTargetAroundPlanet();
            StartCoroutine(ChangeTarget());
        }

        private void OnDestroy()
        {
            StopCoroutine(ChangeTarget());
        }


        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(_targetPosition, 0.5f);
            
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, _targetPosition);
        }
    }
}