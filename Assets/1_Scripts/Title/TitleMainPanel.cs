using UnityEngine.SceneManagement;

public class TitleMainPanel : Panel<TitleMainPanel>
{
    protected override void OnShow(params object[] args)
    {
    }

    protected override void OnHide()
    {
    }

    protected override void OnRefresh()
    {
    }

    public void OnGoMyhomeButtonClick()
    {
        SceneManager.LoadScene(1);
    }
}
