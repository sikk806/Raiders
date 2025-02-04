using UnityEngine;

public class PlayerCanvas : MonoBehaviour
{
    public static PlayerCanvas Instance;
 
    void Start()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

    }

}
