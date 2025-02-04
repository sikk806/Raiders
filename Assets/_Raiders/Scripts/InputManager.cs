using System;
using UnityEngine;

public class InputManager : MonoBehaviour 
{
    static InputManager instance;
    public Action KeyAction = null; // InputDelegate

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public void OnUpdate()
    {
        if(Input.anyKey == false)
        {
            return;
        }

        if(KeyAction != null)
        {
            KeyAction.Invoke();
        }
    }
}
