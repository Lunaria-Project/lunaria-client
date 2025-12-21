using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleMainPanel : Panel<TitleMainPanel>
{
    [SerializeField] GameObject _editorObject;

    protected override void OnShow(params object[] args)
    {
        _editorObject.SetActive(false);
#if UNITY_EDITOR
        _editorObject.SetActive(true);
#endif
    }

    protected override void OnHide() { }

    protected override void OnRefresh() { }

    public void OnGoMyhomeButtonClick()
    {
        StartGame(new UserDataInfo()).Forget();
    }

    public void OnGoMyhomeWithCheatButtonClick()
    {
#if UNITY_EDITOR
        var userDataInfo = new UserDataInfo();
        var userData = ResourceManager.Instance.LoadScriptableObject<UserDataCheatAsset>("user_data_cheat_asset");
        foreach (var (id, quantity) in userData.UserInventory)
        {
            userDataInfo.AddItem(id.DataId, quantity);
        }
        StartGame(userDataInfo).Forget();
#endif
    }

    private async UniTask StartGame(UserDataInfo info)
    {
        UserData.Instance.Init(info);
        await SceneManager.LoadSceneAsync(1);
        GlobalManager.Instance.StartDay();
    }
}