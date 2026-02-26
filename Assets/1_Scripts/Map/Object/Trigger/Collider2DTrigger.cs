using UnityEngine;

public class Collider2DTrigger : MonoBehaviour
{
    public bool IsTriggerIn { get; private set; }

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        IsTriggerIn = true;
    }

    protected virtual void OnTriggerStay2D(Collider2D other) { }

    protected virtual void OnTriggerExit2D(Collider2D other)
    {
        IsTriggerIn = false;
    }
}