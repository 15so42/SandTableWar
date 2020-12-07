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
    public State remainState; //保持当前状态

    public List<State> states = new List<State>();
    public NavMeshAgent navMeshAgent; //导航组件
    public List<Transform> wayPointList; //所有巡逻点
    public BattleUnitBase enemy; //追踪目标
    public float stateTimeElapsed; //状态变化时间间隔
    public BattleUnitBase owner;
    public Vector3 targetPos;
    private Vector3 lastTargetPos;

    public StateController(BattleUnitBase battleUnitBase)
    {
        owner = battleUnitBase;
        Init();
    }

    private void Init()
    {
        navMeshAgent = owner.NavMeshAgent;
        lastTargetPos = targetPos;
        //状态机设置
        State idleState = new BaseIdleState(this);
        State moveState = new BaseMoveState(this);
        State fightState = new BaseFightState(this);

        //idle时发现有新的目标位置切换到移动状态
        idleState.AddTransition(new Transition()
        {
            decision = new HasTargetPosDecision(), trueState = moveState,
            falseState = currentState
        });
        
        //idle时发现敌人切换到战斗状态
        idleState.AddTransition(new Transition()
        {
            decision = new FindEnemyDecision(), trueState = fightState, 
            falseState = currentState
        });

        //移动状态执行移动函数
        moveState.AddAction(new MoveToPosStateAction());
        moveState.AddTransition(new Transition()
        {
            decision = new ReachTargetPosDecision(),
            falseState = currentState,
            trueState = moveState
        });
        moveState.AddTransition(new Transition()
        {
            decision = new FindEnemyDecision(),
            falseState = currentState,
            trueState = fightState
        });

        //战斗状态检查是否还有敌人，没有敌人后检查是否有目标导航路径，有则切回移动，没有则回到待机状态
        fightState.AddAction(new BaseFightAction());
        fightState.AddTransition(new Transition()
        {
            decision = new FindEnemyDecision(),
            falseState = idleState,
            trueState = moveState
        });


        //加入状态机
        AddState(idleState);
        AddState(moveState);
        AddState(fightState);

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
        if (nextState != remainState)
        {
            currentState = nextState;
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
        return states.Find(x => nameof(x.GetType) == name);
    }

    public void SetEnemy(BattleUnitBase battleUnitBase)
    {
        enemy = battleUnitBase;
    }
}