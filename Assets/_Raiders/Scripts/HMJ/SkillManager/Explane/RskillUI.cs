using TMPro;
using UnityEngine;

public class RskillUI : MonoBehaviour
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
        skillKey.text = $"{KeySetting.keys[KeyAction.R]}";
    }
    private void OnGUI()
    {
        skillKey.text = $"{KeySetting.keys[KeyAction.R]}";
    }
    public void PointerEnter()
    {
        explane.SetActive(true);
        // skillKey.text = $"{KeySetting.keys[KeyAction.R]}";
        name.text = $"체인즈 저지먼트 (쿨타임 60초)";
        description.text = $"{R.Instance.UseMp}MP를 소모하여 {R.CalculatingDamage()}만큼의 위력을 갖는 사슬의 심판장을 4초간 연다. 시전 중 무적. 제어불가.";
    }

    public void PointerExit()
    {
        explane.SetActive(false);
    }
}
