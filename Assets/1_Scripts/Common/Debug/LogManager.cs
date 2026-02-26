using System.Diagnostics;
using System.Text;
using UnityEngine;
using Debug = UnityEngine.Debug;

public static class LogManager
{
    [Conditional("UNITY_EDITOR")]
    public static void Log(string message)
    {
        Debug.Log(message);
    }

    [Conditional("UNITY_EDITOR")]
    public static void LogColor(string message, Color color)
    {
        message = $"<color=#{ColorUtility.ToHtmlStringRGBA(color)}>{message}</color>";
        Debug.Log(message);
    }

    [Conditional("UNITY_EDITOR")]
    public static void LogWarning(string message)
    {
        Debug.LogWarning(message);
    }

    [Conditional("UNITY_EDITOR")]
    public static void Assert(bool condition, string message = null)
    {
        Debug.Assert(condition, message);
    }

    [Conditional("UNITY_EDITOR")]
    public static void LogError(string message)
    {
        Debug.LogError(message);
    }

    [Conditional("UNITY_EDITOR")]
    public static void LogErrorPack(string format, params object[] args)
    {
        var sb = new StringBuilder();
        sb.Append(format);
        foreach (var arg in args)
        {
            sb.Append($" {arg.GetType()}={arg}");
        }
        Debug.LogError(sb.ToString());
    }

    [Conditional("UNITY_EDITOR")]
    public static void LogException(System.Exception exception)
    {
        Debug.LogException(exception);
    }
}