using System.Collections;
using UnityEngine;

public class Q : MonoBehaviour
{
    public static Q Instance;
    public bool IsUsable = true; //스킬 사용 가능 여부 (쿨타임에 의함)
    public float CoolTime = 3f; //스킬 쿨타임
    public static float SkillDamage = 100f; //스킬 고유 데미지
    public float UseMp = 30f; //스킬 소모 Mp

    void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        //게임 시작 시, 쿨타임 초기화
        CoolTime = 0;
    }

    public static float CalculatingDamage()
    {
        return SkillDamage + Player.Instance.Power + Player.Instance.AddedPower;
    }

    private void Update()
    {
        //스킬 사용 불가능일 때면
        if (!IsUsable)
        {
            //시간 따라 쿨타임 돔
            CoolTime -= Time.deltaTime;

            //쿨타임 음수 방지 처리
            CoolTime = Mathf.Clamp(CoolTime, 0, Mathf.Infinity);

            //쿨타임 0 이하면
            if (CoolTime <= 0)
            {
                //스킬 사용 가능
                IsUsable = true;
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

        //쿨타임 3초 부여
        CoolTime = 3f;
    }
}