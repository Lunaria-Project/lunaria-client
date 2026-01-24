using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Type = PanelManager.Type;

public class PanelInfo
{
    public Type Type;
    public Type PreviousType;
    public Type NextType;
    public PanelManager.DirectionType ShowDirectionType;
    public PanelManager.DirectionType HideDirectionType;
    public PanelBase Panel;
    public object[] Args;
    public object[] CachingData;
    private Action _onHideAction;

    public PanelInfo SetOnHideAction(Action action)
    {
        _onHideAction = action;
        return this;
    }

    public void InvokeOnHideAction()
    {
        _onHideAction?.Invoke();
    }
}

public partial class PanelManager : SingletonMonoBehaviour<PanelManager>
{
    [SerializeField] private RectTransform _parentRectTransform;

    private readonly Dictionary<Type, Func<Action, UniTask>> _panelPreShowAction = new();

    private readonly object[] _emptyArgs = Array.Empty<object>();
    private readonly List<PanelInfo> _panelInfoList = new();
    private PanelInfo ActivePanelInfo => _panelInfoList.IsNullOrEmpty() ? null : _panelInfoList.LastOrDefault();

    private PanelInfo CreatePanelInfo(Type type, params object[] args)
    {
        args ??= _emptyArgs;
        var panelInfo = new PanelInfo
        {
            Type = type,
            PreviousType = Type.None,
            NextType = Type.None,
            ShowDirectionType = DirectionType.None,
            HideDirectionType = DirectionType.None,
            Panel = null,
            Args = args,
        };
        return panelInfo;
    }

    private PanelInfo PeekPanelInfo()
    {
        return _panelInfoList.IsNullOrEmpty() ? null : _panelInfoList.Last();
    }

    // count 만큼 없앤 후 패널을 보여줄 것이므로, 그 때 보여줄 Panel Info를 Peek
    private PanelInfo PeekPanelInfo(int count)
    {
        if (count < 0 || count >= _panelInfoList.Count)
        {
            Debug.LogError($"Invalid count : {count}, panel info list count : {_panelInfoList.Count}");
            count = Mathf.Clamp(count, 0, _panelInfoList.Count);
        }

        return _panelInfoList.IsNullOrEmpty() ? null : _panelInfoList[^(count + 1)];
    }

    private (PanelInfo previousPopPanelInfo, PanelInfo popPanelInfo) PopPanelInfo(int count)
    {
        if (count <= 0) return (null, null);

        if (_panelInfoList.Count < count)
        {
            Debug.LogError("PanelInfoList count is only one");
            count = _panelInfoList.Count;
        }

        if (count <= 1)
        {
            count = 1;
            var popPanelInfo = _panelInfoList.RemoveLast(count);
            return (null, popPanelInfo);
        }
        else
        {
            var previousPopPanelInfo = _panelInfoList.RemoveLast(count - 1);
            var popPanelInfo = _panelInfoList.RemoveLast(1);
            return (previousPopPanelInfo, popPanelInfo);
        }
    }

    private PanelInfo PushPanelInfo(PanelInfo panelInfo)
    {
        _panelInfoList.Add(panelInfo);
        return panelInfo;
    }

    private Type PeekPanelInfoType()
    {
        return PeekPanelInfo()?.Type ?? Type.None;
    }

    private Type PeekPanelInfoType(int count)
    {
        return PeekPanelInfo(count)?.Type ?? Type.None;
    }

    private void ReleasePanel(PanelInfo panelInfo)
    {
        var panel = panelInfo.Panel;
        if (panel == null) return;

        DestroyPanel(panel);
    }

    private bool IsRootPanel()
    {
        return _panelInfoList.Count <= 1;
    }

    private bool IsActivePanel(Type type)
    {
        if (ActivePanelInfo == null) return false;
        return ActivePanelInfo.Type == type;
    }

    private void SetPanelInPanelInfo(PanelInfo panelInfo)
    {
        if (panelInfo.Panel != null) return;

        panelInfo.Panel = GetPanel(panelInfo.Type);
    }

    private PanelBase GetPanel(Type type)
    {
        var panelPrefab = GetPanelPrefab(type);
        if (panelPrefab != null) return InstantiatePanel(panelPrefab);

        Debug.LogError($"Not found panel prefab : {type}");
        return null;
    }

    private PanelBase GetPanelPrefab(Type panelType)
    {
        if (!_panelResourceKey.TryGetValue(panelType, out var panelResourceKey)) return null;
        return ResourceManager.Instance.LoadPrefab<PanelBase>(panelResourceKey);
    }

    private PanelBase InstantiatePanel(PanelBase panelPrefab)
    {
        var panel = Instantiate(panelPrefab, _parentRectTransform);
        return panel;
    }

    private static void DestroyPanel(PanelBase panel)
    {
        Destroy(panel.gameObject);
    }
}