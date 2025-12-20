using UnityEngine.SceneManagement;

public class TitleMainPanel : Panel<TitleMainPanel>
{
    protected override void OnShow(params object[] args) { }

    protected override void OnHide() { }

    protected override void OnRefresh() { }

    public void OnGoMyhomeButtonClick()
    {
        var userDataInfo = new UserDataInfo();
        UserData.Instance.Init(userDataInfo);
        SceneManager.LoadScene(1);
    }

    public void OnGoMyhomeWithCheatButtonClick()
    {
        var userDataInfo = new UserDataInfo();
        var userData = ResourceManager.Instance.LoadScriptableObject<UserDataCheatAsset>("user_data_cheat_asset");
        foreach (var (id, quantity) in userData.UserInventory)
        {
            userDataInfo.AddItem(id.DataId, quantity);
        }
        UserData.Instance.Init(userDataInfo);
        SceneManager.LoadScene(1);
    }
}