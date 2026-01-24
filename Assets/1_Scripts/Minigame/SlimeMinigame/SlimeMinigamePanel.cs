using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Lunaria;
using UnityEngine;
using Random = UnityEngine.Random;

public class SlimeMinigamePanel : Panel<SlimeMinigamePanel>
{
    [SerializeField] private Text _remainTimeText;
    [SerializeField] private Text _countText;
    [SerializeField] private SlimeBlock[] _slimeBlocks;
    [SerializeField] private SlimeMinigameConfig _config;

    private bool _isInitialized;
    private float _remainTime;
    private readonly List<Coroutine> _coroutines = new();
    private int _slimeCount;

    private void Awake()
    {
        foreach (var slimeBlock in _slimeBlocks)
        {
            slimeBlock.SetOnTouchSlime(OnTouchSlime);
        }
    }

    private void Update()
    {
        if (!_isInitialized) return;
        _remainTime -= Time.deltaTime;
        _remainTimeText.SetText(Mathf.RoundToInt(_remainTime).ToString());
    }

    protected override void OnShow(params object[] args)
    {
        _isInitialized = false;
        _remainTime = _config.MinigameSeconds;
        _slimeCount = 0;

        HideAll();

        PopupManager.Instance.ShowPopup(PopupManager.Type.CountDown, new CountDownPopupParameter { CountDownSeconds = 3 })
            .SetOnHideAction(() =>
            {
                _isInitialized = true;
                StartSlimeCoroutine();
                Invoke(nameof(StopSlimeCoroutine), _config.MinigameSeconds);
            });
    }

    protected override void OnHide()
    {
        HideAll();
    }

    protected override void OnRefresh()
    {
        _countText.SetText(_slimeCount.ToPrice());
    }

    private void StartSlimeCoroutine()
    {
        _coroutines.Clear();
        for (var i = 0; i < _config.SlimeShowCount; i++)
        {
            _coroutines.Add(StartCoroutine(CoShowNpcDialog()));
        }
    }

    private void StopSlimeCoroutine()
    {
        foreach (var coroutine in _coroutines)
        {
            StopCoroutine(coroutine);
        }
        _coroutines.Clear();
        HidePanel();
    }

    private IEnumerator CoShowNpcDialog()
    {
        var waitFirstSeconds = _config.GetShowDelayRandomSeconds(1);
        yield return UniTask.Delay(TimeUtil.SecondsToMillis(waitFirstSeconds));
        while (true)
        {
            var randomDialogIndex = Random.Range(0, _slimeBlocks.Length);
            var slimeBlock = _slimeBlocks.GetAt(randomDialogIndex);
            if (slimeBlock.IsShowing) continue;

            var randomOrder = _config.GetRandomSlimeOrder();
            var delaySeconds = _config.GetShowDelayRandomSeconds(randomOrder);
            yield return slimeBlock.Show(randomOrder, delaySeconds).ToCoroutine();
            yield return UniTask.Delay(TimeUtil.SecondsToMillis(_config.GetHideDelayRandomSeconds()));
        }
    }

    private void HideAll()
    {
        foreach (var slime in _slimeBlocks)
        {
            slime.Hide();
        }
    }

    private void OnTouchSlime(int slimeOrder)
    {
        _slimeCount++;
        OnRefresh();
    }
}