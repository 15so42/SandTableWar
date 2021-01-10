
public enum DiplomaticRelation{//外交关系
    Enemy,//敌人
    Neutral,//中立
    Ally,//盟友
}
public class EnemyIdentifier : Singleton<EnemyIdentifier>
{
    /// <summary>
    /// 获取外交关系
    /// </summary>
    /// <param name="targetId"></param>
    public DiplomaticRelation GetDiplomaticRelation(int targetId)
    {
        if (targetId == -1) //-1表示中立
            return DiplomaticRelation.Neutral;
        FightingManager fightingManager = GameManager.Instance.GetFightingManager();
        if (fightingManager.campId % 2 == targetId % 2)//奇偶性相同为队友，否则为敌人
        {
            return DiplomaticRelation.Ally;
        }
        else
        {
            return DiplomaticRelation.Enemy;
        }
    }
    
 
    /// 非本机判断
    public DiplomaticRelation GetDiplomaticRelation(int a,int b)
    {
        if (a==-1 || b== -1) //-1表示中立
            return DiplomaticRelation.Neutral;
        if (a % 2 == b % 2)//奇偶性相同为队友，否则为敌人
        {
            return DiplomaticRelation.Ally;
        }
        else
        {
            return DiplomaticRelation.Enemy;
        }
    }
}
