#if UNITY_EDITOR
using UnityEditor;

public static partial class LunariaMenu
{
    private const string ToggleVisualizeColliderMenuName = "Lunaria/Debug/Visualize Collider";
    private const string ToggleLibraryRetryButtonMenuName = "Lunaria/Debug/도서관 다시하기 버튼 표시";

    [MenuItem(ToggleLibraryRetryButtonMenuName)]
    private static void ToggleLibraryRetryButton()
    {
        LibraryMinigamePanel.EditorRetryButtonVisible = !LibraryMinigamePanel.EditorRetryButtonVisible;
    }

    [MenuItem(ToggleLibraryRetryButtonMenuName, true)]
    private static bool ToggleLibraryRetryButtonValidate()
    {
        Menu.SetChecked(ToggleLibraryRetryButtonMenuName, LibraryMinigamePanel.EditorRetryButtonVisible);
        return true;
    }

    [MenuItem(ToggleVisualizeColliderMenuName)]
    private static void ToggleVisualizeCollider()
    {
        var current = PlayerPrefsManager.GetBool(PrefKey.ColliderVisualize);
        PlayerPrefsManager.SetBool(PrefKey.ColliderVisualize, !current);
        LogManager.Log($"Collider Visualize set to {!current}");
    }

    [MenuItem(ToggleVisualizeColliderMenuName, true)]
    private static bool ToggleVisualizeColliderValidate()
    {
        var enabled = PlayerPrefsManager.GetBool(PrefKey.ColliderVisualize);
        Menu.SetChecked(ToggleVisualizeColliderMenuName, enabled);
        return true;
    }
}
#endif
