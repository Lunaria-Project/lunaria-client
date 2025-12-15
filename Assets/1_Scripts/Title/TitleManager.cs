using UnityEngine.SceneManagement;

public class TitleManager : SingletonMonoBehaviour<TitleManager>
{

    public void Start()
    {
        GameData.Instance.LoadGameData();
    }

    public void OnGoMyhomeButtonClick()
    {
        SceneManager.LoadScene(1);
    }
}