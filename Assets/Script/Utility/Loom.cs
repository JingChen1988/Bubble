using UnityEngine;
using System;
using System.Linq;
using System.Threading;
using System.Collections.Generic;
/// <summary>
/// 多线程工具类
/// </summary>
public class Loom : MonoBehaviour
{
    public static int maxThreads = 8;
    static int numThreads;
    static bool initialized;
    private static Loom _current;
    private int _count;
    private List<Action> _actions = new List<Action>();
    List<Action> _currentActions = new List<Action>();
    private List<DelayedQueueItem> _delayed = new List<DelayedQueueItem>();
    List<DelayedQueueItem> _currentDelayed = new List<DelayedQueueItem>();

    public struct DelayedQueueItem
    {
        public float time;
        public Action action;
    }

    public static Loom Current
    {
        get
        {
            Initialize();
            return _current;
        }
    }

    void Awake()
    {
        _current = this;
        initialized = true;
    }

    static void Initialize()
    {
        if (!initialized)
        {
            if (!Application.isPlaying)
                return;
            initialized = true;
            var g = new GameObject("Loom");
            _current = g.AddComponent<Loom>();
        }
    }

    void OnDisable()
    {
        if (_current == this) _current = null;
    }

    void Update()
    {
        lock (_actions)
        {
            _currentActions.Clear();
            _currentActions.AddRange(_actions);
            _actions.Clear();
        }
        foreach (var action in _currentActions)
        {
            action();
        }
        lock (_delayed)
        {
            _currentDelayed.Clear();
            _currentDelayed.AddRange(_delayed.Where(d => d.time <= Time.time));
            foreach (var item in _currentDelayed)
                _delayed.Remove(item);
        }
        foreach (var delayed in _currentDelayed)
        {
            delayed.action();
        }
    }

    /// <summary>
    /// 执行Unity主线程
    /// </summary>
    public static void QueueOnMainThread(Action action, float time = 0f)
    {
        if (time != 0)
        {
            lock (Current._delayed)
            {
                Current._delayed.Add(new DelayedQueueItem { time = Time.time + time, action = action });
            }
        }
        else
        {
            lock (Current._actions)
            {
                Current._actions.Add(action);
            }
        }
    }

    /// <summary>
    /// 执行异步线程
    /// </summary>
    public static Thread RunAsync(Action action)
    {
        Initialize();
        while (numThreads >= maxThreads)
        {
            Thread.Sleep(1);
        }
        Interlocked.Increment(ref numThreads);
        ThreadPool.QueueUserWorkItem(RunAction, action);
        return null;
    }

    private static void RunAction(object action)
    {
        try
        {
            ((Action)action)();
        }
        catch
        {
        }
        finally
        {
            Interlocked.Decrement(ref numThreads);
        }
    }

    static void Test(Mesh mesh, float scale)
    {
        var vertices = mesh.vertices;
        Loom.RunAsync(() =>
        {
            for (var i = 0; i < vertices.Length; i++)
                vertices[i] *= scale;

            Loom.QueueOnMainThread(() =>
            {
                mesh.vertices = vertices;
                mesh.RecalculateBounds();
            });
        });
    }
}