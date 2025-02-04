using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

public class Hp : MonoBehaviour
{
    public float Barrier;
    public bool IsNoDamaged;
    public float  Defence;

    // 처음에 대문자로 변수가 적혀있어서 public을 소문자로 함. (VSC에서는 명명 규칙 위반이라지만 내맴)
    [SerializeField]
    private float MaxHp;
    public float maxHp { get { return MaxHp; } set { MaxHp = value; }}

    [SerializeField]
    float CurrentHp;
    public float currentHp { get { return CurrentHp; } set { CurrentHp = value; }}

    [SerializeField]
    Image HpBar;
    [SerializeField]
    TextMeshProUGUI HpText;

    private void Start()
    {
        if (HpText != null)
        {
            //hptext 인스펙터에서 참조하기 
            HpText.text = CurrentHp + "/" + MaxHp;
        }
    }

    public void HpHeal(float heal)
    {
        if (HpBar == null)
        {
            return;
        }

        CurrentHp += heal;
        CurrentHp = Mathf.Clamp(CurrentHp, 0f, MaxHp);
        HpBar.fillAmount = CurrentHp / MaxHp;
        HpText.text = CurrentHp + "/" + MaxHp;

    }

    public void TakeDamage(float damage)
    {
      
        //1이 100퍼   0이 되면 데미지가 x
        damage *= (1f - Defence);
        if (IsNoDamaged) { return; }
        else if (Barrier > 0f)
        {
            float lastDamage = damage - Barrier;

            Barrier -= damage;
            Barrier = Mathf.Clamp(Barrier, 0f, Mathf.Infinity);

            if (lastDamage >= 0f)
            {
                damage = lastDamage;

                CurrentHp -= damage;
                HpBar.fillAmount = CurrentHp / MaxHp;
                HpText.text = CurrentHp + "/" + MaxHp;

                CurrentHp = Mathf.Clamp(CurrentHp, 0, MaxHp);

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
            
            CurrentHp -= damage;
            HpBar.fillAmount = CurrentHp / MaxHp;

            CurrentHp = Mathf.Clamp(CurrentHp, 0, MaxHp);

            if (CurrentHp <= 0f)
            {
                if (CompareTag("Player"))
                {
                    PlayerDie();
                }
                else if (CompareTag("Boss1"))
                {
                    //패턴을 했는지 확인
                    Boss1Die();
                }
                else if (CompareTag("Boss2"))
                {
                    Boss2Die();
                }

            }
        }
    }

    
    /*
     * 수정필요 매개변수 필요함
     */
    public void BarrierSet()
    {
        //test로 값을 높힘
        Barrier = 3000f;
        HpText.text = CurrentHp + "(+" + Barrier + ")" + "/" + MaxHp;
    }

    public void BarrierReset()
    {
        Barrier = 0f;
        HpText.text = CurrentHp + "/" + MaxHp;
    }

    public void Boss1Die()
    {
        //패턴 했냐? 안했냐
        if (GetComponent<BehaviourAI>().isDoPattern)
        {
            GetComponent<Animator>().SetTrigger("Death");
            Player.Instance.GetComponent<Hp>().Defence = 1f;
        }
        else
        {
            return;
        }

    }

    public void Boss2Die()
    {

    }


    public IEnumerator NoDamage(float noDamageTime)
    {
        if (IsNoDamaged) yield break;

        IsNoDamaged = true;

        yield return new WaitForSeconds(noDamageTime);

        IsNoDamaged = false;
    }

    public void PlayerDie()
    {
        Player.Instance.CurrentState = PlayerState.Null;

        Player.Instance.TakeControl();

        GameManager.Instance.DeathCountDown();

        Player.Instance.animator.SetTrigger("Death");



    }

    public void Resurrection()
    {
        StartCoroutine(NoDamage(Player.Instance.NoDamageTime));

        Player.Instance.BringBackControl();

        MaxHp = 100f; //Temp

        Player.Instance.MaxMp = 1000f; //Temp

        CurrentHp = MaxHp;
        HpBar.fillAmount = CurrentHp / MaxHp;
        HpText.text = CurrentHp + "/" + MaxHp;

        Player.Instance.CurrentMp = Player.Instance.MaxMp;
        Player.Instance.MpBar.fillAmount = Player.Instance.CurrentMp / Player.Instance.MaxMp;
        Player.Instance.MpText.text = Player.Instance.CurrentMp + "/" + Player.Instance.MaxMp;

        Player.Instance.CurrentState = PlayerState.Idle;
    }

        // 2025-01-23
    public void HealthPassiveOn(float amount)
    {
        MaxHp *= amount;
        CurrentHp = MaxHp;
        HpText.text = CurrentHp + "/" + MaxHp;
    }
}