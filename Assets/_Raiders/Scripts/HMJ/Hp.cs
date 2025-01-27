using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

public class Hp : MonoBehaviour
{
    public float Barrier;
    public bool IsNoDamaged;
    public float Defence;

    [SerializeField]
    float MaxHp;

    [SerializeField]
    float CurrentHp;
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

    public void BarrierReset()
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