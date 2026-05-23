using System;
using UnityEngine;

public class CottonCandyMakeButton : MonoBehaviour
{
    [SerializeField] private int _value;
    [SerializeField] private GameObject _selectedObject;

    public int Value => _value;

    private Action<CottonCandyMakeButton> _onClickAction;

    public void SetOnClickAction(Action<CottonCandyMakeButton> action)
    {
        _onClickAction = action;
    }

    public void SetSelected(bool isSelected)
    {
        _selectedObject.SetActive(isSelected);
    }

    public void OnClick()
    {
        _onClickAction?.Invoke(this);
    }
}
