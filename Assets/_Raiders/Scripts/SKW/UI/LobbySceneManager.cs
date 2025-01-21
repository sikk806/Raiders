using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbySceneManager : MonoBehaviour
{

    
    public void StartGame()
    {
        SceneManager.LoadScene(2);
    }

    public void OpenOption()
    {
        UIManager.Instance.OpenOptionsMenu();
    }
    

}
