namespace BattleScene.Scripts
{
    public interface IRecycleAble
    {
        void ReUse();//重用,从对象池中取出，并且要求初始化
        void Recycle();//回收
    }
}