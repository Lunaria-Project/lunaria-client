using System.Collections.Generic;
using UnityEngine;

public class InputManager : SingletonMonoBehaviour<InputManager>
{
    [SerializeField] private GameObject _blockerObject;

    private readonly HashSet<object> _blockers = new();

    public bool IsBlocked => _blockers.Count > 0;

    protected override void Awake()
    {
        base.Awake();
        _blockerObject.SetActive(false);
    }

    public void Block(object blocker)
    {
        if (!_blockers.Add(blocker))
        {
            LogManager.LogErrorPack("[InputManager] Block: 이미 Block 중인 대상입니다.", blocker);
            return;
        }

        _blockerObject.SetActive(true);
    }

    public void Unblock(object blocker)
    {
        if (!_blockers.Remove(blocker))
        {
            LogManager.LogErrorPack("[InputManager] Unblock: Block 중이 아닌 대상입니다.", blocker);
            return;
        }

        _blockerObject.SetActive(IsBlocked);
    }
}
