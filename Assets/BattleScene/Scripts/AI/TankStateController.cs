using System.Collections.Generic;

public class TankStateController : StateController
{
    public TankStateController(BattleUnitBase battleUnitBase) : base(battleUnitBase)
    {
    }

    protected override void InitState()
    {
        
        base.InitState();
        
        fightState.actions.Clear();
        fightState.AddAction(new BaseTankFightAction());
        //转换条件按添加顺序执行，满足一个转换条件后就转换，因此此处是在战斗中先判断目标路径点是否发生了变化，是则强行移动
        fightState.AddTransition(new Transition()
        {
            decisions = new List<Decision>{new HasNewTargetPosDecision()},
            falseState = fightState,
            trueState = moveIgnoreEnemyState
        });
        //如果路径点没有发生变化，则判断敌人是否全被消灭,全被消灭后转换为MoveState朝之前的目标点移动
        fightState.AddTransition(new Transition()
        {
            decisions = new List<Decision>{new FindEnemyDecision()},
            falseState = moveState,
            trueState = fightState
        });
    }
}