using System.Collections;
using System.Net.Mail;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuffDebuff : MonoBehaviour
{
    [SerializeField] Transform buffParent;
    [SerializeField] Transform debuffParent;
    [SerializeField] Transform passiveParent;
    [SerializeField] GameObject statusEffectUI;
    [SerializeField] GameObject passiveEffectUI;
    [SerializeField] TextMeshProUGUI atkText;
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
        atkText.text = $"ATK:{(int)Player.Instance.Power} (+(int){Player.Instance.AddedPower})";
        StartCoroutine("DeactivePowerUp", buffTime);
    }

    IEnumerator DeactivePowerUp(float buffTime)
    {
        yield return new WaitForSeconds(buffTime);
        GetComponent<Player>().AddedPower -= amountPowerUp;
        atkText.text = $"ATK:{(int)Player.Instance.Power} (+(int){Player.Instance.AddedPower})";
    }

    // Skill Cool Down Section
    public void CoolDown(float amount, float buffTime)
    {
        GameManager.Instance.Skill.CoolExcel = amount;
        StartCoroutine("DeactiveCoolDown", buffTime);
    }

    IEnumerator DeactiveCoolDown(float buffTime)
    {
        yield return new WaitForSeconds(buffTime);
        GameManager.Instance.Skill.CoolExcel = 1f;
    }

    // Mana Recovering Scetion
    public void ManaRecovering(float amount, float buffTime)
    {
        GetComponent<Player>().ManaExcel = amount;
        StartCoroutine("DeactiveManaRecovering", buffTime);
    }

    IEnumerator DeactiveManaRecovering(float buffTime)
    {
        yield return new WaitForSeconds(buffTime);
        GetComponent<Player>().ManaExcel = 1f;
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

    public void PassiveOn(StatusEffect statusEffect)
    {
        GameObject go = new GameObject();
        go = Instantiate(passiveEffectUI, passiveParent);
        statusUI = go.GetComponent<Image>();
        statusUI.sprite = statusEffect.Sprite;
        SendMessage(statusEffect.name);
    }

    // 광전사 - 받는 데미지 증가하지만 공격력 n% 증가
    public void Berserker()
    {
        originPower = GetComponent<Player>().Power;
        GetComponent<Player>().Power *= 1.3f;
        GetComponent<Hp>().Defence = 0.3f;
        atkText.text = $"ATK:{(int)Player.Instance.Power} (+(int){Player.Instance.AddedPower})";
    }

    // 일생일사 - 부활 1회로 변경하고 기본 공격력 n% 증가
    // GameManager 수정 안되었을 때 작업할 것.
    // 1. GameManager 에서 부활 횟수를 1회로 줄이기 위해 public 을 만들어야한다.
    public void Dedication()
    {
        GameManager.Instance.DeathCount = 1;
        originPower = GetComponent<Player>().Power;
        GetComponent<Player>().Power *= 1.5f;
        Player.Instance.OneLife = true;
        atkText.text = $"ATK:{(int)Player.Instance.Power} (+(int){Player.Instance.AddedPower})";
    }

    // 흡혈 - 타격 시 체력 회복
    public void DrainHp()
    {
        GetComponent<Player>().IsDrainHp = true;
    }

    // 쿨타임 감소
    public void ReduceCoolTime()
    {
        // Skill static을 뺄 것.
        GameManager.Instance.Skill.CoolExcel = 1.2f;
    }

    // 구르기 쿨타임 감소
    public void RollingCool()
    {
        GetComponent<Player>().RollingCoolTime *= 0.7f; // 쿨타임 30% 감소
    }

    // 기본 공격력 증가
    public void BasePowerUp()
    {
        originPower = GetComponent<Player>().Power;
        GetComponent<Player>().Power *= 1.1f;
        atkText.text = $"ATK:{(int)Player.Instance.Power} (+(int){Player.Instance.AddedPower})";
    }

    // 이속 증가
    public void MoveSpeedUp()
    {
        GetComponent<Player>().MoveSpeed *= 1.1f;
    }

    // 최대 체력 증가
    public void HealthUp()
    {
        GetComponent<Hp>().HealthPassiveOn(1.3f);
    }

    // 포션 개수 증가
    public void AddPotion()
    {
        Potion.Instance.HpPotion++;
        Potion.Instance.MpPotion++;

        Potion.Instance.HpPotionText.text = Potion.Instance.HpPotion.ToString();
        Potion.Instance.MpPotionText.text = Potion.Instance.MpPotion.ToString();
    }

    // 1회 부활 추가
    public void OneMore()
    {
        if (!GetComponent<Player>().OneLife)
        {
            GameManager.Instance.DeathCount++;
        }
    }
}
