using System.Collections;
using TMPro;
using UnityEngine;

public class Test : MonoBehaviour
{
    public TextMeshProUGUI[] text;
    
    public TextMeshProUGUI alertText;

    private void Start()
    {
        // 알림 텍스트 초기화
        alertText.text = "";

        // 중복 키 알림 이벤트 연결
        KeyBindingManager.Instance.OnDuplicateKeyAlert += DisplayAlert;

        // UI 초기화
        UpdateUI();
    }

    private void Update()
    {
        if (Input.anyKeyDown)
        {
            UpdateUI();
        }
    }

    private void UpdateUI()
    {
        for (int i = 0; i < text.Length; i++)
        {
            if (KeySetting.keys.TryGetValue((KeyAction)i, out KeyCode keyCode))
            {
                text[i].text = keyCode.ToString();
            }
        }
    }

    private void DisplayAlert(string message)
    {
        alertText.text = message;
        StopCoroutine("ClearAlert"); // 이전 알림 제거
        StartCoroutine("ClearAlert");
    }

    private IEnumerator ClearAlert()
    {
        yield return new WaitForSeconds(2f);
        alertText.text = "";
    }
}