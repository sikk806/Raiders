using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbySceneManager : MonoBehaviour
{

    
    /*
        0. MainScene
        1. LobbyScene
        2. Boss1Scene
        3. Boss2Scene
     */
    public void StartGame()
    {
        SceneManager.LoadScene(2);
    }

    public void OpenOption()
    {
        UIManager.Instance.OpenOptionsMenu();
    }
    

}
