using UnityEngine;

public partial class GlobalManager
{
    [Header("CompassUI")]
    [SerializeField] private NpcCompassUI[] _compassUIs;
    [SerializeField] private Canvas _compassCanvas;

    private RectTransform _canvasRectTransform;

    public void InitCompassUIs(NpcObject[] npcObjects)
    {
        if (_canvasRectTransform == null)
        {
            _canvasRectTransform = _compassCanvas.transform as RectTransform;
        }
        foreach (var compassUI in _compassUIs)
        {
            compassUI.Init();
        }
        if (npcObjects.IsNullOrEmpty()) return;
        for (var i = 0; i < npcObjects.Length; i++)
        {
            if (_compassUIs.Length <= i)
            {
                LogManager.LogError("Compass ui 가 npc 수보다 적습니다.");
                break;
            }
            _compassUIs[i].Show(npcObjects[i]);
        }
    }

    private void UpdateCompassUIs()
    {
        foreach (var compassUI in _compassUIs)
        {
            if (compassUI._npcObject == null) continue;
            var screenPosition = _globalCamara.WorldToScreenPoint(compassUI._npcObject.CompassUITransform.position);
            screenPosition.x -= _canvasRectTransform.rect.width * 0.5f;
            compassUI.UpdatePosition(screenPosition);
        }
    }
}