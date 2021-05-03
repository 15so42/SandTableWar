
using System.Linq;

public enum DiplomaticRelation{//外交关系
    Self,//自己
    Enemy,//敌人
    Neutral,//中立
    Ally,//盟友
}
public class EnemyIdentifier : Singleton<EnemyIdentifier>
{

    public FightingManager fightingManager;

    public FightingManager GetFightingManager()
    {
        if (fightingManager==null)
        {
            fightingManager=FightingManager.Instance;
        }

        return fightingManager;
    }
    public FactionManager GetFactionManager(int targetFactionId)
    {
        return fightingManager.GetFaction(targetFactionId);
    }
    /// <summary>
    /// 针对本客户端,获取外交关系
    /// </summary>
    /// <param name="targetId"></param>
    public DiplomaticRelation GetMyDiplomaticRelation(int targetId)
    {
        if (targetId == -1) //-1表示中立
            return DiplomaticRelation.Neutral;
        
        if (GetFightingManager().myFactionId == targetId)
        {
            return DiplomaticRelation.Self;
        }
        if (GetFightingManager().GetMyFaction().ally.Contains(GetFactionManager(targetId)))//奇偶性相同为队友，否则为敌人
        {
            return DiplomaticRelation.Ally;
        }
        else
        {
            return DiplomaticRelation.Enemy;
        }
    }
    
    /// <summary>
    /// 非本机判断，对于a来说b和a的关系是什么？通过campId来判断
    /// <param name="a"></param> 
    /// <param name="b"></param> 
    /// </summary>
    public DiplomaticRelation GetDiplomaticRelation(int a,int b)
    {
        if (a==-1 || b== -1) //-1表示中立
            return DiplomaticRelation.Neutral;
        if (a == b)
            return DiplomaticRelation.Self;
        if (GetFactionManager(a).ally.Contains(GetFactionManager(b)))
        {
            return DiplomaticRelation.Ally;
        }
        else
        {
            return DiplomaticRelation.Enemy;
        }
    }
}
