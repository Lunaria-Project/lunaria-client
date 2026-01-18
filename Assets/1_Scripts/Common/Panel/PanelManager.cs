using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using UnityEngine;

public partial class PanelManager
{
    public event Action<Type> OnShowAction;
    public event Action<Type> OnHideAction;
    public PanelInfo CurrentPanelInfo;
    
    private UniTaskCompletionSource<bool> _taskCompletionSource;
    private (bool isCompleted, bool isSuccess) _isHideAndShowCompletedResult;

    [CanBeNull]
    public PanelInfo ShowPanel(Type showType, params object[] args)
    {
        if (IsActivePanel(showType)) return null;
        if (!_panelPreShowAction.TryGetValue(showType, out var preShow)) return ShowPanelInternal(showType, args);

        preShow.Invoke(() => { ShowPanelInternal(showType, args); });
        return null;
    }

    [CanBeNull]
    public PanelInfo HidePanel(int count = 1)
    {
        if (CheckHidePanel(count, out var panelInfo))
        {
            return panelInfo;
        }

        return HidePanelInternal(count);
    }

    // 모든 Show Panel 종류의 최종 처리 함수. 이 함수에 들어오기 전까지 PanelList에 Add, Remove 하면 안된다.
    [CanBeNull]
    private PanelInfo ShowPanelInternal(Type showType, object[] args)
    {
        var showPanelInfo = CreatePanelInfo(showType, args);
        NewHideAndShowPanel(DirectionType.Forward, 0, new List<PanelInfo> { showPanelInfo }).Forget();
        return showPanelInfo;
    }

    // 모든 Hide Panel 종류의 최종 처리 함수. 이 함수에 들어오기 전까지 PanelList에 Add, Remove 하면 안된다.
    private PanelInfo HidePanelInternal(int count)
    {
        // 최종적으로 NewHideAndShowPanel의 return 값을 사용하는 형태로 변경이 필요함
        var showPanelInfo = PeekPanelInfo(count);
        NewHideAndShowPanel(DirectionType.Backward, count, null).Forget();
        return showPanelInfo;
    }

    // 실제로 Panel을 Hide, Show 하는 함수
    private async UniTask NewHideAndShowPanel(DirectionType showDirectionType, int popPanelCount, List<PanelInfo> showPanelInfoList)
    {
        _isHideAndShowCompletedResult = (false, false);

        switch (showDirectionType)
        {
            // Forward 케이스
            // n(n >= 0)개의 패널을 Remove하고, m개(m > 1)의 패널을 Add한다.
            case DirectionType.Forward:
            {
                if (showPanelInfoList == null || showPanelInfoList.Count == 0)
                {
                    Debug.LogError("Show panel infos is empty");
                    break;
                }

                // show 할 패널의 정보를 가져와야함
                var showPanelInfo = showPanelInfoList.RemoveLast();
                var hidePanelInfo = PeekPanelInfo();

                // Hide
                if (hidePanelInfo != null)
                {
                    if (!await HidePanelProcess(showDirectionType, hidePanelInfo, CurrentPanelInfo.Type)) return;

                    PopPanelInfo(1);
                    popPanelCount--;
                }

                // Pop 할게 있다면 Pop 함
                PopPanelInfoProcess(popPanelCount);
                // Show 해야 할게 여러개 있다면 미리 Push 함
                ShowPanelInfoProcess(showPanelInfoList);

                var previousPanelType = PeekPanelInfoType();
                await ShowPanelProcess(showDirectionType, showPanelInfo, previousPanelType);

                break;
            }
            // Backward 케이스
            // n(n > 0)개의 패널을 Remove하고, 1개의 패널을 Add한다.
            case DirectionType.Backward:
            {
                // pop후 show 할때 패널 type을 가져옴
                var showPanelInfoType = PeekPanelInfoType(popPanelCount);
                var hidePanelInfo = PeekPanelInfo();

                // Hide
                if (hidePanelInfo != null)
                {
                    if (!await HidePanelProcess(showDirectionType, hidePanelInfo, showPanelInfoType)) return;
                    PopPanelInfo(1);
                }

                // pop하고 show할 패널을 가져옴
                var (previousPopPanelInfo, showPanelInfo) = PopPanelInfoProcess(popPanelCount);
                previousPopPanelInfo ??= hidePanelInfo;
                var previousPanelType = previousPopPanelInfo?.Type ?? Type.None;

                await ShowPanelProcess(showDirectionType, showPanelInfo, previousPanelType);
                break;
            }
        }

        // Invoke Callback
        _isHideAndShowCompletedResult = (true, true);
        TaskUtil.SetResult(ref _taskCompletionSource, true);
        ReleaseShowPanelInfos();
        return;

        async UniTask<bool> HidePanelProcess(DirectionType hidePanelDirectionType, PanelInfo hidePanelInfo, Type nextType)
        {
            hidePanelInfo.NextType = nextType;
            hidePanelInfo.HideDirectionType = hidePanelDirectionType;

            if (!await Hide(hidePanelInfo))
            {
                _isHideAndShowCompletedResult = (true, false);
                TaskUtil.SetResult(ref _taskCompletionSource, false);
                ReleaseShowPanelInfos();
                return false;
            }

            return true;
        }

        (PanelInfo previousPopPanelInfo, PanelInfo popPanelInfo) PopPanelInfoProcess(int count)
        {
            return count <= 0 ? (null, null) : PopPanelInfo(count);
        }

        void ShowPanelInfoProcess(IList<PanelInfo> panelInfoList)
        {
            if (panelInfoList.IsNullOrEmpty()) return;

            var previousPanelType = PeekPanelInfoType();

            foreach (var info in panelInfoList)
            {
                var panelInfo = PushPanelInfo(info);

                panelInfo.PreviousType = previousPanelType;
                panelInfo.ShowDirectionType = DirectionType.Forward;

                previousPanelType = panelInfo.Type;
            }
        }

        async UniTask ShowPanelProcess(DirectionType showPanelDirectionType, PanelInfo showPanelInfo, Type previousPanelType)
        {
            CurrentPanelInfo = showPanelInfo;
            showPanelInfo.PreviousType = previousPanelType;
            showPanelInfo.ShowDirectionType = showPanelDirectionType;

            PushPanelInfo(showPanelInfo);

            await Show(showPanelInfo);
        }

        void ReleaseShowPanelInfos()
        {
            if (showPanelInfoList == null) return;

            showPanelInfoList.Clear();
        }
    }

    private bool CheckHidePanel(int count, out PanelInfo panelInfo)
    {
        if (count >= _panelInfoList.Count)
        {
            panelInfo = HidePanel(_panelInfoList.Count - 1);
            return true;
        }

        if (count <= 0)
        {
            Debug.Log("Hide panel count is zero");
        }

        if (IsRootPanel())
        {
            Debug.Log("Current panel is root panel");
        }

        panelInfo = null;
        return false;
    }

    private async UniTask Show(PanelInfo panelInfo)
    {
        if (panelInfo == null)
        {
            Debug.LogError("PanelInfo is null");
            return;
        }

        SetPanelInPanelInfo(panelInfo);

        var panel = panelInfo.Panel;
        var panelType = panelInfo.Type;

        panel.gameObject.SetActive(true);
        await panel.ShowByManager(this, panelInfo, panelInfo.Args);
        OnShowAction?.Invoke(panelType);
    }

    private async UniTask<bool> Hide(PanelInfo panelInfo)
    {
        if (panelInfo == null)
        {
            Debug.LogError("PanelInfo is null");
            return true;
        }

        var panel = panelInfo.Panel;
        var panelType = panelInfo.Type;

        if (!await panel.HideByManager())
        {
            return false;
        }

        panel.gameObject.SetActive(false);
        OnHideAction?.Invoke(panelType);
        panelInfo.InvokeOnHideAction();

        ReleasePanel(panelInfo);

        return true;
    }
}