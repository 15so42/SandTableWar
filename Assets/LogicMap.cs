using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogicMap : MonoBehaviour
{
    [Header("地图容纳最大人数")]
    public int maxPlayer;

    [Header("基地位置列表")] public List<Transform> baseSlots;

    public Vector3 GetBasePosByPlayerId(int playerId)
    {
        return baseSlots[playerId].position;
    }
}
