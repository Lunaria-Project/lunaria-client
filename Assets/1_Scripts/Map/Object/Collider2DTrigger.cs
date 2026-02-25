using UnityEngine;

public class Collider2DTrigger : MonoBehaviour
{
    public bool IsTriggerIn { get; private set; }

    private void OnTriggerEnter2D(Collider2D other)
    {
        IsTriggerIn = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        IsTriggerIn = false;
    }
}