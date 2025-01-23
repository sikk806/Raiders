using System;
using TMPro;
using UnityEditor.Networking.PlayerConnection;
using UnityEngine;

public class QskillUI : MonoBehaviour
{
    [SerializeField]
    GameObject explane;
    [SerializeField]
    TextMeshProUGUI skillKey;
    [SerializeField]
    TextMeshProUGUI name;
    [SerializeField]
    TextMeshProUGUI description;

    private void Start()
    {
        skillKey.text = $"{KeySetting.keys[KeyAction.Q]}";

    }
    //01-23 수정  키변경할시 이렇게 수정하기 
    private void OnGUI()
    {
        skillKey.text = $"{KeySetting.keys[KeyAction.Q]}";
    }

    public void PointerEnter()
    {
        explane.SetActive(true);
        // skillKey.text = $"{KeySetting.keys[KeyAction.Q]}";
        name.text = $"매직 불릿 (쿨타임 3초)";
        description.text = $"{Q.Instance.UseMp}MP를 소모하여, 정면으로 {Q.CalculatingDamage()} 만큼의 위력을 갖는 마법 총알을 2초간 발사한다. 제어불가.";
    }

    public void PointerExit()
    {
        explane.SetActive(false);
    }
}
