using System;

namespace Assets.OutOfTheBox.Scripts.Extensions
{
    public static class FuncExtensions
    {
        public static TResult SafelyInvoke<TResult>(this Func<TResult> func)
        {
            var handler = func;
            if (handler != null)
            {
                func.Invoke();
            }
            return default(TResult);
        }

        public static TResult SafelyInvoke<T, TResult>(this Func<T, TResult> func, T arg)
        {
            var handler = func;
            if (handler != null)
            {
                func.Invoke(arg);
            }
            return default(TResult);
        }

        public static TResult SafelyInvoke<T1, T2, TResult>(this Func<T1, T2, TResult> func, T1 arg1, T2 arg2)
        {
            var handler = func;
            if (handler != null)
            {
                func.Invoke(arg1, arg2);
            }
            return default(TResult);
        }

        public static TResult SafelyInvoke<T1, T2, T3, TResult>(this Func<T1, T2, T3, TResult> func, T1 arg1, T2 arg2,
            T3 arg3)
        {
            var handler = func;
            if (handler != null)
            {
                func.Invoke(arg1, arg2, arg3);
            }
            return default(TResult);
        }

        public static TResult SafelyInvoke<T1, T2, T3, T4, TResult>(this Func<T1, T2, T3, T4, TResult> func, T1 arg1,
            T2 arg2, T3 arg3, T4 arg4)
        {
            var handler = func;
            if (handler != null)
            {
                func.Invoke(arg1, arg2, arg3, arg4);
            }
            return default(TResult);
        }
    }
}