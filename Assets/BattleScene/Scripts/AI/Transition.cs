//状态转换。通过决定的返回值，选则两种状态中其中一个

using System.Collections.Generic;

public class Transition 
{
    public List<Decision> decisions;
    public State trueState;
    public State falseState;
}