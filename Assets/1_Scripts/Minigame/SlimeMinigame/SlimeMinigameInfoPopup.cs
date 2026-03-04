public class SlimeMinigameInfoPopup : EmptyParamPopup
{
    protected override void OnShow() { }

    protected override void OnHide() { }

    public override void OnHideButtonClick()
    {
        base.OnHideButtonClick();
        PopupManager.Instance.ShowPopupWithEmptyParameter(PopupManager.Type.SlimeMinigameReady);
    }
}