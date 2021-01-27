using System.Collections.Generic;

public class WorkerStateController : StateController
{
    public WorkerStateController(BattleUnitBase battleUnitBase) : base(battleUnitBase)
    {
    }

    public State mineState;
    protected override void InitState()
    {
        //状态机设置
        State escapeState = new BaseEscapeState(this,"逃跑");
        mineState = new BaseMineState(this, "采矿");

        base.InitState();
        idleState.transitions.Clear();
        //idle时发现敌人切换到逃跑状态
        idleState.AddTransition(new Transition()
        {
            decisions =  new List<Decision>{new HasNewTargetPosDecision()}, 
            trueState = moveState,
            falseState = idleState
        });
        idleState.AddTransition(new Transition()
        {
            decisions = new List<Decision>{new HasChaseTargetDecision()},
            falseState = idleState,
            trueState = chaseState
        });
        idleState.AddTransition(new Transition()
        {
            decisions =  new List<Decision>{new FindEnemyDecision()},
            trueState = escapeState, 
            falseState = idleState
        });

       
        //移动状态执行移动函数
        moveState.transitions.Clear();
        moveState.AddTransition(new Transition()//注意顺序，顺序代表优先级
        {
            decisions =  new List<Decision>{new IsInBuildingDecision()},
            falseState = moveState,
            trueState = inBuildingIdleState
        });
        moveState.AddTransition(new Transition()
        {
            decisions = new List<Decision>{new HasChaseTargetDecision()},
            falseState = moveState,
            trueState = chaseState
        });
        moveState.AddTransition(new Transition()
        {
            decisions = new List<Decision>{new ReachTargetPosDecision()},
            falseState = moveState,
            trueState = idleState
        });
        moveState.AddTransition(new Transition()
        {
            decisions =  new List<Decision>{new FindEnemyDecision()},
            falseState = moveState,
            trueState = escapeState
        });
        
        
        
        chaseState.transitions.Clear();
        chaseState.AddTransition(new Transition()
        {
            decisions = new List<Decision>{new HasNewTargetPosDecision()},
            falseState = chaseState,
            trueState = moveIgnoreEnemyState
        });
        chaseState.AddTransition(new Transition()
        {
            decisions = new List<Decision>{new InAttackRangeDecision()},
            falseState = chaseState,
            trueState = mineState
        });
        
        mineState.AddAction(new BaseMineAction());
        mineState.AddTransition(new Transition()
        {
            decisions = new List<Decision>{new FindEnemyDecision()},
            falseState = mineState,
            trueState = escapeState
        });
        mineState.AddTransition(new Transition()
        {
            decisions = new List<Decision>{new HasNewTargetPosDecision()},
            falseState = mineState,
            trueState = moveIgnoreEnemyState
        });

        escapeState.AddAction(new BaseEscapeAction());
        //转换条件按添加顺序执行，满足一个转换条件后就转换，因此此处是在战斗中先判断目标路径点是否发生了变化，是则强行移动
        escapeState.AddTransition(new Transition()
        {
            decisions = new List<Decision>{new HasNewTargetPosDecision()},
            falseState = escapeState,
            trueState = moveIgnoreEnemyState
        });
        //如果路径点没有发生变化，则判断敌人是否全被消灭,全被消灭后转换为MoveState朝之前的目标点移动
        escapeState.AddTransition(new Transition()
        {
            decisions = new List<Decision>{new FindEnemyDecision()},
            falseState = moveState,
            trueState = escapeState
        });


        //加入状态机
        AddState(escapeState);
        AddState(mineState);
       
        
        currentState = states[0];
    }
}