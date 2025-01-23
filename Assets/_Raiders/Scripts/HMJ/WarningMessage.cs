using TMPro;
using UnityEngine;

public class WarningMessage : MonoBehaviour
{
    public static WarningMessage Instance;
    [SerializeField]
    GameObject warning;
    [SerializeField]
    TextMeshProUGUI warningText;

    private void Start()
    {
        Instance = this;
    }
    public void NoPotion()
    {
        if (!warning.activeSelf)
        {
            warning.SetActive(true);
            warningText.text = "포션이 부족합니다.";
            Invoke("WarningClose", 1.5f);
        }
    }

    public void NoMp()
    {
        if (!warning.activeSelf)
        {
            warning.SetActive(true);
            warningText.text = "MP가 부족합니다.";
            Invoke("WarningClose", 1.5f);
        }
    }

    public void SkillCooling()
    {
        if (!warning.activeSelf)
        {
            warning.SetActive(true);
            warningText.text = "아직 스킬을 사용할 수 없습니다.";
            Invoke("WarningClose", 1.5f);
        }

    }

    void WarningClose()
    {
        warning.SetActive(false);
    }

}
