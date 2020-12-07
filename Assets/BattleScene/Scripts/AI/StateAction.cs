using UnityEngine;

public abstract class StateAction 
{
    //State直接调用这个方法来执行动作。
    public abstract void Act(StateController controller);
}