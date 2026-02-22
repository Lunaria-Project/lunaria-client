using Cysharp.Threading.Tasks;
using Generated;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleMainPanel : Panel<TitleMainPanel>
{
    [SerializeField] GameObject _editorObject;
#if UNITY_EDITOR
    [ValueDropdown("@DataIdDropDownList.GetCutsceneDataIds()")]
#endif
    [SerializeField] int _cutsceneId;

    protected override void OnShow(params object[] args)
    {
        _editorObject.SetActive(false);
#if UNITY_EDITOR
        _editorObject.SetActive(true);
#endif
    }

    protected override void OnHide() { }

    protected override void OnRefresh() { }

    private async UniTask StartGame(UserDataInfo info)
    {
        UserData.Instance.Init(info, true);
        await SceneManager.LoadSceneAsync(1);
        MapManager.Instance.SetMap(MapType.Myhome);
        GlobalManager.Instance.StartDay();
    }

    public void OnStartButtonClick()
    {
        StartGame(new UserDataInfo()).Forget();
    }

    public void OnStartWithCheatButtonClick()
    {
#if UNITY_EDITOR
        var userDataInfo = new UserDataInfo();
        var userData = ResourceManager.Instance.LoadScriptableObject<UserDataCheatAsset>("user_data_cheat_asset");
        foreach (var (id, quantity) in userData.UserInventory)
        {
            userDataInfo.AddItem(id.DataId, quantity);
        }
        userDataInfo.SlimeGauge = userData.InitSlimeGauge;
        StartGame(userDataInfo).Forget();
#endif
    }

    public void OnCutsceneTestButtonClick()
    {
        CutsceneManager.Instance.PlayCutscene(_cutsceneId).Forget();
    }
}