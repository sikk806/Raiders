using TMPro;
using UnityEngine;

public class WskillUI : MonoBehaviour
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
        skillKey.text = $"{KeySetting.keys[KeyAction.W]}";
    }
    
    //01-23 수정  키변경할시 이렇게 수정하기 
    private void OnGUI()
    {
        skillKey.text = $"{KeySetting.keys[KeyAction.W]}";
    }
    
    public void PointerEnter()
    {
        explane.SetActive(true);
        // skillKey.text = $"{KeySetting.keys[KeyAction.W]}";
        name.text = $"카드 쇼 (쿨타임 10초)";
        description.text = $"{W.Instance.UseMp}MP를 소모하여, 정면 방향에 {W.CalculatingDamage()} 만큼의 위력을 갖는 카드 쇼를 2초간 펼친다. 설치형.";
    }

    public void PointerExit()
    {
        explane.SetActive(false);
    }
}
