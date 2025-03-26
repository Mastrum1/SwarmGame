﻿using System.Collections;
using System.Collections.Generic;
using Unity.Entities.UniversalDelegates;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.EventSystems;

public class BoidBehaviour_Singlethreaded : BoidBehaviour
{
    private BoidVision_Singlethreaded boidVision;

    protected override void Start()
    {
        boidVision = GetComponent<BoidVision_Singlethreaded>();
        base.Start();
    }

    protected override void UpdateBoid()
    {
        boidVision.UpdateSeenBoids();
        moveDirection = CalcRules();
    }

    void Update()
    {
        if (ControlInputs.Instance.useMouseFollow) mouseTarget = mouseTargetObj.mouseTargetPosition;
    }

    void FixedUpdate()
    {
        boidMovement.MoveBoid(moveDirection);
    }

    Vector3 ReturnToBounds()
    {
        Vector3 boundsAvoidVector = Vector3.zero;

        if (ControlInputs.Instance.useBoundingCoordinates)
        {
            //if close to edge of bounding box, move away from the edge
            if (transform.position.x > positiveBounds.x)
            {
                boundsAvoidVector.x -= Mathf.Abs(transform.position.x - positiveBounds.x);
            }
            else if (transform.position.x < negativeBounds.x)
            {
                boundsAvoidVector.x += Mathf.Abs(transform.position.x - negativeBounds.x);
            }

            if (transform.position.y > positiveBounds.y)
            {
                boundsAvoidVector.y -= Mathf.Abs(transform.position.y - positiveBounds.y);
            }
            else if (transform.position.y < negativeBounds.y)
            {
                boundsAvoidVector.y += Mathf.Abs(transform.position.y - negativeBounds.y);
            }

            if (transform.position.z > positiveBounds.z)
            {
                boundsAvoidVector.z -= Mathf.Abs(transform.position.z - positiveBounds.z);
            }
            else if (transform.position.z < negativeBounds.z)
            {
                boundsAvoidVector.z += Mathf.Abs(transform.position.z - negativeBounds.z);
            }

            boundsAvoidVector *= behaviourParams.boundsReturnSpeed;
        }

        return boundsAvoidVector;
    }

    //determines the boid's velocity vector after reacting to other boids in the environment (avoiding other boids, moving to centre of other boids,
    //matching velocity with other boids)
    //(this is all in one function so we don't have to loop through seen boids more than once)
    Vector3 ReactToOtherBoids()
    {
        Vector3 boidAvoidVector = Vector3.zero;
        Vector3 centre = Vector3.zero;
        Vector3 velocityMatch = Vector3.zero;

        if (boidVision.SeenBoids.Count == 0)
        {
            return Vector3.zero;
        }
        else
        {
            //check if numClosestToCheck > number of visible boids
            int numToCheck = Mathf.Min(behaviourParams.numClosestToCheck, boidVision.SeenBoids.Count);

            for (int i = 0; i < numToCheck; i++)
            {
                //avoid other boids
                if (Vector3.SqrMagnitude(transform.position - boidVision.SeenBoids[i].transform.position) < sqrBoidAvoidDistance)
                {
                    boidAvoidVector += transform.position - boidVision.SeenBoids[i].transform.position;
                    Debug.DrawLine(transform.position, boidVision.SeenBoids[i].transform.position, new Color(transform.position.x % 1, transform.position.y % 1, transform.position.z % 1));
                }

                //move towards centre of nearby boids
                centre += boidVision.SeenBoids[i].transform.position - transform.position;

                //match velocity with nearby boids
                velocityMatch += boidVision.SeenBoids[i].GetComponent<BoidMovement>().GetVelocity();
            }

            centre = centre / boidVision.SeenBoids.Count;
            velocityMatch = velocityMatch / boidVision.SeenBoids.Count;
        }

        return (boidAvoidVector * behaviourParams.boidAvoidSpeed) + centre + velocityMatch;
    }

    Vector3 Disperse(List<GameObject> seenBoids)
    {
        Vector3 disperse = Vector3.zero;

        int numToCheck = Mathf.Min(behaviourParams.numClosestToCheck, seenBoids.Count);
        for(int i = 0; i < numToCheck; i++)
        {
            disperse -= seenBoids[i].transform.position - transform.position;
        }

        return disperse;
    }

    //Check if the boid is heading towards an obstacle; if so, fire rays out in increasing angles to the left, right,
    //above and below the boid in order to find a path around an obstacle.
    //If multiple rays find a path, select the one closest to the boid's current velocity.
    Vector3 AvoidObstacles()
    {
        Vector3 avoidVector = Vector3.zero; //return vector

        //fire a ray in direction of boid's velocity (sqr magnitude of velocity in length; change this?) to see if there is an obstacle in its path.
        //if there is an obstacle in its path (regardless of whether it is currently avoiding a(nother) obstacle,
        //find avoid vector which is closest to either current velocity vector or mouse target, depending on if using mouse follow.
        Vector3 target;
        float checkDistance;
        if (ControlInputs.Instance.useMouseFollow)
        {
            target = mouseTarget - transform.position;
            checkDistance = Vector3.Distance(transform.position, mouseTarget);
        }
        else
        {
            target = boidMovement.GetVelocity();
            checkDistance = OBSTACLE_CHECK_DISTANCE;
        }

        RaycastHit hit;
        if (Physics.Raycast(transform.position, target, out hit, checkDistance, LAYER_OBSTACLE))
        {
            int raycastTries = 0;
            float inc = OBSTACLE_AVOID_RAY_INCREMENT;

            Vector3 closestVector = Vector3.positiveInfinity;
            bool foundAvoidVector = false;
            float minDiff = Mathf.Infinity;

            while (!foundAvoidVector && raycastTries <= MAX_OBSTACLE_RAYCAST_TRIES)
            {
                //up
                Vector3 up = new Vector3(target.x, target.y + inc, target.z - inc);
                //Debug.DrawRay(transform.position, up, Color.blue);
                if (!Physics.Raycast(transform.position, up, checkDistance, LAYER_OBSTACLE)) //if this raycast doesn't hit 
                {
                    closestVector = up;
                    minDiff = Vector3.SqrMagnitude(target - up);
                    foundAvoidVector = true;
                }

                //right
                Vector3 right = new Vector3(target.x + inc, target.y, target.z - inc);
                float rightDiff = Vector3.SqrMagnitude(target - right);
                //Debug.DrawRay(transform.position, right, Color.blue);
                if (rightDiff < minDiff && !Physics.Raycast(transform.position, right, checkDistance, LAYER_OBSTACLE)) //if this raycast doesn't hit 
                {
                    closestVector = right;
                    minDiff = rightDiff;
                    foundAvoidVector = true;
                }

                //down
                Vector3 down = new Vector3(target.x, target.y - inc, target.z - inc);
                float downDiff = Vector3.SqrMagnitude(target - down);
                //Debug.DrawRay(transform.position, down, Color.blue);
                if (downDiff < minDiff && !Physics.Raycast(transform.position, down, checkDistance, LAYER_OBSTACLE)) //if this raycast doesn't hit 
                {
                    closestVector = down;
                    minDiff = downDiff;
                    foundAvoidVector = true;
                }

                //left
                Vector3 left = new Vector3(target.x - inc, target.y, target.z - inc);
                //Debug.DrawRay(transform.position, left, Color.blue);
                if (Vector3.SqrMagnitude(target - left) < minDiff && !Physics.Raycast(transform.position, left, checkDistance, LAYER_OBSTACLE)) //if this raycast doesn't hit 
                {
                    closestVector = left;
                    foundAvoidVector = true;
                }

                inc += OBSTACLE_AVOID_RAY_INCREMENT;
                raycastTries++;
            }

            //if we found a way/ways around obstacle on this loop, return closestVector
            if (foundAvoidVector)
            {
                Debug.DrawRay(transform.position, closestVector, Color.green);
                return closestVector * behaviourParams.obstacleAvoidSpeed;
                //return closestVector.normalized * behaviourParams.obstacleAvoidSpeed;
            }
        }

        return Vector3.zero;
    }

    //causes the boid to repel itself from obstacles closer than OBSTACLE_CRITICAL_DISTANCE
    Vector3 ObstacleRepulsion()
    {
        Vector3 repulsionVec = Vector3.zero;

        /*
        //check cardinal directions for close objects
        if (Physics.Raycast(transform.position, transform.forward, OBSTACLE_CRITICAL_DISTANCE, LAYER_OBSTACLE)) repulsionVec += -transform.forward; //forward
        if (Physics.Raycast(transform.position, -transform.forward, OBSTACLE_CRITICAL_DISTANCE, LAYER_OBSTACLE)) repulsionVec += transform.forward; //back
        if (Physics.Raycast(transform.position, -transform.right, OBSTACLE_CRITICAL_DISTANCE, LAYER_OBSTACLE)) repulsionVec += transform.right; //left
        if (Physics.Raycast(transform.position, transform.right, OBSTACLE_CRITICAL_DISTANCE, LAYER_OBSTACLE)) repulsionVec += -transform.right; //right
        if (Physics.Raycast(transform.position, transform.up, OBSTACLE_CRITICAL_DISTANCE, LAYER_OBSTACLE)) repulsionVec += -transform.up; //up
        if (Physics.Raycast(transform.position, -transform.up, OBSTACLE_CRITICAL_DISTANCE, LAYER_OBSTACLE)) repulsionVec += transform.up; //down
        */

        if (Physics.Raycast(transform.position, boidMovement.GetVelocity(), out RaycastHit hit, OBSTACLE_CRITICAL_DISTANCE, LAYER_OBSTACLE)) repulsionVec = hit.normal;
        //Debug.DrawRay(transform.position, repulsionVec, Color.yellow,  0.2f);

        return repulsionVec * behaviourParams.obstacleAvoidSpeed;
    }

    Vector3 FollowCursor()
    {
        return (ControlInputs.Instance.useMouseFollow) ? (mouseTarget - transform.position).normalized * behaviourParams.cursorFollowSpeed : Vector3.zero;
    }

    Vector3 MoveToGoal()
    {
        return (ControlInputs.Instance.useRandomGoal) ? (boidCollectiveController.GetGoal() + transform.position).normalized * behaviourParams.goalFollowSpeed : Vector3.zero;
    }

    Vector3 MoveIdle() 
    {
        Vector3 idleDir = DirectionalPerlin.Directional3D(transform.position, behaviourParams.idleNoiseFrequency, behaviourParams.useTimeOffset ? Time.timeSinceLevelLoad : 0f);
        Debug.DrawRay(transform.position, idleDir, Color.magenta, 0.1f);
        return DirectionalPerlin.Directional3D(transform.position, behaviourParams.idleNoiseFrequency, behaviourParams.useTimeOffset ? Time.timeSinceLevelLoad : 0);
    }

    //calculates and returns a velocity vector based on a priority ordering of the boid's rules
    Vector3 CalcRules()
    {
        Vector3 avoidVector = behaviourParams.usePreemptiveObstacleAvoidance ? AvoidObstacles() : Vector3.zero; //non-zero if using obstacle avoidance and boid is heading towards an obstacle
        Vector3 repulsionVector = behaviourParams.useObstacleRepulsion ? ObstacleRepulsion() : Vector3.zero;
        if (!(avoidVector == Vector3.zero)) //prioritise obstacle avoidance
        {
            return avoidVector + repulsionVector + ReactToOtherBoids();
        }
        else if (boidVision.SeenBoids.Count == 0 && !ControlInputs.Instance.useMouseFollow) //if no boids nearby, not avoiding an obstacle, and not following mouse, do idle behaviour
        {
            return MoveIdle() + repulsionVector + ReturnToBounds();
        }
        else
        {
            return ReactToOtherBoids() + repulsionVector + FollowCursor() + MoveToGoal() + ReturnToBounds();
        }
    }
}