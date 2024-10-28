using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class UnityMainThreadDispatcher : MonoBehaviour
{

    private static readonly Queue<Action> s_executionQueue = new();

    public void Update()
    {
        lock (s_executionQueue)
        {
            while (s_executionQueue.Count > 0)
            {
                s_executionQueue.Dequeue().Invoke();
            }
        }
    }
    public void Enqueue(IEnumerator action)
    {
        lock (s_executionQueue)
        {
            s_executionQueue.Enqueue(() =>
            {
                StartCoroutine(action);
            });
        }
    }
    public void Enqueue(Action action)
    {
        Enqueue(ActionWrapper(action));
    }
    public Task EnqueueAsync(Action action)
    {
        var tcs = new TaskCompletionSource<bool>();
        void WrappedAction()
        {
            try
            {
                action();
                tcs.TrySetResult(true);
            }
            catch (Exception ex)
            {
                tcs.TrySetException(ex);
            }
        }
        Enqueue(ActionWrapper(WrappedAction));
        return tcs.Task;
    }
    IEnumerator ActionWrapper(Action a)
    {
        a();
        yield return null;
    }
    private static UnityMainThreadDispatcher s_instance = null;

    public static bool Exists()
    {
        return s_instance != null;
    }
    public static UnityMainThreadDispatcher Instance()
    {
        if (!Exists())
        {
            throw new Exception("UnityMainThreadDispatcher could not find the UnityMainThreadDispatcher object. Please ensure you have added the MainThreadExecutor Prefab to your scene.");
        }
        return s_instance;
    }
    void Awake()
    {
        if (s_instance == null)
        {
            s_instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }
    void OnDestroy()
    {
        s_instance = null;
    }
}
