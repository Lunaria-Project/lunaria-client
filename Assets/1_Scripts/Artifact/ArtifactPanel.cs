using Lunaria;
using UnityEngine;

public class ArtifactPanel : Panel<ArtifactPanel>
{
    public enum ArtifactTabType
    {
        None = -1,
        Craft = 0,
    }

    [SerializeField] private TabGroup _tabGroup;
    [SerializeField] private ArtifactCraftContent _craftContent;

    private ArtifactTabType _currentTabType = ArtifactTabType.None;

    protected void Awake()
    {
        _tabGroup.SetTabChangedAction(OnTabChanged);
    }

    protected override void OnShow(params object[] args)
    {
        var startTabType = args.GetAt(0) is ArtifactTabType tabType ? tabType : ArtifactTabType.Craft;
        _tabGroup.Init((int)startTabType);
    }

    protected override void OnHide()
    {
        switch (_currentTabType)
        {
            case ArtifactTabType.Craft:
            {
                _craftContent.OnHide();
                break;
            }
        }
    }

    private void OnTabChanged(int tabIndex)
    {
        if (_currentTabType == (ArtifactTabType)tabIndex) return;
        
        switch (_currentTabType)
        {
            case ArtifactTabType.Craft:
            {
                _craftContent.OnHide();
                break;
            }
        }

        _currentTabType = (ArtifactTabType)tabIndex;

        switch (_currentTabType)
        {
            case ArtifactTabType.Craft:
            {
                _craftContent.OnShow();
                break;
            }
        }
    }
}
