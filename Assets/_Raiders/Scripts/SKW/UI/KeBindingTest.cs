using UnityEngine;

public class KeBindingTest : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeySetting.keys[KeyAction.Q]))
        {
            Debug.Log("up");
        }
        if (Input.GetKey(KeySetting.keys[KeyAction.W]))
        {
            Debug.Log("DOWN");
        }
        if (Input.GetKey(KeySetting.keys[KeyAction.E]))
        {
            Debug.Log("LEFT");
        }
        if (Input.GetKey(KeySetting.keys[KeyAction.R]))
        {
            Debug.Log("RIGHT");
        }
        if (Input.GetKey(KeySetting.keys[KeyAction.Space]))
        {
            Debug.Log("space");
        }
    }
}
