using TMPro;
using UnityEngine;

public class EskillUI : MonoBehaviour
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
        skillKey.text = $"{KeySetting.keys[KeyAction.E]}";

    }
    public void PointerEnter()
    {
        explane.SetActive(true);
        skillKey.text = $"{KeySetting.keys[KeyAction.E]}";
        name.text = $"매직 실드 (쿨타임 8초)";
        description.text = $"{E.Instance.UseMp}MP를 소모하여, 30 만큼의 대미지를 막아주는 \n방어막을 몸에 두른다.";
    }

    public void PointerExit()
    {
        explane.SetActive(false);
    }
}
