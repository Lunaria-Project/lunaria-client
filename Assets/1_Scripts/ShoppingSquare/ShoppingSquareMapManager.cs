using UnityEngine;

public class ShoppingSquareMapManager : BaseMapManager
{
    [SerializeField] private Transform _camera;

    protected override void Update()
    {
        base.Update();
        _camera.position = GetPlayerPosition();
    }
}
