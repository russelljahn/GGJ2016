#if UNITY_EDITOR || DEBUG
    #define ENABLED_LOGGER
#endif

using System;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;


namespace Sense.Utils
{

    /// <summary>
    /// Wrapper around Unity's Debug class that automatically omits debugging logs, lines, and rays in builds unless the DEBUG symbol is defined.
    /// </summary>
    public static class Logger
    {

        private const string EnabledLogger = "ENABLED_LOGGER";


        [Conditional(EnabledLogger)]
        public static void Break()
        {
            Debug.Break();
        }


        [Conditional(EnabledLogger)]
        public static void ClearDeveloperConsole()
        {
            Debug.ClearDeveloperConsole();
        }


        [Conditional(EnabledLogger)]
        public static void DebugBreak()
        {
            Debug.DebugBreak();
        }


        [Conditional(EnabledLogger)]
        public static void DrawLine(Vector3 start, Vector3 end)
        {
            Debug.DrawLine(start, end);
        }


        [Conditional(EnabledLogger)]
        public static void DrawLine(Vector3 start, Vector3 end, Color color)
        {
            Debug.DrawLine(start, end, color);
        }


        [Conditional(EnabledLogger)]
        public static void DrawLine(Vector3 start, Vector3 end, Color color, float duration)
        {
            Debug.DrawLine(start, end, color, duration);
        }


        [Conditional(EnabledLogger)]
        public static void DrawLine(Vector3 start, Vector3 end, Color color, float duration, bool depthTest)
        {
            Debug.DrawLine(start, end, color, duration, depthTest);
        }


        [Conditional(EnabledLogger)]
        public static void DrawRay(Vector3 start, Vector3 direction)
        {
            Debug.DrawRay(start, direction);
        }


        [Conditional(EnabledLogger)]
        public static void DrawRay(Vector3 start, Vector3 direction, Color color)
        {
            Debug.DrawRay(start, direction, color);
        }


        [Conditional(EnabledLogger)]
        public static void DrawRay(Vector3 start, Vector3 direction, Color color, float duration)
        {
            Debug.DrawRay(start, direction, color, duration);
        }


        [Conditional(EnabledLogger)]
        public static void DrawRay(Vector3 start, Vector3 direction, Color color, float duration, bool depthTest)
        {
            Debug.DrawRay(start, direction, color, duration, depthTest);
        }


        [Conditional(EnabledLogger)]
        public static void Log(object message)
        {
            Debug.Log(message);
        }


        [Conditional(EnabledLogger)]
        public static void Log(object message, Object context)
        {
            Debug.Log(message, context);
        }


        [Conditional(EnabledLogger)]
        public static void Log(object message, Color color)
        {
            var r = (int) (color.r*255);
            var g = (int) (color.g*255);
            var b = (int) (color.b*255);
            var a = (int) (color.a*255);
            Debug.Log(string.Format("<color=#{0:X2}{1:X2}{2:X2}{3:X2}>{4}</color>", r, g, b, a, message));
        }


        [Conditional(EnabledLogger)]
        public static void LogError(object message)
        {
            Debug.LogError(message);
        }


        [Conditional(EnabledLogger)]
        public static void LogError(object message, Object context)
        {
            Debug.LogError(message, context);
        }


        [Conditional(EnabledLogger)]
        public static void LogErrorFormat(string format, params object[] args)
        {
            Debug.LogErrorFormat(format, args);
        }


        [Conditional(EnabledLogger)]
        public static void LogErrorFormat(Object context, string format, params object[] args)
        {
            Debug.LogErrorFormat(context, format, args);
        }


        [Conditional(EnabledLogger)]
        public static void LogException(Exception exception)
        {
            Debug.LogException(exception);
        }


        [Conditional(EnabledLogger)]
        public static void LogException(Exception exception, Object context)
        {
            Debug.LogException(exception, context);
        }


        [Conditional(EnabledLogger)]
        public static void LogFormat(string format, params object[] args)
        {
            Debug.LogFormat(format, args);
        }


        [Conditional(EnabledLogger)]
        public static void LogFormat(Object context, string format, params object[] args)
        {
            Debug.LogFormat(context, format, args);
        }


        [Conditional(EnabledLogger)]
        public static void LogWarning(object message)
        {
            Debug.LogWarning(message);
        }


        [Conditional(EnabledLogger)]
        public static void LogWarning(object message, Object context)
        {
            Debug.LogWarning(message, context);
        }


        [Conditional(EnabledLogger)]
        public static void LogWarningFormat(string format, params object[] args)
        {
            Debug.LogWarningFormat(format, args);
        }


        [Conditional(EnabledLogger)]
        public static void LogWarningFormat(Object context, string format, params object[] args)
        {
            Debug.LogWarningFormat(context, format, args);
        }
    }
}
