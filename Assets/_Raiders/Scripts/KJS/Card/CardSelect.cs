using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardSelect : MonoBehaviour
{
    [SerializeField] Transform cardParent;
    [SerializeField] GameObject cardSelectPanel;
    [SerializeField] GameObject cardPrefab;

    [SerializeField] StatusEffect[] cardEffects;

    GameObject leftCard;
    GameObject middleCard;
    GameObject rightCard;

    Quaternion originRotation;

    List<StatusEffect> usableCardSets;

    public int cardSelectCnt;

    bool openLeft;
    bool openMiddle;
    bool openRight;

    bool closeLeft;
    bool closeMiddle;
    bool closeRight;

    public bool onceL;
    bool onceM;
    bool onceR;

    bool open;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        usableCardSets = new List<StatusEffect>();

        cardSelectCnt = 3;

        openLeft = false;
        openMiddle = false;
        openRight = false;

        onceL = false;
        onceM = false;
        onceR = false;

        open = false;

        foreach (var card in cardEffects)
        {
            usableCardSets.Add(card);
        }

        InitCards(3);
    }

    // Update is called once per frame
    void Update()
    {
        if (cardSelectCnt > 0)
        {
            if (open) Open();
            else Close();
        }
    }

    void Open()
    {
        if (openLeft)
        {
            leftCard.transform.rotation = Quaternion.Lerp(leftCard.transform.rotation, Quaternion.identity, Time.deltaTime);
            if (leftCard.transform.rotation.y >= -0.7f && !onceL)
            {
                onceL = true;
                leftCard.GetComponent<Card>().CardOpen();
            }
            else if (leftCard.transform.rotation.y >= 0.05f)
            {
                openLeft = false;
            }
        }
        if (openMiddle)
        {
            middleCard.transform.rotation = Quaternion.Lerp(middleCard.transform.rotation, Quaternion.identity, Time.deltaTime);
            if (middleCard.transform.rotation.y >= -0.7f && !onceM)
            {
                onceM = true;
                middleCard.GetComponent<Card>().CardOpen();
            }
            else if (middleCard.transform.rotation.y >= 0.05f)
            {
                openMiddle = false;
            }
        }
        if (openRight)
        {
            rightCard.transform.rotation = Quaternion.Lerp(rightCard.transform.rotation, Quaternion.identity, Time.deltaTime);
            if (rightCard.transform.rotation.y >= -0.7f && !onceR)
            {
                onceR = true;
                rightCard.GetComponent<Card>().CardOpen();
            }
            else if (rightCard.transform.rotation.y >= 0.05f)
            {
                openRight = false;
            }
        }
    }

    void Close()
    {
        if (closeLeft)
        {
            leftCard.transform.rotation = Quaternion.Lerp(leftCard.transform.rotation, new Quaternion(0f, -1f, 0f, 0f), Time.deltaTime);
            if (leftCard.transform.rotation.y <= -0.7f && !onceL)
            {
                onceL = true;
                leftCard.GetComponent<Card>().CardClose();
            }
            else if (leftCard.transform.rotation.y <= -0.95f)
            {
                onceL = false;
                closeLeft = false;
            }
        }
        if (closeMiddle)
        {
            middleCard.transform.rotation = Quaternion.Lerp(middleCard.transform.rotation, new Quaternion(0f, -1f, 0f, 0f), Time.deltaTime);
            if (middleCard.transform.rotation.y <= -0.7f && !onceM)
            {
                onceM = true;
                middleCard.GetComponent<Card>().CardClose();
            }
            else if (middleCard.transform.rotation.y <= -0.95f)
            {
                onceM = false;
                closeMiddle = false;
            }
        }
        if (closeRight)
        {
            rightCard.transform.rotation = Quaternion.Lerp(rightCard.transform.rotation, new Quaternion(0f, -1f, 0f, 0f), Time.deltaTime);
            if (rightCard.transform.rotation.y <= -0.7f && !onceR)
            {
                onceR = true;
                rightCard.GetComponent<Card>().CardClose();
                cardSelectCnt--;
                if (cardSelectCnt == 0)
                {
                    cardSelectPanel.SetActive(false);
                }
            }
            else if (rightCard.transform.rotation.y <= -0.95f)
            {
                onceR = false;
                closeRight = false;
                CardDestroy();
                InitCards(cardSelectCnt);
            }
        }
    }

    void CardDestroy()
    {
        Destroy(leftCard);
        Destroy(middleCard);
        Destroy(rightCard);
    }

    public void InitCards(int cnt)
    {
        cardSelectCnt = cnt;
        List<int> selectedCards = SelectCard();

        leftCard = Instantiate(cardPrefab, cardParent);
        leftCard.GetComponent<Card>().StatusEffect = usableCardSets[selectedCards[0]];
        leftCard.GetComponent<Card>().SettingCard();
        leftCard.GetComponent<Card>().SetPlayer(gameObject);

        middleCard = Instantiate(cardPrefab, cardParent);
        middleCard.GetComponent<Card>().StatusEffect = usableCardSets[selectedCards[1]];
        middleCard.GetComponent<Card>().SettingCard();
        middleCard.GetComponent<Card>().SetPlayer(gameObject);

        rightCard = Instantiate(cardPrefab, cardParent);
        rightCard.GetComponent<Card>().StatusEffect = usableCardSets[selectedCards[2]];
        rightCard.GetComponent<Card>().SettingCard();
        rightCard.GetComponent<Card>().SetPlayer(gameObject);

        InitOpen();
    }

    void InitOpen()
    {
        onceL = false;
        onceM = false;
        onceR = false;
        open = true;
        openLeft = true;
        openMiddle = true;
        openRight = true;
    }

    void InitClose()
    {
        onceL = false;
        onceM = false;
        onceR = false;
        open = false;
        closeLeft = true;
        closeMiddle = true;
        closeRight = true;
    }

    List<int> SelectCard()
    {
        HashSet<int> numbers = new HashSet<int>();
        while (numbers.Count < 3)
        {
            if (usableCardSets == null) return null;
            int num = Random.Range(0, usableCardSets.Count);
            numbers.Add(num);
        }

        return new List<int>(numbers);
    }

    public void CardClose(StatusEffect statusEffect)
    {
        InitClose();
        leftCard.GetComponent<Card>().ButtonDisable();
        middleCard.GetComponent<Card>().ButtonDisable();
        rightCard.GetComponent<Card>().ButtonDisable();
        GetComponent<BuffDebuff>().PassiveOn(statusEffect);
        if(statusEffect.name == "Berserker" || statusEffect.name == "Dedication" || statusEffect.name == "DrainHp")
        {
            foreach(var usableCard in usableCardSets)
            {
                if(usableCard.name == statusEffect.name)
                {
                    usableCardSets.Remove(usableCard);
                    return;
                }
            }
        }
    }
}
