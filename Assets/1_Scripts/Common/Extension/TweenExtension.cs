using Cysharp.Threading.Tasks;
using DG.Tweening;

public static class TweenExtension
{
    public static UniTask<T> GetTask<T>(this T @this, bool suppressCancellation = true) where T : Tween
    {
        var source = new UniTaskCompletionSource<T>();
        @this.AppendOnComplete(OnComplete);
        @this.AppendOnKill(OnKill);
        if (suppressCancellation)
        {
            source.Task.SuppressCancellationThrow();
        }
        return source.Task;

        void OnComplete()
        {
            Cleanup();
            source.TrySetResult(@this);
        }

        void OnKill()
        {
            Cleanup();
            source.TrySetCanceled();
        }

        void Cleanup()
        {
            if (@this == null) return;

            @this.onComplete -= OnComplete;
            @this.onKill -= OnKill;
        }
    }

    public static T AppendOnComplete<T>(this T tween, TweenCallback tweenCallback) where T : Tween
    {
        if (tween == null || !tween.IsActive()) return tween;
        tween.onComplete += tweenCallback;
        return tween;
    }

    public static T AppendOnKill<T>(this T tween, TweenCallback tweenCallback) where T : Tween
    {
        if (tween == null || !tween.IsActive()) return tween;
        tween.onKill += tweenCallback;
        return tween;
    }
}