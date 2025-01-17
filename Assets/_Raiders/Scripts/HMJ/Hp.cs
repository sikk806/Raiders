using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Hp : MonoBehaviour
{

    public float Barrier;
    public bool IsNoDamaged;

    [SerializeField]
    float MaxHp; //최대HP
    [SerializeField]
    float CurrentHp; //현재hp
    [SerializeField]
    Image HpBar;
    [SerializeField]
    TextMeshProUGUI HpText;

    private void Start()
    {
        HpText.text = CurrentHp + "/" + MaxHp;
    }

    public void Heal(float heal)
    {
        CurrentHp += heal;
        CurrentHp = Mathf.Clamp(CurrentHp, 0f, MaxHp);
        HpBar.fillAmount += heal;
        HpText.text = CurrentHp + "/" + MaxHp;

    }

    public void TakeDamage(float damage)
    {
        //무적 상태라면 처리하지 않음
        if (IsNoDamaged) { return; }
        else if (Barrier > 0f)
        {
            float lastDamage = damage - Barrier;

            Barrier -= damage;
            Barrier = Mathf.Clamp(Barrier, 0f, Mathf.Infinity);

            if (lastDamage >= 0f)
            {
                damage = lastDamage;

                //Hp가 데미지만큼 줄어듬
                CurrentHp -= damage;
                HpBar.fillAmount -= damage;
                HpText.text = CurrentHp + "/" + MaxHp;

                //Hp 음수 방지 처리
                CurrentHp = Mathf.Clamp(CurrentHp, 0, MaxHp);

                //Hp가 0 이하라면
                if (CurrentHp <= 0f)
                {
                    if (CompareTag("Player"))
                    {
                        PlayerDie();
                    }
                    else if (CompareTag("Boss1"))
                    {
                        Boss1Die();
                    }
                    else if (CompareTag("Boss2"))
                    {
                        Boss2Die();
                    }
                }
            }
           
            }
        else
        {
            //Hp가 데미지만큼 줄어듬
            CurrentHp -= damage;
            HpBar.fillAmount = CurrentHp / MaxHp;

            //Hp 음수 방지 처리
            CurrentHp = Mathf.Clamp(CurrentHp, 0, MaxHp);

            //Hp가 0 이하라면
            if (CurrentHp <= 0f)
            {
                if (CompareTag("Player"))
                {
                    PlayerDie();
                }
                else if (CompareTag("Boss1"))
                {
                    Boss1Die();
                }
                else if (CompareTag("Boss2"))
                {
                    Boss2Die();
                }

            }
        }



    }

    public void BarrierSet()
    {
        Barrier = 50f;
        HpText.text = CurrentHp + "(+" + Barrier + ")" + "/" + MaxHp;
    }

    public void BarrierReset() //쉴드 리셋 (사용시, Invoke 통해서 호출하는 것으로 유지시간 설정)
    {
        Barrier = 0f;
        HpText.text = CurrentHp + "/" + MaxHp;
    }

    public void Boss1Die()
    {

    }

    public void Boss2Die()
    {

    }


    public IEnumerator NoDamage(float noDamageTime) //무적 부여 코루틴
    {
        //이미 무적 상태라면, 코루틴 종료
        if (IsNoDamaged) yield break;

        //무적 상태로 변경
        IsNoDamaged = true;

        //NoDamageTime만큼 무적 상태 유지
        yield return new WaitForSeconds(noDamageTime);

        //무적 상태 해제
        IsNoDamaged = false;
    }

    public void PlayerDie()
    {
        //현재 플레이어 상태를 Null로 변경
        Player.Instance.CurrentState = PlayerState.Null;

        //키액션 구독 해지
        Player.Instance.TakeControl();

        //데스카운트 감소
        GameManager.Instance.DeathCountDown();

        //Death 애니메이션 재생
        Player.Instance.animator.SetTrigger("Death");

    }

    public void Resurrection() //부활 처리
    {
        //무적 부여
        StartCoroutine(NoDamage(Player.Instance.NoDamageTime));

        //키액션 재구독
        Player.Instance.BringBackControl();

        //최대 Hp 초기화
        MaxHp = 100f;

        //최대 Mp 초기화
        Player.Instance.MaxMp = 1000f;

        //현재 Hp 초기화
        CurrentHp = MaxHp;
        HpBar.fillAmount = CurrentHp / MaxHp;
        HpText.text = CurrentHp + "/" + MaxHp;

        //현재 Mp 초기화
        Player.Instance.CurrentMp = Player.Instance.MaxMp;
        Player.Instance.MpBar.fillAmount = Player.Instance.CurrentMp / Player.Instance.MaxMp;
        Player.Instance.MpText.text = Player.Instance.CurrentMp + "/" + Player.Instance.MaxMp;

        //현재 플레이어 상태를 Idle로 변경
        Player.Instance.CurrentState = PlayerState.Idle;
    }


}
