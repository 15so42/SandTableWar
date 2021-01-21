using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeMark : MonoBehaviour
{
    
    private CollisionDetection collisionDetection;
    public GameObject selectMark;
    private Material selectMarkMat;
    private void Awake()
    {
        collisionDetection = GetComponent<CollisionDetection>();
        selectMarkMat = selectMark.GetComponent<MeshRenderer>().material;
    }

    private void Update()
    {
        bool canPlace = collisionDetection.CanPlace();
        selectMarkMat.SetColor("_Coloer", canPlace?Color.green:Color.red);
        selectMarkMat.SetColor("_EmissionColor",canPlace?Color.green:Color.red);
    }
}
