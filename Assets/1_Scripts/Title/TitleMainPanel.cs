using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleMainPanel : Panel<TitleMainPanel>
{
    [SerializeField] GameObject _editorObject;
    [SerializeField] GameObject _loadButton;
#if UNITY_EDITOR
    [ValueDropdown("@DataIdDropDownList.GetCutsceneDataIds()")]
#endif
    [SerializeField] int _cutsceneId;

    private bool _isStarting;

    protected override void OnShow(params object[] args)
    {
        _isStarting = false;
        _loadButton.SetActive(UserData.Instance.HasSaveData);
        _editorObject.SetActive(false);
#if UNITY_EDITOR
        _editorObject.SetActive(true);
#endif
    }

    protected override void OnHide() { }

    private async UniTask StartGame(UserDataInfo info, bool addInitialItems)
    {
        if (_isStarting) return;
        _isStarting = true;
        UserData.Instance.Init(info, addInitialItems);
        await SceneManager.LoadSceneAsync(1);
        GlobalManager.Instance.StartDay();
        GlobalManager.Instance.OnChangeMap(MapType.Myhome);
    }

    public void OnStartButtonClick()
    {
        var userDataInfo = new UserDataInfo
        {
            CurrentDay = 1,
        };
        if (!UserData.Instance.HasSaveData)
        {
            StartGame(userDataInfo, true).Forget();
            return;
        }

        // TODO(지선): 세이브 덮어쓰기 안내용 임시 팝업. 정식 UI 작업 시 제거 또는 교체 필요
        var parameter = new SystemTwoButtonParameter
        {
            Description = "하루가 종료될때 데이터가 덮어쓰기 됩니다",
            ConfirmButtonText = LocalizationKey.ConfirmButton,
            CancelButtonText = LocalizationKey.CancelButton,
            OnConfirm = () => { StartGame(userDataInfo, true).Forget(); },
        };
        PopupManager.Instance.ShowPopup(PopupManager.Type.SystemButton, parameter);
    }

    public void OnLoadButtonClick()
    {
        if (!UserData.Instance.TryLoad(out var userDataInfo)) return;
        StartGame(userDataInfo, false).Forget();
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
        foreach (var familiar in userData.SummonedFamiliars)
        {
            userDataInfo.AddFamiliar(familiar.DataId);
        }
        userDataInfo.SlimeGauge = Mathf.Min(userData.InitSlimeGauge, 100);
        userDataInfo.CurrentDay = 1;
        StartGame(userDataInfo, true).Forget();
#endif
    }
}