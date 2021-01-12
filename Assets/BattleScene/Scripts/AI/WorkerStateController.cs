using System.Collections.Generic;

public class WorkerStateController : StateController
{
    public WorkerStateController(BattleUnitBase battleUnitBase) : base(battleUnitBase)
    {
    }

    protected override void InitState()
    {
        //状态机设置
        State idleState = new BaseIdleState(this,"闲置");
        State moveState = new BaseMoveState(this,"移动");
        State moveIgnoreEnemyState =new BaseForciblyMoveState(this,"强行移动");
        State escapeState = new BaseEscapeState(this,"逃跑");

        //idle时发现有新的目标位置切换到移动状态
        idleState.AddTransition(new Transition()
        {
            decisions =  new List<Decision>{new HasTargetPosDecision()}, 
            trueState = moveState,
            falseState = idleState
        });
        
        //idle时发现敌人切换到逃跑状态
        idleState.AddTransition(new Transition()
        {
            decisions =  new List<Decision>{new FindEnemyDecision()},
            trueState = escapeState, 
            falseState = idleState
        });

        //移动状态执行移动函数
        moveState.AddAction(new MoveToPosStateAction());
        moveState.AddTransition(new Transition()
        {
            decisions =  new List<Decision>{new FindEnemyDecision()},
            falseState = moveState,
            trueState = escapeState
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

      
        escapeState.AddAction(new BaseEscapeAction());
        //转换条件按添加顺序执行，满足一个转换条件后就转换，因此此处是在战斗中先判断目标路径点是否发生了变化，是则强行移动
        escapeState.AddTransition(new Transition()
        {
            decisions = new List<Decision>{new HasTargetPosDecision()},
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
        AddState(idleState);
        AddState(moveState);
        AddState(escapeState);
        AddState(moveIgnoreEnemyState);
        
        currentState = states[0];
    }
}