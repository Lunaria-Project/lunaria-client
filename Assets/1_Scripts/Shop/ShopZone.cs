using Cysharp.Threading.Tasks;
using Generated;
using Sirenix.OdinInspector;
using UnityEngine;

public class ShopZone : MonoBehaviour
{
#if UNITY_EDITOR
    [ValueDropdown("@DataIdDropDownList.GetShopDataIds()")]
#endif
    [SerializeField] private int _shopDataId;
    [SerializeField] private GameObject _openedObject;
    [SerializeField] private GameObject _closedObject;

    private int _startTime;
    private int _endTime;
    private bool _isNearBy;

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

    private void OnTriggerEnter2D(Collider2D other)
    {
        _isNearBy = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        _isNearBy = false;
    }

    private void OnIntervalChanged()
    {
        var currentTime = GameTimeManager.Instance.CurrentGameTime;
        var currentHHMM = currentTime.Hours * 100 + currentTime.MinutesForUI;
        var isOpened = _startTime <= currentHHMM && currentHHMM < _endTime;
        _openedObject.SetActive(isOpened);
        _closedObject.SetActive(!isOpened);
    }

    public void OnClosedShopButtonClick()
    {
        GlobalManager.Instance.ShowToastMessage("상점이 준비중이에요!"); // TODO
    }

    public void OnOpenedShopButtonClick()
    {
        if (_isNearBy)
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