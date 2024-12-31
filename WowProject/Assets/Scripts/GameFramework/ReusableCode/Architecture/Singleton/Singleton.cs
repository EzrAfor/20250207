using System.Reflection;
using System;

public class Singleton<T> where T:class ,ISingleton
{
    private static T mInstance;
    public static T Instance
    {
        get
        {
            if (mInstance == null)
            {
               var ctors = typeof(T).GetConstructors(BindingFlags.Instance|BindingFlags.NonPublic);
               var ctor =  Array.Find(ctors, c => c.GetParameters().Length == 0);
                if (ctor == null) {
                    throw new Exception("没有找到非公共构造函数"+typeof(T));
                }
                mInstance = ctor.Invoke(null) as T;
            }
            return mInstance;
        }
    }
}
