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

    private void OnEnable()
    {
        GameTimeManager.Instance.OnIntervalChanged -= OnIntervalChanged;
        GameTimeManager.Instance.OnIntervalChanged += OnIntervalChanged;
        var shopInfoData = GameData.Instance.GetShopInfoData(_shopDataId);
        _startTime = shopInfoData.StartTime;
        _endTime = shopInfoData.EndTime;
    }

    private void OnDisable()
    {
        GameTimeManager.Instance.OnIntervalChanged -= OnIntervalChanged;
    }

    private void OnTriggerEnter2D(Collider2D other) { }

    private void OnTriggerExit2D(Collider2D other) { }

    private void OnIntervalChanged()
    {
        var currentTime = GameTimeManager.Instance.CurrentGameTime;
        var currentHHMM = currentTime.Hours * 100 + currentTime.MinutesForUI;
        var isOpened = _startTime <= currentHHMM && currentHHMM < _endTime;
        _openedObject.SetActive(isOpened);
        _closedObject.SetActive(!isOpened);
    }
}