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
    public BattleUnitBase chaseTarget;

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
        currentState.OnStateEnter();
    }

    protected State idleState ;
    protected State moveState ;
    protected State moveIgnoreEnemyState ;
    protected State chaseState ;
    protected State fightState ;
    protected State inBuildingIdleState;
    protected State inBuildingFightState;
    protected virtual void InitState()
    {
        idleState = new BaseIdleState(this,"闲置");
        moveState = new BaseMoveState(this,"移动");
        moveIgnoreEnemyState =new BaseForciblyMoveState(this,"强行移动");
        chaseState = new BaseChaseState(this,"追赶");
        fightState = new BaseFightState(this,"战斗");
        inBuildingIdleState=new BaseInBuildingIdleState(this,"房间内待机");
        inBuildingFightState = new BaseInBuildingFightState(this, "房间内战斗");
        
        //idle时发现有新的目标位置切换到移动状态
        idleState.AddTransition(new Transition()
        {
            decisions =  new List<Decision>{new HasTargetPosDecision()}, 
            trueState = moveState,
            falseState = idleState
        });
        idleState.AddTransition(new Transition()
        {
            decisions = new List<Decision>{new HasChaseTargetDecision()},
            falseState = idleState,
            trueState = chaseState
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
        moveState.AddTransition(new Transition()//注意顺序，顺序代表优先级
        {
            decisions =  new List<Decision>{new FindEnemyDecision()},
            falseState = moveState,
            trueState = fightState
        });
        moveState.OnStateEnterEvent.AddListener(() =>
        {
            navMeshAgent.isStopped = false;
        });
        
       
        
        //强制移动到目标点，用于战斗中强行移动(撤退，突围等操作)
        moveIgnoreEnemyState.AddAction(new MoveToPosStateAction());
        moveIgnoreEnemyState.AddTransition(new Transition()
        {
            decisions =  new List<Decision>{new IsInBuildingDecision()},
            falseState = moveIgnoreEnemyState,
            trueState = inBuildingIdleState
        });
        moveIgnoreEnemyState.AddTransition(new Transition()
        {
            decisions =  new List<Decision>{new HasChaseTargetDecision()},
            falseState = moveIgnoreEnemyState,
            trueState = chaseState
        });
        moveIgnoreEnemyState.AddTransition(new Transition()
        {
            decisions =  new List<Decision>{new ReachTargetPosDecision()},
            falseState = moveIgnoreEnemyState,
            trueState = idleState
        });
        moveIgnoreEnemyState.OnStateEnterEvent.AddListener(() =>
        {
            navMeshAgent.isStopped = false;
            chaseState = null;
            owner.SetChaseTarget(null);
        });
        
        ///追踪
        chaseState.AddAction(new BaseChaseAction());
        chaseState.AddTransition(new Transition()
        {
            decisions = new List<Decision>{new HasTargetPosDecision()},
            falseState = chaseState,
            trueState = moveIgnoreEnemyState
        });
        chaseState.AddTransition(new Transition()
        {
            decisions = new List<Decision>{new InAttackRangeDecision()},
            falseState = chaseState,
            trueState = idleState
        });
        chaseState.OnStateEnterEvent.AddListener(() =>
        {
            navMeshAgent.isStopped = false;
        });
        chaseState.OnStateExitEvent.AddListener(() =>
        {
            SetChaseTarget(null);
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
            decisions = new List<Decision>{new EnemyAliveDecision()},
            falseState = idleState,
            trueState = fightState
        });
        fightState.AddTransition(new Transition()
        {
            decisions = new List<Decision>{new HasTargetPosDecision()},
            falseState = fightState,
            trueState = moveIgnoreEnemyState
        });
        fightState.AddTransition(new Transition()
        {
            decisions = new List<Decision>{new HasChaseTargetDecision()},
            falseState = fightState,
            trueState = chaseState
        });
        //如果路径点没有发生变化，且不用追踪敌人，则判断敌人是否全被消灭,全被消灭后转换为MoveState朝之前的目标点移动
        fightState.AddTransition(new Transition()
        {
            decisions = new List<Decision>{new FindEnemyDecision()},
            falseState = moveState,
            trueState = fightState
        });
        
        fightState.OnStateEnterEvent.AddListener(() =>
        {
            navMeshAgent.isStopped = true;
        });
        
        //房间内待机
        inBuildingIdleState.AddTransition(new Transition()
        {
            decisions = new List<Decision>{new IsInBuildingDecision()},
            falseState = idleState,
            trueState = inBuildingIdleState
        });
        inBuildingIdleState.AddTransition(new Transition()
        {
            decisions =  new List<Decision>{new FindEnemyDecision()},
            trueState = inBuildingFightState, 
            falseState = inBuildingIdleState
        });
        inBuildingIdleState.OnStateEnterEvent.AddListener(() =>
        {
            navMeshAgent.isStopped = true;
        });
        
        //防守状态
        inBuildingFightState.AddAction(new BaseFightAction());
        inBuildingFightState.AddTransition(new Transition()
        {
            decisions = new List<Decision>{new IsInBuildingDecision()},
            falseState = idleState,
            trueState = inBuildingFightState
        });
        inBuildingFightState.AddTransition(new Transition()
        {
            decisions = new List<Decision>{new FindEnemyDecision()},
            falseState = inBuildingIdleState,
            trueState = inBuildingFightState
        });
        inBuildingFightState.OnStateEnterEvent.AddListener(() =>
        {
            navMeshAgent.isStopped = true;
        });


        //加入状态机
        AddState(idleState);
        AddState(moveState);
        AddState(fightState);
        AddState(moveIgnoreEnemyState);
        AddState(chaseState);
        AddState(inBuildingIdleState);
        AddState(inBuildingFightState);

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
        if (nextState != null && currentState!=nextState)
        {
            currentState.OnStateExit();
            currentState = nextState;
            currentState.OnStateEnter();
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

    public void SetChaseTarget(BattleUnitBase target)
    {
        chaseTarget = target;
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