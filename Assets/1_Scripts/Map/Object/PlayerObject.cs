using UnityEngine;

public class PlayerObject : MovableObject
{
    public void Init(Vector2 position)
    {
        InitPositionAndScale(position, new Vector2(0, 86), 0.5f, 1);
    }

    protected override void Update()
    {
        base.Update();
        if (!GlobalManager.Instance.CanPlayerMove()) return;

        var previousMoveDirection = MoveDirection;
        MoveDirection = Vector2.zero;
        var moveUp = Input.GetKey(KeyCode.W);
        var moveDown = Input.GetKey(KeyCode.S);
        var moveRight = Input.GetKey(KeyCode.D);
        var moveLeft = Input.GetKey(KeyCode.A);
        if (moveUp && moveDown)
        {
            MoveDirection += previousMoveDirection.y > 0 ? Vector2.up : Vector2.down;
        }
        else if (moveUp)
        {
            MoveDirection += Vector2.up;
        }
        else if (moveDown)
        {
            MoveDirection += Vector2.down;
        }

        if (moveLeft && moveRight)
        {
            MoveDirection += previousMoveDirection.x > 0 ? Vector2.right : Vector2.left;
        }
        else if (moveLeft)
        {
            MoveDirection += Vector2.left;
        }
        else if (moveRight)
        {
            MoveDirection += Vector2.right;
        }

        MoveDirection.Normalize();
        if (Input.GetKey(KeyCode.LeftShift))
        {
            MoveDirection *= GameSetting.Instance.SpeedUpRate;
        }
    }

    protected override int GetCharacterDataId()
    {
        return 1000011;
    }
}