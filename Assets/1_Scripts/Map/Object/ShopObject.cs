using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

public class ShopObject : MapObject
{
#if UNITY_EDITOR
    [ValueDropdown("@DataIdDropDownList.GetShopDataIds()")]
#endif
    [SerializeField] private int _shopDataId;
    [SerializeField] private GameObject _openedObject;
    [SerializeField] private GameObject _closedObject;
    [SerializeField] private Collider2DTrigger _shopZoneTrigger;

    public bool IsNearBy { get; private set; }
    private int _startTime;
    private int _endTime;
    private bool _isOpened;

    private void OnEnable()
    {
        GameTimeManager.Instance.OnIntervalChanged -= OnIntervalChanged;
        GameTimeManager.Instance.OnIntervalChanged += OnIntervalChanged;
        var shopInfoData = GameData.Instance.GetShopInfoData(_shopDataId);
        _startTime = shopInfoData.StartTime;
        _endTime = shopInfoData.EndTime;
        OnIntervalChanged();
    }

    private void OnDisable()
    {
        if (!GameTimeManager.HasInstance) return;
        GameTimeManager.Instance.OnIntervalChanged -= OnIntervalChanged;
    }

    protected override void Update()
    {
        base.Update();
        IsNearBy = _shopZoneTrigger.IsTriggerIn;
    }

    private void OnIntervalChanged()
    {
        var currentTime = GameTimeManager.Instance.CurrentGameTime;
        var currentHHMM = currentTime.Hours * 100 + currentTime.MinutesForUI;
        _isOpened = _startTime <= currentHHMM && currentHHMM < _endTime;
        _openedObject.SetActive(_isOpened);
        _closedObject.SetActive(!_isOpened);
    }

    public void OnShopButtonClick()
    {
        if (!_isOpened)
        {
            GlobalManager.Instance.ShowToastMessage("상점이 준비중이에요!"); // TODO    
            return;
        }

        if (IsNearBy)
        {
            var shopInfoData = GameData.Instance.GetShopInfoData(_shopDataId);
            switch (shopInfoData.ShopType)
            {
                case ShopType.PowderShop:
                {
                    GlobalManager.Instance.ShortcutInvoke(ShortcutType.PowderShop).Forget();
                    break;
                }
                case ShopType.CottonCandyShop:
                case ShopType.BeddingShop:
                {
                    GlobalManager.Instance.ShowToastMessage("개발중 - 지선"); // TODO
                    break;
                }
            }
        }
        else
        {
            GlobalManager.Instance.ShowToastMessage("상점 앞으로 가주세요."); // TODO
        }
    }
}