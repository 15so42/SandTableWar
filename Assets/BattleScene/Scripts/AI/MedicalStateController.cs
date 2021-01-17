using System.Collections.Generic;

public class MedicalStateController : StateController
{
    public MedicalStateController(BattleUnitBase battleUnitBase) : base(battleUnitBase)
    {
    }

    protected State cureState;
    public BattleUnitBase cureTarget;
    protected override void InitState()
    {
        cureState = new BaseCureState(this, "治疗");
        
        base.InitState();
        
        cureState.AddAction(new BaseLookForWoundedAction());
        cureState.AddTransition(new Transition()
        {
            decisions = new List<Decision>{new HasEnoughCurePointDecision()},
            falseState = idleState,
            trueState = cureState
        });
        cureState.OnStateEnterEvent.AddListener(() =>
        {
            navMeshAgent.isStopped = false;
        });
        
        idleState.AddTransition(new Transition()
        {
            decisions = new List<Decision>{new HasEnoughCurePointDecision()},
            falseState = idleState,
            trueState = cureState
        });
        
        fightState.AddTransition(new Transition()
        {
            decisions = new List<Decision>{new HasEnoughCurePointDecision()},
            falseState = fightState,
            trueState = cureState
        });
        states.Add(cureState);
    }

    public void SetCureTarget(BattleUnitBase unit)
    {
        cureTarget = unit;
    }
    //医疗包到达位置后的回调
    public void CureTargetUnit()
    {
        cureTarget.CureHp(10);
       (owner as MedicalSolider).curePoint = 0;
        SetCureTarget(null);
    }
}