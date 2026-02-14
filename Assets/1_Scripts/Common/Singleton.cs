using UnityEngine;

public class Singleton<T> where T : Singleton<T>, new()
{
    public static T Instance => GetInstance();
    public static bool HasInstance => _instance != null;

    private static T _instance;
    private static readonly object _lock = new();

    private static T GetInstance()
    {
        lock (_lock)
        {
            if (!HasInstance)
            {
                _instance = new T();
                _instance.OnInit();
                Application.quitting -= DestroyInstance;
                Application.quitting += DestroyInstance;
            }
            return _instance;
        }
    }

    protected virtual void OnInit() { }

    private static void DestroyInstance()
    {
        if (!HasInstance) return;
        _instance.OnDestroy();
    }

    protected virtual void OnDestroy()
    {
        Application.quitting -= DestroyInstance;
        _instance = null;
    }
}