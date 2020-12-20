namespace GameCode.ObjectPool
{
    public interface IPool<T>
    {
        T Allocate();//分配

        bool Recycle(T obj);//回收
    }
}