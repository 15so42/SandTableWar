using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class State
{
    public List<StateAction> actions=new List<StateAction>();                        //动作
    public List<Transition> transitions=new List<Transition>();                //转换条件
    public StateController stateController;
    
    public State(StateController controller)
    {
        this.stateController = controller;
    }

    //每一帧更新状态，在StateController的OnUpdate中调用。
    public void UpdateState(StateController controller)
    {
        DoActions(controller);                      //执行动作
        CheckTransition(controller);                //检测转换状态
    }

    #region 设置Actions和Transitions

    //添加Action
    public void AddAction(StateAction stateAction)
    {
        actions.Add(stateAction);
    }

    public void RemoveAction(StateAction stateAction)
    {
        var toRemove = actions.FirstOrDefault(t => t == stateAction);
        actions.Remove(toRemove);
    }

    public void ClearActions()
    {
        actions.Clear();
    }

    //添加条件
    public void AddTransition(Transition transition)
    {
        transitions.Add(transition);
    }
    
    public void RemoveTransition(Transition transition)
    {
        var toRemove = transitions.FirstOrDefault(t => t == transition);
        transitions.Remove(toRemove);
    }

    public void ClearTransitions()
    {
        transitions.Clear();
    }

    #endregion
    
    //顺序执行动作列表的动作。
    private void DoActions(StateController controller)
    {
        foreach (var t in actions)
            t.Act(controller);
    }

    //检查所有转换状态，并改变状态。,满足一个就会切换
    private void CheckTransition(StateController controller)
    {
        foreach (var t in transitions)
        {
            //decisions判断
            bool decisionSucceeded = true;
            foreach (var d in t.decisions)
            {
                if (d.Decide(controller) == false)
                {
                    decisionSucceeded = false;
                    break;
                }
            }
            controller.TransitionToState(decisionSucceeded ? t.trueState : t.falseState);
        }
    }

}