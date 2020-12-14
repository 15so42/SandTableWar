using UnityEngine;
[System.Serializable]
public class BattleUnitBaseProp
{
    public float weaponProficiency = 1f; //武器熟练度倍率，如果要做单位自定义的话有用，先留着吧
    public int maxHp=100;
    public int hp=100;
    public int defense=0;
    [Header("视野范围")]
    public float viewDistance = 10f;
    /// <summary>
    /// 扣除血量，不要传入负值
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public int ReduceHp(int value)
    {
        value = Mathf.Abs(value);
        hp -= value;
        if (hp <= 0)
        {
            return 0;
        }

        return hp;
    }

    /// <summary>
    /// 回复血量，不要传入负值
    /// </summary>
    /// <param name="value"></param>
    public void CureHp(int value)
    {
        value = Mathf.Abs(value);
        if (hp + value >= maxHp)
        {
            hp = maxHp;
        }
        else
        {
            hp += value;
        }
    }
}