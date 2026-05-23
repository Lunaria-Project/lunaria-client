using System;
using UnityEngine;

public class CottonCandyMinigameCustomerBlock : MonoBehaviour
{
    [SerializeField] private RectTransform[] _customersPositions;
    [SerializeField] private RectTransform _startPositions;
    [SerializeField] private CottonCandyCustomer[] _customers;

    private CottonCandyMinigameConfig _config;
    private System.Random _random;
    private float _spawnTimer;
    private Action<CottonCandyCustomer> _onCurrentCustomerChanged;

    public void SetOnCurrentCustomerChangedAction(Action<CottonCandyCustomer> action)
    {
        _onCurrentCustomerChanged = action;
    }

    public void Init(CottonCandyMinigameConfig config)
    {
        _config = config;
        var seed = Environment.TickCount;
        _random = new System.Random(seed);
        LogManager.Log($"[CottonCandyMinigame] seed={seed}");
        _spawnTimer = 0;
        foreach (var customer in _customers)
        {
            customer.Hide();
        }
    }

    public void UpdateCustomer(float deltaTime)
    {
        UpdateSpawn(deltaTime);
        UpdateMove(deltaTime);
    }

    public bool TryServeFrontCustomer(out int scoreReward)
    {
        scoreReward = 0;
        foreach (var customer in _customers)
        {
            if (!customer.IsReadyToOrder) continue;

            scoreReward = customer.Order.ScoreReward;
            customer.Hide();
            ShiftForward();
            _onCurrentCustomerChanged?.Invoke(null);
            return true;
        }
        return false;
    }

    private void ShiftForward()
    {
        foreach (var customer in _customers)
        {
            if (!customer.IsActive) continue;
            if (customer.SlotIndex <= 0) continue;

            var newSlot = customer.SlotIndex - 1;
            customer.MoveToSlot(newSlot, _customersPositions[newSlot].position);
        }
    }

    private void UpdateSpawn(float deltaTime)
    {
        _spawnTimer -= deltaTime;
        if (_spawnTimer > 0 && HasAnyCustomer()) return;

        var customer = FindIdleCustomer();
        if (customer == null) return;

        var slot = FindFrontEmptySlot();
        if (slot < 0) return;

        customer.Spawn(_startPositions.position, slot, _customersPositions[slot].position);

        var min = _config.CustomerSpawnMinSeconds;
        var max = _config.CustomerSpawnMaxSeconds;
        _spawnTimer = min + (float)_random.NextDouble() * (max - min);
    }

    private void UpdateMove(float deltaTime)
    {
        foreach (var customer in _customers)
        {
            var justArrived = customer.UpdateMove(deltaTime, _config.CustomerMoveSpeed);
            if (justArrived && customer.SlotIndex == 0)
            {
                _onCurrentCustomerChanged?.Invoke(customer);
            }
        }
    }

    private bool HasAnyCustomer()
    {
        foreach (var customer in _customers)
        {
            if (customer.IsActive) return true;
        }
        return false;
    }

    private CottonCandyCustomer FindIdleCustomer()
    {
        foreach (var customer in _customers)
        {
            if (!customer.IsActive) return customer;
        }
        return null;
    }

    private int FindFrontEmptySlot()
    {
        for (var slot = 0; slot < _customersPositions.Length; slot++)
        {
            if (IsSlotEmpty(slot)) return slot;
        }
        return -1;
    }

    private bool IsSlotEmpty(int slot)
    {
        foreach (var customer in _customers)
        {
            if (customer.SlotIndex == slot) return false;
        }
        return true;
    }
}
