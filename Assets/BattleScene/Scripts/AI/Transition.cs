//状态转换。通过决定的返回值，选则两种状态中其中一个
public class Transition 
{
    public Decision decision;
    public State trueState;
    public State falseState;
}