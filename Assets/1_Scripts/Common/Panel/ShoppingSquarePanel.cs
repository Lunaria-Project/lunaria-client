using UnityEngine;

public class ShoppingSquarePanel : Panel<ShoppingSquarePanel>
{
    [SerializeField] TopTimeUI _timeUI;

    protected override void OnShow(params object[] args)
    {
        _timeUI.OnShow();
    }
    protected override void OnHide()
    {
        _timeUI.OnHide();
    }
    protected override void OnRefresh() { }
}