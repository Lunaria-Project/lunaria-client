// ReSharper disable All
public class GlobalManager : Singleton<GlobalManager>
{
    private bool _isRunning = false;

    protected override void OnInit()
    {
        base.OnInit();
        GameTimeManager.Instance.OnEndDay -= OnEndDay;
        GameTimeManager.Instance.OnEndDay += OnEndDay;
    }

    public void StartDay()
    {
        if (_isRunning)
        {
            LogManager.LogError("이미 게임을 진행중입니다.");
            return;
        }
        _isRunning = true;
        GameTimeManager.Instance.StartDay();
    }

    private void OnEndDay()
    {
        _isRunning = false;
        // TODO(지선): 여기서 영수증이 나오게 작업 필요
        LogManager.Log("하루 끝");
        StartDay();
    }
}