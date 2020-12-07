using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HasTargetPosDecision : Decision
{
    public override bool Decide(StateController controller)
    {
        return controller.TargetPosChanged();
    }
}
