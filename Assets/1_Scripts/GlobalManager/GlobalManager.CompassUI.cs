using System.Collections.Generic;
using UnityEngine;

public partial class GlobalManager
{
    [Header("CompassUI")]
    [SerializeField] private NpcCompassUI[] _compassUIs;
    [SerializeField] private Canvas _compassCanvas;

    private RectTransform _canvasRectTransform;

    private void InitCompassUIs(IList<NpcInfo> npcInfos)
    {
        _npcInfos.Clear();
        _npcInfos.AddRange(npcInfos);
        if (_canvasRectTransform == null)
        {
            _canvasRectTransform = _compassCanvas.transform as RectTransform;
        }
        foreach (var compassUI in _compassUIs)
        {
            compassUI.Init();
        }
        if (_npcInfos.IsNullOrEmpty()) return;
        for (var i = 0; i < _npcInfos.Count; i++)
        {
            if (_compassUIs.Length <= i)
            {
                LogManager.LogError("Compass ui 가 npc 수보다 적습니다.");
                break;
            }
            _compassUIs[i].Show(_npcInfos[i]);
        }
    }

    private void UpdateCompassUIs()
    {
        foreach (var compassUI in _compassUIs)
        {
            if (compassUI.NpcInfo == null) continue;
            var screenPosition = _globalCamara.WorldToScreenPoint(compassUI.NpcInfo.CompassUITransform.position);
            screenPosition.x -= _canvasRectTransform.rect.width * 0.5f;
            compassUI.UpdatePosition(screenPosition);
        }
    }
}