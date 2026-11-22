using UnityEngine;

public class ArtifactCraftContent : MonoBehaviour
{
    public void OnShow()
    {
        gameObject.SetActive(true);
    }

    public void OnHide()
    {
        gameObject.SetActive(false);
    }
}
