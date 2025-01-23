using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    private StatusEffect statusEffect;
    public StatusEffect StatusEffect
    {
        get
        {
            return statusEffect;
        }
        set
        {
            statusEffect = value;
        }
    }


    [SerializeField] Sprite CardFront;
    [SerializeField] Sprite CardBack;

    [SerializeField] GameObject CardImage;
    [SerializeField] TMP_Text CardName;
    [SerializeField] TMP_Text CardInfo;

    GameObject player;

    public void SettingCard()
    {
        if(statusEffect == null)
        {
            Debug.Log("There is No Status Effect Object Script.");
            return;
        }
        GetComponent<Image>().sprite = CardBack;
    }

    public void SetPlayer(GameObject go)
    {
        player = go;
    }

    public void CardOpen()
    {
        GetComponent<Image>().sprite = CardFront;
        CardImage.GetComponent<Image>().sprite = statusEffect.Sprite;
        CardImage.GetComponent<Image>().color = new Color(1, 1, 1, 1);
        CardName.text = statusEffect.StatusEffectName;
        CardInfo.text = statusEffect.Info;
        GetComponent<Button>().interactable = true;
    }

    public void CardClose()
    {
        GetComponent<Image>().sprite = CardBack;
        CardImage.GetComponent<Image>().sprite = null;
        CardImage.GetComponent<Image>().color = Color.clear;
        CardName.text = "";
        CardInfo.text = "";
    }

    public void OnClick()
    {
        player.GetComponent<CardSelect>().CardClose(statusEffect);
    }

    public void ButtonDisable()
    {
        GetComponent<Button>().interactable = false;
    }
}
