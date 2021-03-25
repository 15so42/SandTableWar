using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName="Config/BattleFxConfig")]
public class BattleFxConfig : ScriptableObject
{
    public List<BattleFxConfigPair> pairs;

    public GameObject GetFxPfbByBattleFxType(BattleFxType battleFxType)
    {
        return pairs.Find(x => x.battleFxType == battleFxType).fxPfb;
    }
}

[System.Serializable]
public class BattleFxConfigPair
{
    public BattleFxType battleFxType;
    public GameObject fxPfb;
}

public enum BattleFxType
{
    None,
    //血
    Blood_1,
    
    //爆炸
    Explosion_1,
    DestionMark,
}

public enum VictimMaterial
{
    Human,//人体
    Metal,//金属
    Concrete//混凝土
}