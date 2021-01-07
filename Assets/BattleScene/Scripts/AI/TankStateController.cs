using System.Collections.Generic;

public class TankStateController : StateController
{
    public TankStateController(BattleUnitBase battleUnitBase) : base(battleUnitBase)
    {
    }

    protected override void InitState()
    {
        //状态机设置
        State idleState = new BaseIdleState(this,"闲置");
        State moveState = new BaseMoveState(this,"移动");
        State moveIgnoreEnemyState =new BaseMoveState(this,"强行移动");
        State fightState = new BaseFightState(this,"战斗");

        //idle时发现有新的目标位置切换到移动状态
        idleState.AddTransition(new Transition()
        {
            decisions =  new List<Decision>{new HasTargetPosDecision()}, 
            trueState = moveState,
            falseState = idleState
        });
        
        //idle时发现敌人切换到战斗状态
        idleState.AddTransition(new Transition()
        {
            decisions =  new List<Decision>{new FindEnemyDecision()},
            trueState = fightState, 
            falseState = idleState
        });

        //移动状态执行移动函数
        moveState.AddAction(new MoveToPosStateAction());
        moveState.AddTransition(new Transition()
        {
            decisions =  new List<Decision>{new FindEnemyDecision()},
            falseState = moveState,
            trueState = fightState
        });
        moveState.AddTransition(new Transition()
        {
            decisions = new List<Decision>{new ReachTargetPosDecision()},
            falseState = moveState,
            trueState = idleState
        });
    
        
        //强制移动到目标点，用于战斗中强行移动(撤退，突围等操作)
        moveIgnoreEnemyState.AddAction(new MoveToPosStateAction());
        moveIgnoreEnemyState.AddTransition(new Transition()
        {
            decisions =  new List<Decision>{new ReachTargetPosDecision()},
            falseState = moveIgnoreEnemyState,
            trueState = idleState
        });

        //战斗状态
        //是否有敌人T:fight
        //        有新的目标点T:强制移动
        //                  F：fight
        //    F:idle
        fightState.AddAction(new BaseTankFightAction());
        //转换条件按添加顺序执行，满足一个转换条件后就转换，因此此处是在战斗中先判断目标路径点是否发生了变化，是则强行移动
        fightState.AddTransition(new Transition()
        {
            decisions = new List<Decision>{new HasTargetPosDecision()},
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


        //加入状态机
        AddState(idleState);
        AddState(moveState);
        AddState(fightState);
        AddState(moveIgnoreEnemyState);
        
        currentState = states[0];
    }
}