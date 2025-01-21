using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum KeyAction
{
    Q,
    W,
    E,
    R,
    Space,
    KEYCOUNT
}

public static class KeySetting
{
    public static Dictionary<KeyAction, KeyCode> keys = new Dictionary<KeyAction, KeyCode>();
    
}

public class KeyBindingManager : MonoBehaviour
{
    public static KeyBindingManager Instance { get; private set; }
    
    
    private KeyCode[] defaultKeys = new KeyCode[]
        { KeyCode.Q, KeyCode.W, KeyCode.E, KeyCode.R,KeyCode.Space, };

    private void Awake()
    {
        // 이미 다른 인스턴스가 존재하면 파괴
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        // 인스턴스 설정
        Instance = this;
        
        DontDestroyOnLoad(gameObject);
        
        for (int i = 0; i < (int)KeyAction.KEYCOUNT; i++)
        {   
            KeySetting.keys.Add((KeyAction)i, defaultKeys[i]);
        }
    }
    
    //키 입력 등의 이벤트가 발생할 때 호출됨
    private void OnGUI()
    {
        Event KeyEvent = Event.current;

        //눌렸는지 안눌렸는 지 확인법
        if (KeyEvent.isKey)
        {
            KeySetting.keys[(KeyAction)key] = KeyEvent.keyCode;
            key = -1;
        }
    }

    private int key = -1;
    public void ChageKeys(int num)
    {
        key = num;
    }
}
