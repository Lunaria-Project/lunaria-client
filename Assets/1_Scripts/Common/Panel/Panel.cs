using Cysharp.Threading.Tasks;
using UnityEngine;

public class PanelBase : MonoBehaviour
{
    private PanelInfo _panelInfo;

    public virtual UniTask ShowByManager(PanelManager panelManager, PanelInfo panelInfo, params object[] args)
    {
        return UniTask.CompletedTask;
    }

    public virtual UniTask<bool> HideByManager()
    {
        return UniTask.FromResult(true);
    }
    
    public virtual void RefreshByManager()
    {
    }
}

public abstract class Panel<TPanelType> : PanelBase where TPanelType : Panel<TPanelType>
{
    public override async UniTask ShowByManager(PanelManager panelManager, PanelInfo panelInfo, params object[] args)
    {
        await base.ShowByManager(panelManager, panelInfo, args);
        OnShow(args);
    }

    public override async UniTask<bool> HideByManager()
    {
        OnHide();
        return await base.HideByManager();
    }

    public override void RefreshByManager()
    {
        base.RefreshByManager();
        OnRefresh();
    }
    
    protected abstract void OnShow(params object[] args);
    protected abstract void OnHide();
    protected abstract void OnRefresh();
    
    public void HidePanel()
    {
        PanelManager.Instance.HidePanel();
    }
}
