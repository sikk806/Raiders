using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class E : MonoBehaviour
{
    public static E Instance;
    public TextMeshProUGUI ECoolTime;
    public Image ECool;
    public bool IsUsable = true; //스킬 사용 가능 여부 (쿨타임에 의함)
    public float CoolTime = 8f; //스킬 쿨타임
    public float UseMp = 50f; //스킬 소모 Mp

    void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        //게임 시작 시, 쿨타임 초기화
        CoolTime = 0;
        ECoolTime.text = "";
        ECool.fillAmount = 0;
    }


    private void Update()
    {
        //스킬 사용 불가능일 때면
        if (!IsUsable)
        {
            //시간 따라 쿨타임 돔
            CoolTime -= Time.deltaTime * SkillManager.instance.CoolExcel;

            //쿨타임 음수 방지 처리
            CoolTime = Mathf.Clamp(CoolTime, 0, Mathf.Infinity);

            ECoolTime.text = ((int)CoolTime).ToString();
            ECool.fillAmount = CoolTime / 8;

            //쿨타임 0 이하면
            if (CoolTime <= 0)
            {
                //스킬 사용 가능
                IsUsable = true;
                ECoolTime.text = "";
            }
        }
    }

    public void UseSkill() //스킬 사용
    {
        //스킬 소모 Mp만큼 플레이어 Mp 소모
        Player.Instance.CurrentMp -= UseMp;
        Player.Instance.MpBar.fillAmount = Player.Instance.CurrentMp / Player.Instance.MaxMp;
        Player.Instance.MpText.text = Player.Instance.CurrentMp + "/" + Player.Instance.MaxMp;

        //쿨타임 적용
        StartCoolDown();
    }

    private void StartCoolDown() //쿨타임 적용
    {
        //스킬 사용 불가능 처리
        IsUsable = false;

        ECool.fillAmount = 1;

        ECoolTime.text = ((int)CoolTime).ToString();

        //쿨타임 8초 부여
        CoolTime = 8f;
    }
}
