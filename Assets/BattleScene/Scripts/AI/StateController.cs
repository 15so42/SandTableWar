using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// 来自unity官方可插拔状态机教程，有大量的修改
/// https://blog.csdn.net/l773575310/article/details/73008669
///stateController是狀態機的基類，不同的AI應該繼承並重寫部分代碼
public class StateController
{
    public State currentState; //当前状态
    //public State remainState=null; //保持当前状态

    public List<State> states = new List<State>();
    public NavMeshAgent navMeshAgent; //导航组件
    public List<Transform> wayPointList; //所有巡逻点
    public BattleUnitBase enemy; //追踪目标
    public float stateTimeElapsed; //状态变化时间间隔
    public BattleUnitBase owner;
    public Vector3 targetPos;
    public Vector3 lastTargetPos;

    public StateController(BattleUnitBase battleUnitBase)
    {
        owner = battleUnitBase;
        Init();
    }

    private void Init()
    {
        navMeshAgent = owner.navMeshAgent;
        lastTargetPos = targetPos;

        InitState();
    }

    protected virtual void InitState()
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
            decisions = new List<Decision>{new ReachTargetPosDecision()},
            falseState = moveState,
            trueState = idleState
        });
        moveState.AddTransition(new Transition()
        {
            decisions =  new List<Decision>{new FindEnemyDecision()},
            falseState = moveState,
            trueState = fightState
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
        fightState.AddAction(new BaseFightAction());
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

        //todo 设置初始状态
        currentState = states[0];
    }


    public void Update()
    {
        currentState.UpdateState(this); //更新状态
    }


    //转换到下一个状态
    public void TransitionToState(State nextState)
    {
        if (nextState != null)
        {
            currentState = nextState;
            Debug.Log($"{owner.transform.name}切换状态至{nextState}");
            OnExitState();
        }
    }

    //返回是否过了时间间隔
    public bool CheckIfCountDownElapsed(float duration)
    {
        stateTimeElapsed += Time.deltaTime;
        return (stateTimeElapsed >= duration);
    }

    private void OnExitState()
    {
        stateTimeElapsed = 0;
    }


    public void SetTargetPos(Vector3 pos)
    {
        targetPos = pos;
    }

    public bool TargetPosChanged()
    {
        bool result = lastTargetPos != targetPos;
        lastTargetPos = targetPos;
        return result;
    }

    #region 设置States

    public void AddState(State state)
    {
        states.Add(state);
    }

    public void ClearState()
    {
        states.Clear();
    }

    public void SetCurrentState(State state)
    {
        currentState = state;
    }

    #endregion

    public State GetState(string name)
    {
        return states.Find(x => x.stateName == name);
    }

    public void SetEnemy(BattleUnitBase battleUnitBase)
    {
        enemy = battleUnitBase;
    }
    
    
}