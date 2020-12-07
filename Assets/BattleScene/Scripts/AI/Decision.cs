using UnityEngine;

public abstract class Decision
{
    //通过这个方法的返回值来判断决定的选择。
    public abstract bool Decide(StateController controller);
}