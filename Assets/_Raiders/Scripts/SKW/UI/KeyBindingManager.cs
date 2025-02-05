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
    F,
    KEYCOUNT
}

public static class KeySetting
{
    public static Dictionary<KeyAction, KeyCode> keys = new Dictionary<KeyAction, KeyCode>();
    
}

public class KeyBindingManager : MonoBehaviour
{
    //싱글턴 만들기
    public static KeyBindingManager Instance { get; private set; }
    public Action<string> OnDuplicateKeyAlert;
    //기보니 정의
    private KeyCode[] defaultKeys = new KeyCode[]
        { KeyCode.Q, KeyCode.W, KeyCode.E, KeyCode.R,KeyCode.Space,KeyCode.F };

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
        if (key == -1) return;

        Event keyEvent = Event.current;

        if (keyEvent.isKey)
        {
            KeyCode newKey = keyEvent.keyCode;

            // 키 중복 확인
            if (IsDuplicateKey(newKey))
            {
                OnDuplicateKeyAlert?.Invoke($"키 '{newKey}'는 이미 사용 중입니다!"); // 중복 알림
                key = -1;
                return;
            }

            // 키 변경 처리
            KeySetting.keys[(KeyAction)key] = newKey;
            OnDuplicateKeyAlert?.Invoke($"키 '{newKey}'로 변경 완료!"); // 변경 성공 메시지
            key = -1;
        }
    }
    
    // 키 중복 여부 확인
    private bool IsDuplicateKey(KeyCode keyCode)
    {
        foreach (var key in KeySetting.keys.Values)
        {
            if (key == keyCode)
            {
                return true;
            }
        }
        return false;
    }


    private int key = -1;
    public void ChageKeys(int num)
    {
        key = num;
    }
}
