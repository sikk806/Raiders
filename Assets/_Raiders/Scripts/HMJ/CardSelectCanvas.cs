using UnityEngine;

public class CardSelectCanvas : MonoBehaviour
{
    static CardSelectCanvas instance;

    private void Start()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
}
