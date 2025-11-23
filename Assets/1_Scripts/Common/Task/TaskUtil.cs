using Cysharp.Threading.Tasks;

public static class TaskUtil
{
    public static bool SetResult(ref UniTaskCompletionSource taskCompletionSource)
    {
        if (taskCompletionSource == null) return false;

        var task = taskCompletionSource;
        taskCompletionSource = null;
        return task.TrySetResult();
    }

    public static bool SetResult<TResult>(ref UniTaskCompletionSource<TResult> taskCompletionSource, TResult result)
    {
        if (taskCompletionSource == null) return false;

        var task = taskCompletionSource;
        taskCompletionSource = null;
        return task.TrySetResult(result);
    }
}