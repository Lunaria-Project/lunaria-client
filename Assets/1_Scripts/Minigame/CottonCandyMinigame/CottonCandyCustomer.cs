using UnityEngine;

public class CottonCandyCustomer : MonoBehaviour
{
    [SerializeField] private GameObject _root;
    [SerializeField] private CottonCandyOrderBlock _orderBlock;

    public CottonCandyOrder Order { get; private set; }
    public bool HasOrder => Order != null;
    public int SlotIndex { get; private set; } = -1;
    public bool IsReadyToOrder => HasOrder && SlotIndex == 0 && _arrived;
    public bool IsActive => SlotIndex >= 0;

    private Vector3 _targetPosition;
    private bool _arrived;

    public void Spawn(Vector3 startPosition, int slotIndex, Vector3 slotPosition)
    {
        SlotIndex = slotIndex;
        _targetPosition = slotPosition;
        _arrived = false;
        _root.SetActive(true);
        transform.position = startPosition;
        _orderBlock.Hide();
    }

    public void MoveToSlot(int slotIndex, Vector3 slotPosition)
    {
        SlotIndex = slotIndex;
        _targetPosition = slotPosition;
        _arrived = false;
    }

    public bool UpdateMove(float deltaTime, float speed)
    {
        if (!IsActive) return false;

        var justArrived = false;
        if (!_arrived)
        {
            transform.position = Vector3.MoveTowards(transform.position, _targetPosition, speed * deltaTime);
            if (transform.position == _targetPosition)
            {
                _arrived = true;
                justArrived = true;
            }
        }

        if (!(_arrived && SlotIndex == 0))
        {
            _orderBlock.Hide();
        }
        return justArrived;
    }

    public void SetOrder(CottonCandyOrder order)
    {
        Order = order;
        _root.SetActive(true);
        _orderBlock.SetOrder(order);
    }

    public void Hide()
    {
        Order = null;
        SlotIndex = -1;
        _arrived = false;
        _root.SetActive(false);
        _orderBlock.Hide();
    }
}
