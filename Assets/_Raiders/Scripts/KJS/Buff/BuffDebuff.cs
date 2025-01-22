using System.Collections;
using System.Net.Mail;
using UnityEngine;
using UnityEngine.UI;

public class BuffDebuff : MonoBehaviour
{
    [SerializeField] Transform buffParent;
    [SerializeField] Transform debuffParent;
    [SerializeField] Transform passiveParent;
    [SerializeField] GameObject statusEffectUI;
    [SerializeField] GameObject passiveEffectUI;
    Image statusUI;

    float originPower;
    float amountPowerUp = 0f;
    float originMoveSpeed;

    // StatusEffect UI
    public void EffectOn(GameObject SE)
    {
        GameObject go = new GameObject();
        if (SE.GetComponent<Marble>().statusEffect.StatusType == StatusEffect.StatusTypes.Buff)
        {
            go = Instantiate(statusEffectUI, buffParent);
            go.transform.Find("MatUI").GetComponent<Image>().color = Color.green;
        }
        else if (SE.GetComponent<Marble>().statusEffect.StatusType == StatusEffect.StatusTypes.Debuff)
        {
            go = Instantiate(statusEffectUI, debuffParent);
            go.transform.Find("MatUI").GetComponent<Image>().color = Color.red;
        }
        statusUI = go.GetComponent<Image>();
        statusUI.sprite = SE.GetComponent<Marble>().statusEffect.Sprite;

        StartCoroutine("EffectOff", go);
    }

    IEnumerator EffectOff(GameObject statusEffect)
    {
        yield return new WaitForSeconds(5f);

        Destroy(statusEffect.gameObject);
    }

    // Increase In Percentage
    // Power Up Section
    public void PowerUp(float amount, float buffTime)
    {
        amountPowerUp = GetComponent<Player>().Power * amount;
        GetComponent<Player>().AddedPower += amountPowerUp;
        StartCoroutine("DeactivePowerUp", buffTime);
    }

    IEnumerator DeactivePowerUp(float buffTime)
    {
        yield return new WaitForSeconds(buffTime);
        GetComponent<Player>().AddedPower -= amountPowerUp;
    }

    // Skill Cool Down Section
    public void CoolDown(float amount, float buffTime)
    {
        //GameManager.Instance.Skill.CoolExcel = amount;
        StartCoroutine("DeactiveCoolDown", buffTime);
    }

    IEnumerator DeactiveCoolDown(float buffTime)
    {
        yield return new WaitForSeconds(buffTime);
        //GameManager.Instance.Skill.CoolExcel = 1f;
    }

    // Mana Recovering Scetion
    public void ManaRecovering(float amount, float buffTime)
    {
        //GetComponent<Mp>().ManaExcel = amount;
        StartCoroutine("DeactiveManaRecovering", buffTime);
    }

    IEnumerator DeactiveManaRecovering(float buffTime)
    {
        yield return new WaitForSeconds(buffTime);
        //GetComponent<Mp>().ManaExcel = 1f;
    }

    // Speed Up Section
    public void SpeedUp(float amount, float buffTime)
    {
        originMoveSpeed = GetComponent<Player>().MoveSpeed;
        GetComponent<Player>().MoveSpeed += amount;
        StartCoroutine("DeactiveSpeedUp", buffTime);
    }

    IEnumerator DeactiveSpeedUp(float buffTime)
    {
        yield return new WaitForSeconds(buffTime);
        GetComponent<Player>().MoveSpeed = originMoveSpeed;
    }

    // Defence Section
    public void Defence(float amount, float buffTime)
    {
        GetComponent<Hp>().Defence = amount;
        StartCoroutine("DeactiveDefence", buffTime);
    }

    IEnumerator DeactiveDefence(float buffTime)
    {
        yield return new WaitForSeconds(buffTime);
        GetComponent<Hp>().Defence = 1f;
    }

    // Card Section
    public void SelectCard(string cardName)
    {
        switch (cardName)
        {
            case "Berserker":
                break;
            case "Dedication":
                break;
            default:
                Debug.Log("No name.");
                break;
        }
    }

    public void PassiveOn(StatusEffect statusEffect)
    {
        GameObject go = new GameObject();
        go = Instantiate(passiveEffectUI, passiveParent);
        statusUI = go.GetComponent<Image>();
        statusUI.sprite = statusEffect.Sprite;
    }

    // 광전사 - 받는 데미지 증가하지만 공격력 n% 증가
    public void Berserker()
    {
        originPower = GetComponent<Player>().Power;
        GetComponent<Player>().Power *= 1.3f;
        GetComponent<Hp>().Defence = 0.6f;
    }

    // 일생일사 - 부활 1회로 변경하고 기본 공격력 n% 증가
    // GameManager 수정 안되었을 때 작업할 것.
    // 1. GameManager 에서 부활 횟수를 1회로 줄이기 위해 public 을 만들어야한다.
    public void Dedication()
    {
        // GameManger.Instance.deathCount = 1;
        originPower = GetComponent<Player>().Power;
        GetComponent<Player>().Power *= 1.5f;
    }

    // 흡혈 - 타격 시 체력 회복
    public void DrainHp()
    {
        // 스킬에 함수 추가가 필요.
    }

    public void CoolDown()
    {
        // Skill static을 뺄 것.
        //GameManager.Instance.Skill.CoolExcel = amount;
    }

    public void RollingCool()
    {
        // 플레이어의 구르기 쿨타임이 필요함.
    }

    // 기본 공격력 증가
    public void PowerUp()
    {
        originPower = GetComponent<Player>().Power;
        GetComponent<Player>().Power *= 1.1f;
    }

    public void SpeedUp()
    {
        GetComponent<Player>().MoveSpeed *= 1.1f;
    }

    public void HealthUp()
    {
        GetComponent<Player>().MaxHp *= 300f;
        GetComponent<Player>().CurrentHp = GetComponent<Player>().MaxHp;
    }

    public void AddPotion()
    {
        // 포션 갯수가 어딨지
    }

    public void OneMore()
    {
        // GameManager private 바꿀 것.
    }
}
