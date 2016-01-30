using System;

namespace Assets.OutOfTheBox.Scripts.Extensions
{
    public static class ActionExtensions
    {
        public static void SafelyInvoke(this Action action)
        {
            var handler = action;
            if (handler != null)
            {
                action.Invoke();
            }
        }

        public static void SafelyInvoke<T>(this Action<T> action, T arg)
        {
            var handler = action;
            if (handler != null)
            {
                action.Invoke(arg);
            }
        }

        public static void SafelyInvoke<T1, T2>(this Action<T1, T2> action, T1 arg1, T2 arg2)
        {
            var handler = action;
            if (handler != null)
            {
                action.Invoke(arg1, arg2);
            }
        }

        public static void SafelyInvoke<T1, T2, T3>(this Action<T1, T2, T3> action, T1 arg1, T2 arg2, T3 arg3)
        {
            var handler = action;
            if (handler != null)
            {
                action.Invoke(arg1, arg2, arg3);
            }
        }

        public static void SafelyInvoke<T1, T2, T3, T4>(this Action<T1, T2, T3, T4> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            var handler = action;
            if (handler != null)
            {
                action.Invoke(arg1, arg2, arg3, arg4);
            }
        }
    }
}
