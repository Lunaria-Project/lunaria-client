using DG.Tweening;
using Generated;
using Lunaria;
using UnityEngine;

public class MyhomeMainPanel : Panel<MyhomeMainPanel>
{
    [Header("Wallet"), Space(8)]
    [SerializeField] Text _walletText;

    [Header("Time"), Space(8)]
    [SerializeField] MyhomeTimeUI _timeUI;

    [Header("마도구"), Space(8)]
    [SerializeField] Transform _backgroundTemp;
    [SerializeField] DOTweenAnimation _cellAnimation;

    private bool _isRotating;
    private int _currentZRotation;

    protected override void OnShow(params object[] args)
    {
        GlobalManager.Instance.OnQKeyDown -= OnQKeyDown;
        GlobalManager.Instance.OnQKeyDown += OnQKeyDown;
        GlobalManager.Instance.OnEKeyDown -= OnEKeyDown;
        GlobalManager.Instance.OnEKeyDown += OnEKeyDown;
        _timeUI.OnShow();
        OnShowUI();
    }

    protected override void OnHide()
    {
        GlobalManager.Instance.OnQKeyDown -= OnQKeyDown;
        GlobalManager.Instance.OnEKeyDown -= OnEKeyDown;
        _timeUI.OnHide();
    }

    protected override void OnRefresh() { }

    private void OnShowUI()
    {
        _walletText.SetText(UserData.Instance.GetItemQuantity(ItemType.MainCoin).ToPrice());
    }

    private void OnQKeyDown()
    {
        if (_isRotating) return;

        _isRotating = true;
        _currentZRotation -= 60;
        _backgroundTemp.DOLocalRotate(new Vector3(0f, 0f, _currentZRotation), 0.1f, RotateMode.Fast).SetEase(Ease.OutQuad).OnComplete(() => _isRotating = false).SetUpdate(true);
    }

    private void OnEKeyDown()
    {
        if (_isRotating) return;

        _isRotating = true;
        _currentZRotation += 60;
        _backgroundTemp.DOLocalRotate(new Vector3(0f, 0f, _currentZRotation), 0.1f, RotateMode.Fast).SetEase(Ease.OutQuad).OnComplete(() => _isRotating = false).SetUpdate(true);
    }
}