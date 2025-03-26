﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseTargetController : MonoBehaviour
{
    public MouseTargetPosition mouseTarget;
    public GameObject targetVisualiser; //object to use as 3D mouse cursor

    private Camera cam;

    private float targetZ = 10.0f; //z distance in front of camera 
    private float scrollRate = 1.0f;

    void Start()
    {
        cam = GetComponent<Camera>();
        targetVisualiser = Instantiate(targetVisualiser);
        targetZ += cam.nearClipPlane;
    }

    void Update()
    {
        if(ControlInputs.Instance.useMouseFollow)
        {
            Vector3 mousePosition = Input.mousePosition;
            targetZ += Input.mouseScrollDelta.y * scrollRate; //move target z forward (away from camera) when scrolling up, inward (towards camera) when scrolling down
            mouseTarget.mouseTargetPosition = cam.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, targetZ));
            targetVisualiser.transform.position = mouseTarget.mouseTargetPosition;
            targetVisualiser.GetComponent<Renderer>().enabled = true;
        }
        else
        {
            targetVisualiser.GetComponent<Renderer>().enabled = false;
        }
    }
}
