using System.Collections;
using System.Net.Mail;
using UnityEngine;
using UnityEngine.UI;

public class BuffDebuff : MonoBehaviour
{
    [SerializeField] Transform buffParent;
    [SerializeField] GameObject statusEffectUI;
    Image statusUI;
    Color matUI;

    float amountPowerUp = 0f;
    float originMoveSpeed;

    // StatusEffect UI
    public void EffectOn(GameObject Marble)
    {
        GameObject go = Instantiate(statusEffectUI, buffParent);
        matUI = go.transform.Find("MatUI").GetComponent<Image>().color;
        statusUI = go.GetComponent<Image>();
        statusUI.sprite = Marble.GetComponent<Marble>().statusEffect.Sprite;
        matUI = Color.blue;

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
        //GetComponent<MP>().ManaExcel = amount;
        StartCoroutine("DeactiveManaRecovering", buffTime);
    }

    IEnumerator DeactiveManaRecovering(float buffTime)
    {
        yield return new WaitForSeconds(buffTime);
        //GetComponent<MP>().ManaExcel = 1f;
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
        //GetComponent<HP>().Defence = amount;
        StartCoroutine("DeactiveDefence", buffTime);
    }

    IEnumerator DeactiveDefence(float buffTime)
    {
        yield return new WaitForSeconds(buffTime);
        //GetComponent<HP>().Defence = 1f;
    }
}
