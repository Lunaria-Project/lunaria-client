using UnityEngine;

public class OneWayZone : Collider2DTrigger
{
    [SerializeField] private Vector2 _moveDirection;

    protected override void OnTriggerStay2D(Collider2D other)
    {
        base.OnTriggerStay2D(other);
        if (!other.TryGetComponent<MovableObject>(out var movableObject)) return;
        var moveDirection = movableObject.MoveDirection;
        if (moveDirection == Vector2.zero)
        {
            movableObject.SetForceMoveDirection(Vector2.zero);
        }
        else if (moveDirection.x < 0 || moveDirection.y > 0)
        {
            movableObject.SetForceMoveDirection(_moveDirection.normalized);
        }
        else if (moveDirection.x > 0 || moveDirection.y < 0)
        {
            movableObject.SetForceMoveDirection(-_moveDirection.normalized);
        }
    }

    protected override void OnTriggerExit2D(Collider2D other)
    {
        base.OnTriggerExit2D(other);
        if (!other.TryGetComponent<MovableObject>(out var movableObject)) return;
        movableObject.SetForceMoveDirection(Vector2.zero);
    }
}