using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbySceneManager : MonoBehaviour
{

    
    /*
     * 1:SKW
     * 2:PLAYERTEST
     */
    public void StartGame()
    {
        SceneManager.LoadScene(1);
    }

    public void OpenOption()
    {
        UIManager.Instance.OpenOptionsMenu();
    }
    

}
