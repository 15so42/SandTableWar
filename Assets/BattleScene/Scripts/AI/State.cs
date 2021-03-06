﻿using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;


public class State
{
    public List<StateAction> actions=new List<StateAction>();                        //动作
    public List<Transition> transitions=new List<Transition>();                //转换条件
    public StateController ownerController;

    public UnityEvent OnStateEnterEvent=new UnityEvent();
    public UnityEvent OnStateExitEvent=new UnityEvent();
    public string stateName;
    public State(StateController controller,string stateName)
    {
        this.ownerController = controller;
        this.stateName = stateName;
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

    public virtual void OnStateEnter()
    {
        OnStateEnterEvent?.Invoke();
    }

    public virtual void OnStateExit()
    {
        OnStateExitEvent?.Invoke();
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

            if ((t.falseState == controller.currentState && decisionSucceeded == false) ||
                (t.trueState == controller.currentState && decisionSucceeded))//无效转化
            {
                continue;
            }
            controller.TransitionToState(decisionSucceeded ? t.trueState : t.falseState);
            //走过一个成功转换后则停止
            return;
        }
    }

}