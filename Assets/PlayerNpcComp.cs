using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerNpcComp : MonoBehaviour
{
    protected PlayerNpcCommander playerNpcCommander;

    public void Init(PlayerNpcCommander playerNpcCommander)
    {
        this.playerNpcCommander = playerNpcCommander;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
