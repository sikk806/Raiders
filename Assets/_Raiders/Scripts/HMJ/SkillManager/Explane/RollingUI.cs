using TMPro;
using UnityEngine;

public class RollingUI : MonoBehaviour
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
        skillKey.text = $"{KeySetting.keys[KeyAction.Space]}";
    }
    
    private void OnGUI()
    {
        skillKey.text = $"{KeySetting.keys[KeyAction.Space]}";
    }
    
    public void PointerEnter()
    {
        explane.SetActive(true);
        // skillKey.text = $"{KeySetting.keys[KeyAction.Space]}";
        name.text = $"구르기 (쿨타임 3초)";
        description.text = $"마우스 커서 방향으로 재빠르게 굴러 이동한다. \n시전 중 무적.";
    }

    public void PointerExit()
    {
        explane.SetActive(false);
    }
}
