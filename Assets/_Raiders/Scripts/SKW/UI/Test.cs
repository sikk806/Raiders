using TMPro;
using UnityEngine;

public class Test : MonoBehaviour
{
    public TextMeshProUGUI[] text;
    void Start()
    {
        for (int i = 0; i < text.Length; i++) {
            text[i].text = KeySetting.keys[(KeyAction)i].ToString();
        }
    }

    
    void Update()
    {
        for (int i = 0; i < text.Length; i++) {
            text[i].text = KeySetting.keys[(KeyAction)i].ToString();
        }
    }
}
