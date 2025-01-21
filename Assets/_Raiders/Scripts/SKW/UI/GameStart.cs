using UnityEngine;
using UnityEngine.UI;

public class GameStart : MonoBehaviour
{
    public Button StartButton;
    public string sceneName = "PlayerTest";
    void Start()
    {
        if (StartButton ==null)
        {
            StartButton = GameObject.Find("BtnGameStart")?.GetComponent<Button>();
        }
        StartButton.onClick.AddListener(() => UIManager.Instance.LoadSceneByName(sceneName));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
