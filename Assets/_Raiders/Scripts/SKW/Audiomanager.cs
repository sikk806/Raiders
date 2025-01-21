using UnityEngine;

public class Audiomanager : MonoBehaviour
{
    public static Audiomanager instance;
 

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
