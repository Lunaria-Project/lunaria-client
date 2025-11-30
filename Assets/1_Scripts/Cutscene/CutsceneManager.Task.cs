using Cysharp.Threading.Tasks;
using UnityEngine;

public partial class CutsceneManager
{
    [SerializeField] private GameObject _emptyButton;

    public bool IsWaiting => _waitClickTask != null;
    private UniTaskCompletionSource _waitClickTask;

    public UniTask GetWaitClickTask()
    {
        if (_waitClickTask != null)
        {
            LogManager.LogError("이미 기다리는 _waitClickTask가 있습니다.");
            TaskUtil.SetResult(ref _waitClickTask);
        }

        _emptyButton.SetActive(true);
        _waitClickTask = new UniTaskCompletionSource();
        return _waitClickTask.Task;
    }

    public void OnWaitButtonClick()
    {
        _emptyButton.SetActive(false);
        TaskUtil.SetResult(ref _waitClickTask);
    }

    private void ClearTask()
    {
        _emptyButton.SetActive(false);
        if (_waitClickTask != null)
        {
            TaskUtil.SetResult(ref _waitClickTask);
        }
    }
}