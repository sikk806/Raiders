using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Hp : MonoBehaviour
{

    public float Barrier;
    public bool IsNoDamaged;
    public float Defence; //����� ���� (%)
    
    [SerializeField]
    float MaxHp; //�ִ�HP
    [SerializeField]
    float CurrentHp; //����hp
    [SerializeField]
    Image HpBar;
    [SerializeField]
    TextMeshProUGUI HpText;

    private void Start()
    {
        if (HpText !=null)
        {
            //hptext 인스펙터에서 참조하기 
            HpText.text = CurrentHp + "/" + MaxHp;
        }
        
    }

    public void Heal(float heal)
    {
        if (HpBar ==null)
        {
            return;
        }
        
        CurrentHp += heal;
        CurrentHp = Mathf.Clamp(CurrentHp, 0f, MaxHp);
        HpBar.fillAmount += heal;
        HpText.text = CurrentHp + "/" + MaxHp;

    }

    public void TakeDamage(float damage)
    {
        damage *= (1f - Defence);
        //���� ���¶�� ó������ ����
        if (IsNoDamaged) { return; }
        else if (Barrier > 0f)
        {
            float lastDamage = damage - Barrier;

            Barrier -= damage;
            Barrier = Mathf.Clamp(Barrier, 0f, Mathf.Infinity);

            if (lastDamage >= 0f)
            {
                damage = lastDamage;

                //Hp�� ��������ŭ �پ��
                CurrentHp -= damage;
                HpBar.fillAmount -= damage;
                HpText.text = CurrentHp + "/" + MaxHp;

                //Hp ���� ���� ó��
                CurrentHp = Mathf.Clamp(CurrentHp, 0, MaxHp);

                //Hp�� 0 ���϶��
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
            //Hp�� ��������ŭ �پ��
            CurrentHp -= damage;
            HpBar.fillAmount = CurrentHp / MaxHp;

            //Hp ���� ���� ó��
            CurrentHp = Mathf.Clamp(CurrentHp, 0, MaxHp);

            //Hp�� 0 ���϶��
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

    public void BarrierReset() //���� ���� (����, Invoke ���ؼ� ȣ���ϴ� ������ �����ð� ����)
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


    public IEnumerator NoDamage(float noDamageTime) //���� �ο� �ڷ�ƾ
    {
        //�̹� ���� ���¶��, �ڷ�ƾ ����
        if (IsNoDamaged) yield break;

        //���� ���·� ����
        IsNoDamaged = true;

        //NoDamageTime��ŭ ���� ���� ����
        yield return new WaitForSeconds(noDamageTime);

        //���� ���� ����
        IsNoDamaged = false;
    }

    public void PlayerDie()
    {
        //���� �÷��̾� ���¸� Null�� ����
        Player.Instance.CurrentState = PlayerState.Null;

        //Ű�׼� ���� ����
        Player.Instance.TakeControl();

        //����ī��Ʈ ����
        GameManager.Instance.DeathCountDown();

        //Death �ִϸ��̼� ���
        Player.Instance.animator.SetTrigger("Death");
        
        

    }

    public void Resurrection() //��Ȱ ó��
    {
        //���� �ο�
        StartCoroutine(NoDamage(Player.Instance.NoDamageTime));

        //Ű�׼� �籸��
        Player.Instance.BringBackControl();

        //�ִ� Hp �ʱ�ȭ
        MaxHp = 100f;

        //�ִ� Mp �ʱ�ȭ
        Player.Instance.MaxMp = 1000f;

        //���� Hp �ʱ�ȭ
        CurrentHp = MaxHp;
        HpBar.fillAmount = CurrentHp / MaxHp;
        HpText.text = CurrentHp + "/" + MaxHp;

        //���� Mp �ʱ�ȭ
        Player.Instance.CurrentMp = Player.Instance.MaxMp;
        Player.Instance.MpBar.fillAmount = Player.Instance.CurrentMp / Player.Instance.MaxMp;
        Player.Instance.MpText.text = Player.Instance.CurrentMp + "/" + Player.Instance.MaxMp;

        //���� �÷��̾� ���¸� Idle�� ����
        Player.Instance.CurrentState = PlayerState.Idle;
    }


}
