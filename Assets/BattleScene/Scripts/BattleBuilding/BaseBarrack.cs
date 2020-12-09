using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseBarrack : BattleUnitBase
{
    public FightingManager fightingManager;
    
    
    // Start is called before the first frame update
    void Start()
    {
        fightingManager = GameManager.Instance.GetFightingManager();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
