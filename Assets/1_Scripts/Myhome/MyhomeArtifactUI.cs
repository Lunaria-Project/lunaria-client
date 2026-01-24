using System.Collections.Generic;
using DG.Tweening;
using Generated;
using UnityEngine;

public class MyhomeArtifactUI : MonoBehaviour
{
    [SerializeField] Transform _backgroundTemp;
    [SerializeField] MyhomeArtifactUICell[] _cells;

    private readonly List<int> _itemDataIds = new();
    private bool _isRotating;
    private Vector3 _rotation;
    private Vector3 _rotationCache = Vector3.zero;

    public void OnShow()
    {
        _isRotating = false;
        _rotation = Vector3.zero;
        _backgroundTemp.localRotation = Quaternion.Euler(_rotation);

        _itemDataIds.Clear();
        foreach (var (id, quantity) in UserData.Instance.GetItemQuantities(ItemType.Artifact))
        {
            if (quantity <= 0)
            {
                LogManager.LogErrorFormat("UserData.ItemDictionary에 개수가 0개인 아이템이 있습니다.", id);
                continue;
            }
            _itemDataIds.Add(id);
        }
        _itemDataIds.Sort((left, right) =>
        {
            var leftData = GameData.Instance.GetItemData(left);
            var rightData = GameData.Instance.GetItemData(right);
            return leftData.Order.CompareTo(rightData.Order);
        });

        var selectedIndex = GetSelectedIndex();
        for (var i = 0; i < _cells.Length; i++)
        {
            _cells[i].SetSelected(i == selectedIndex);
        }
        SetItem(selectedIndex, 0);
        SetItem(selectedIndex + 1, 1);
        SetItem(selectedIndex - 1, _itemDataIds.Count - 1);
        RefreshCellRotation(true);
        UserData.Instance.SetEquippedArtifact(_itemDataIds.GetAt(0));

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

    private void DoRotation(bool isClockwise)
    {
        if (_isRotating) return;

        _isRotating = true;

        var selectedIndex = GetSelectedIndex();
        var selectedCellItemId = _cells.GetAt(selectedIndex).ItemDataId;
        var selectedCellItemIndex = _itemDataIds.IndexOf(selectedCellItemId);
        SetItem(selectedIndex - 2, selectedCellItemIndex - 2);
        SetItem(selectedIndex + 2, selectedCellItemIndex + 2);
        

        _rotation.z += isClockwise ? 60 : -60;
        var newSelectedIndex = GetSelectedIndex();
        var newSelectedCellItemId = _cells.GetAt(newSelectedIndex).ItemDataId;
        UserData.Instance.SetEquippedArtifact(newSelectedCellItemId);
        
        _backgroundTemp.DOLocalRotate(_rotation, 0.1f, RotateMode.Fast).SetEase(Ease.OutQuad)
            .OnComplete(() =>
            {
                _isRotating = false;

                
                for (var i = 0; i < _cells.Length; i++)
                {
                    var isSelected = i == newSelectedIndex;
                    _cells[i].SetSelected(isSelected);
                }
                RefreshCellRotation(false);
            })
            .SetUpdate(true);
    }

    private void OnQKeyDown()
    {
        DoRotation(false);
    }

    private void OnEKeyDown()
    {
        DoRotation(true);
    }

    private int GetSelectedIndex()
    {
        var zRotation = _rotation.z;
        var normalizedRotation = Mathf.Repeat(zRotation, 360.0f);
        var stepIndex = Mathf.RoundToInt(normalizedRotation / 60.0f) % 6;
        return (stepIndex + 5) % 6;
    }

    private void SetItem(int cellIndex, int itemIndex)
    {
        cellIndex = Mathf.FloorToInt(Mathf.Repeat(cellIndex, _cells.Length));
        itemIndex = Mathf.FloorToInt(Mathf.Repeat(itemIndex, _itemDataIds.Count));

        _cells.GetAt(cellIndex).SetData(_itemDataIds.GetAt(itemIndex));
    }

    private void RefreshCellRotation(bool isInit)
    {
        var selectedIndex = GetSelectedIndex();
        for (var i = 0; i < _cells.Length; i++)
        {
            var cellRotation = (i - selectedIndex + 3.5f) * 60f;
            _rotationCache.z = cellRotation;
            if (isInit)
            {
                _cells[i].ImageRectTransform.localRotation = Quaternion.Euler(_rotationCache);
            }
            else
            {
                _cells[i].ImageRectTransform.DOLocalRotate(_rotationCache, 0.1f, RotateMode.Fast).SetEase(Ease.OutQuad);
            }
        }
    }
}