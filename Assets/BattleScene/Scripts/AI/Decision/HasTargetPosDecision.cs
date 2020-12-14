using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HasTargetPosDecision : Decision
{
    public override bool Decide(StateController controller)
    {
        bool result=controller.TargetPosChanged();
        Debug.Log($"[{nameof(HasTargetPosDecision)}]是否有新的目标点：{result}");
        return result;
    }
}
