using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FxDestinationMark : MonoBehaviour
{
    public Transform circle;
    private float agentRadius = 1;

  

    public void SetAgentRadius(float radius)
    {
        circle.transform.localScale = Vector3.one * radius;
    }
}
