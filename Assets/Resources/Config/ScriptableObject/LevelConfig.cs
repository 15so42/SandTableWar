using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName="Config/LevelConfig")]
public class LevelConfig : ScriptableObject
{
   public List<FactionSlot> factionSlots=new List<FactionSlot>();
}
