using System;


public class Singleton<T> where T : Singleton<T>
{
    private static T instance;

    public static T Instance
    {
        get
        {
            if (null == instance)
            {
                instance = Activator.CreateInstance<T>();
            }

            return instance;
        }
    }
}