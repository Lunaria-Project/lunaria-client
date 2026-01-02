using DG.Tweening;
using UnityEngine;

public class MyhomeArtifactUI : MonoBehaviour
{
    [SerializeField] Transform _backgroundTemp;

    private bool _isRotating;
    private int _currentZRotation;

    public void OnShow()
    {
        GlobalManager.Instance.OnQKeyDown -= OnQKeyDown;
        GlobalManager.Instance.OnQKeyDown += OnQKeyDown;
        GlobalManager.Instance.OnEKeyDown -= OnEKeyDown;
        GlobalManager.Instance.OnEKeyDown += OnEKeyDown;
    }

    public void OnHide()
    {
        GlobalManager.Instance.OnQKeyDown -= OnQKeyDown;
        GlobalManager.Instance.OnEKeyDown -= OnEKeyDown;
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