using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleManager : SingletonMonoBehaviour<TitleManager>
{
    [SerializeField] private GameObject _dontDestroyOnLoadObject;

    public void Start()
    {
        GameData.Instance.LoadGameData();
        DontDestroyOnLoad(_dontDestroyOnLoadObject);
    }

    public void OnGoMyhomeButtonClick()
    {
        SceneManager.LoadScene(1);
    }
}