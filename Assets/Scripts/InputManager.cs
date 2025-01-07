using System;
using UnityEngine;

public class InputManager
{
    public Action KeyAction = null; // InputDelegate

    // Update is called once per frame
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
